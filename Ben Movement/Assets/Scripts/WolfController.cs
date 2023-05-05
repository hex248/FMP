using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WolfController : MonoBehaviour
{
    TrailDrawer trailDrawer;
    Rigidbody rb;
    float timeSinceStart;


    [Header("Movement")]
    [SerializeField] float moveSpeed;
    [SerializeField] float moveRotationSpeed;
    [SerializeField] float aimRotationSpeed;
    [SerializeField] float inRangeMoveSpeed;
    [SerializeField] float stunMaxSpeedChange;
    Quaternion rotationalDirection;

    [Header("Attack")]
    [SerializeField] float attackTime;
    [SerializeField] float attackMoveDistance;
    [SerializeField] float attackMoveTime;
    [SerializeField] float damageTime;
    [SerializeField] float biteSFXDelay = 0.25f;



    bool doneAttackHit;
    float timeSinceAttackEnd;
    float timeSinceLastAttacking;
    Vector3 currentAttackDirection;
    bool isAttacking;
    bool biteSFXPlayed;



    [SerializeField] float minAttackPrepareTime;
    bool isPreparingAttack;
    float timeSincePrepare;

    [SerializeField] float missedAttackStunTime;
    float timeSinceStunned = 0f;
    bool isMissedAttackStunned;

    [SerializeField] float damagedStunnedTime;
    bool isDamageStunned;
    float timeSinceDamaged;
    bool isDeathStunned;

    [SerializeField] Vector3 hitboxOffset;
    [SerializeField] Vector3 hitboxSize; 
    [SerializeField] LayerMask attackLayerMask;
    [SerializeField] float attackForce;

    [Header("Visuals")]
    [SerializeField] GameObject wolfVisuals;
    WolfAnimationScript wolfAnimationScript;

    [Header("Behavior")]
    [SerializeField] float maxAttackRange;
    public GameObject currentTarget;
    [SerializeField] bool targetPlayer;
    Bed bed;
    Vector3 moveDirection;

    [SerializeField] LayerMask raycastLayer;

    [Header("Navmesh Settings")]
    private NavMeshAgent navMesh;
    bool useNavMesh;

    AudioManager AM;

    void Start()
    {
        trailDrawer = GetComponent<TrailDrawer>();
        trailDrawer.enabled = false;
        rb = GetComponent<Rigidbody>();
        bed = FindObjectOfType<Bed>();
        AM = FindObjectOfType<AudioManager>();
        wolfAnimationScript = GetComponentInChildren<WolfAnimationScript>();
        currentTarget = bed.gameObject;
        timeSinceLastAttacking = 0f;

        navMesh = GetComponent<NavMeshAgent>();
        navMesh.updatePosition = false;
        navMesh.updateRotation = false;
    }

    void FixedUpdate()
    {
        Vector3 offsetToTarget = currentTarget.transform.position - transform.position;
        Vector3 directionToTarget = offsetToTarget.normalized;
        float distanceToTarget = offsetToTarget.magnitude;
        bool targetIsInDirectView = false;

        RaycastHit hit;
        float minDistanceToTargetCollider = Mathf.Infinity;
        Vector3 raycastStart = transform.position + new Vector3(0f, 0.5f, 0f);
        Physics.Raycast(raycastStart, directionToTarget, out hit, Mathf.Infinity, raycastLayer);

        if (hit.collider != null)
        {
            targetIsInDirectView = (hit.collider.gameObject == currentTarget);
            minDistanceToTargetCollider = (hit.point - transform.position).magnitude;
        }

        navMesh.destination = currentTarget.transform.position;
        Debug.DrawRay(transform.position, navMesh.desiredVelocity.normalized, Color.red, Time.deltaTime);

        float navMeshTargetDirectionDot = Vector3.Dot(directionToTarget, navMesh.desiredVelocity.normalized);
        if (navMeshTargetDirectionDot >= 0.99f)
        {
            //normal behaviour
            useNavMesh = false;
            moveDirection = directionToTarget;
        }
        else
        {
            if (currentTarget == bed.gameObject)
            {
                navMesh.agentTypeID = GetAgentTypeIDByName("Attacking Bed");
            }
            else
            {
                navMesh.agentTypeID = GetAgentTypeIDByName("Attacking Player");
                AM.PlayInChannel("wolf_growl", ChannelType.SFX, 2);
            }
            //take directions from navmesh
            useNavMesh = true;
            moveDirection = navMesh.desiredVelocity.normalized;
        }

        if (useNavMesh && !isMovementLocked())
        {
            //don't attack while using navmesh

            RotateTowardsTarget();
            MoveForward(moveSpeed);
        }
        else if (!isMovementLocked())
        {
            if (minDistanceToTargetCollider <= maxAttackRange)
            {
                //in range

                if (Vector3.Dot(transform.forward, directionToTarget) >= 0.5f)
                {
                    //can see target
                    PrepareAttack();
                }


            }
            else
            {
                RotateTowardsTarget();
                MoveForward(moveSpeed);
            }
        }
        else if (isPreparingAttack && !isInteruptAttack())
        {
            if (timeSincePrepare >= minAttackPrepareTime)
            {
                //can see target
                StartAttack();
            }
            else
            {
                //need to aim
                AimTowardsTarget();
                MoveForward(inRangeMoveSpeed);
                timeSincePrepare += Time.deltaTime;
            }
        }
        else if (isMissedAttackStunned && !isInteruptAttack())
        {
            timeSinceStunned += Time.deltaTime;
            if (timeSinceStunned >= missedAttackStunTime)
            {
                wolfAnimationScript.Recover();
                isMissedAttackStunned = false;
                StartCoroutine(DisableTrailsAfter(1.0f));
            }
            MoveForward(0, stunMaxSpeedChange);
        }



        if (isAttacking && !isInteruptAttack())
        {
            timeSinceLastAttacking = 0f;
            DoMeleeAttackMove();
            if (timeSinceAttackEnd >= damageTime && doneAttackHit == false)
            {
                DoMeleeAttackHit();
            }
        }
        else
        {
            timeSinceLastAttacking += Time.deltaTime;
        }

        if (isDamageStunned)
        {
            if (timeSinceDamaged >= damagedStunnedTime)
            {
                isDamageStunned = false;
                wolfAnimationScript.EndDamageStun();
            }
            timeSinceDamaged += Time.deltaTime;
            MoveForward(0, stunMaxSpeedChange);
        }
        else if (isDeathStunned)
        {
            MoveForward(0, stunMaxSpeedChange);
        }


        navMesh.nextPosition = transform.position;
    }

    int GetAgentTypeIDByName(string agentTypeName)
    {
        int count = NavMesh.GetSettingsCount();
        string[] agentTypeNames = new string[count + 2];
        for (var i = 0; i < count; i++)
        {
            int id = NavMesh.GetSettingsByIndex(i).agentTypeID;
            string name = NavMesh.GetSettingsNameFromID(id);
            if (name == agentTypeName)
            {
                return id;
            }
        }
        return -1;
    }


    void MoveForward(float speed, float maxSpeedChange = Mathf.Infinity)
    {
        Vector3 desiredVelocity = wolfVisuals.transform.forward * speed;
        Vector3 newVelocity = Vector3.zero;
        newVelocity.x = Mathf.MoveTowards(rb.velocity.x, desiredVelocity.x, maxSpeedChange);
        newVelocity.z = Mathf.MoveTowards(rb.velocity.z, desiredVelocity.z, maxSpeedChange);

        rb.velocity = newVelocity;
    }

    void RotateTowardsTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

        rotationalDirection = Quaternion.Lerp(wolfVisuals.transform.rotation, targetRotation, moveRotationSpeed * Time.deltaTime);
        transform.rotation = rotationalDirection;
        //smoke.SetVector3("EmissionDirection", head.forward * -1);
    }

    void AimTowardsTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(currentTarget.transform.position - transform.position, Vector3.up);

        rotationalDirection = Quaternion.Lerp(wolfVisuals.transform.rotation, targetRotation, aimRotationSpeed * Time.deltaTime);
        transform.rotation = rotationalDirection;
        //smoke.SetVector3("EmissionDirection", head.forward * -1);
    }

    bool isMovementLocked()
    {
        return isAttacking || isPreparingAttack || isMissedAttackStunned || isDamageStunned || isDeathStunned;
    }

    bool isInteruptAttack()
    {
        return isDamageStunned || isDeathStunned;
    }

    void PrepareAttack()
    {
        isPreparingAttack = true;
        wolfAnimationScript.PrepareAttack();
        timeSincePrepare = 0f;
    }

    void StartAttack()
    {
        isPreparingAttack = false;
        isAttacking = true;
        wolfAnimationScript.StartAttackAnimation();

        //...
        timeSinceAttackEnd = 0f;
        doneAttackHit = false;
        currentAttackDirection = transform.forward;
    }

    void DoMeleeAttackMove()
    {
        if (timeSinceAttackEnd >= attackTime)
        {
            isAttacking = false;

            //time you were last  attacking

        }
        else if (timeSinceAttackEnd >= attackMoveTime)
        {
            //finished attack movement
        }
        else
        {
            //move player
            float attackSpeed = attackMoveDistance / attackMoveTime;
            MoveForward(attackSpeed);
        }
        timeSinceAttackEnd += Time.deltaTime;
    }

    void DoMeleeAttackHit()
    {
        doneAttackHit = true;
        Vector3 offset = Quaternion.AngleAxis(rotationalDirection.eulerAngles.y, Vector3.up) * hitboxOffset;
        Collider[] hitColliders = Physics.OverlapBox(transform.position + offset, (hitboxSize / 2f), rotationalDirection, attackLayerMask);
        bool hasHitPlayer = false;
        for (int i = 0; i < hitColliders.Length; i++)
        {
            Rigidbody hitRb = hitColliders[i].gameObject.GetComponent<Rigidbody>();
            if (hitRb != null)
            {
                hitRb.AddForce(currentAttackDirection * attackForce);
                hasHitPlayer = true;
            }
            PlayerHealth player = hitColliders[i].gameObject.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.Damage();
                hasHitPlayer = true;
            }
            Bed bed = hitColliders[i].gameObject.GetComponent<Bed>();
            if (bed != null)
            {
                bed.TakeDamage(1, gameObject);
                hasHitPlayer = true;
            }
        }
        
        if (hasHitPlayer)
        {
            wolfAnimationScript.AttackHit();
            AM.PlayInChannel("wolf_bite", ChannelType.SFX, 2);
            isMissedAttackStunned = false;
        }
        else
        {
            wolfAnimationScript.AttackMissed();
            isMissedAttackStunned = true;
            trailDrawer.enabled = true;
            timeSinceStunned = 0f;
        }
    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        //Matrix4x4 matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Matrix4x4 matrix = transform.localToWorldMatrix;
        Gizmos.matrix = matrix;
        //Vector3 offset = Quaternion.AngleAxis(rotationalDirection.eulerAngles.y, Vector3.up) * meleeCombo[currentMeleeComboStage].hitboxOffset;
        Gizmos.DrawWireCube(hitboxOffset, hitboxSize);
    }

    public void Damage()
    {
        isDamageStunned = true;
        timeSinceDamaged = 0f;
        isAttacking = false;
        isPreparingAttack = false;
    }

    public void Death()
    {
        isDeathStunned = true;
        isAttacking = false;
        isPreparingAttack = false;
    }

    IEnumerator DisableTrailsAfter(float s)
    {
        yield return new WaitForSeconds(s);
        trailDrawer.enabled = false;
    }
}
