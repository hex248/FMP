using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : MonoBehaviour
{
    Rigidbody rb;
    float timeSinceStart;


    [Header("Movement")]
    [SerializeField] float moveSpeed;
    [SerializeField] float moveRotationSpeed;
    [SerializeField] float aimRotationSpeed;
    [SerializeField] float inRangeMoveSpeed;
    Quaternion rotationalDirection;

    [Header("Attack")]
    [SerializeField] float attackTime;
    [SerializeField] float attackMoveDistance;
    [SerializeField] float attackMoveTime;
    [SerializeField] float damageTime;
    


    bool doneAttackHit;
    float timeSinceAttackEnd;
    float timeSinceLastAttacking;
    Vector3 currentAttackDirection;
    bool isAttacking;

    [SerializeField] float minAttackPrepareTime;
    bool isPreparingAttack;
    float timeSincePrepare;

    [SerializeField] float missedAttackStunTime;
    float timeSinceStunned = 0f;
    bool isStunned; 

    [SerializeField] Vector3 hitboxOffset;
    [SerializeField] Vector3 hitboxSize;
    [SerializeField] LayerMask attackLayerMask;
    [SerializeField] float attackForce;
    [SerializeField] float attackDamage;

    [Header("Visuals")]
    [SerializeField] GameObject wolfVisuals;
    WolfAnimationScript wolfAnimationScript;

    [Header("Behavior")]
    [SerializeField] float maxAttackRange;
    GameObject currentTarget;
    [SerializeField] bool targetPlayer;
    Bed bed;

    [SerializeField] LayerMask raycastLayer;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bed = FindObjectOfType<Bed>();
        wolfAnimationScript = GetComponentInChildren<WolfAnimationScript>();
        currentTarget = bed.gameObject;
        timeSinceLastAttacking = 0f;
    }

    // Update is called once per frame
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


        if (!isMovementLocked())
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
        else if(isPreparingAttack)
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
        else if(isStunned)
        {
            timeSinceStunned += Time.deltaTime;
            if(timeSinceStunned >= missedAttackStunTime)
            {
                isStunned = false;
            }
            MoveForward(0);
        }



        if (isAttacking)
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


        if (targetPlayer)
        {
            targetPlayer = false;
            currentTarget = FindObjectOfType<PlayerController>().gameObject;
        }
    }


    void MoveForward(float speed)
    {
        Vector3 desiredVelocity = wolfVisuals.transform.forward * speed;
        rb.velocity = desiredVelocity;
    }

    void RotateTowardsTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(currentTarget.transform.position - transform.position, Vector3.up);

        rotationalDirection = Quaternion.Lerp(wolfVisuals.transform.rotation, targetRotation, moveRotationSpeed * Time.deltaTime);
        transform.rotation = rotationalDirection;
        //smoke.SetVector3("EmissionDirection", head.forward * -1);
    }

    void AimTowardsTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(currentTarget.transform.position - transform.position, Vector3.up);

        transform.rotation = Quaternion.Lerp(wolfVisuals.transform.rotation, targetRotation, aimRotationSpeed * Time.deltaTime);
        //smoke.SetVector3("EmissionDirection", head.forward * -1);
    }

    bool isMovementLocked()
    {
        return isAttacking || isPreparingAttack || isStunned;
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
                Debug.Log("Apply force to " + hitRb.gameObject);
                hitRb.AddForce(currentAttackDirection * attackForce);
                hasHitPlayer = true;
            }
            EnemyHealth enemy = hitColliders[i].gameObject.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
                hasHitPlayer = true;
            }
            Bed bed = hitColliders[i].gameObject.GetComponent<Bed>();
            if (bed != null)
            {
                //bed.TakeDamage(attackDamage);
                hasHitPlayer = true;
            }
        }

        if (hasHitPlayer)
        {
            wolfAnimationScript.AttackHit();
            isStunned = false;
        }
        else
        {
            wolfAnimationScript.AttackMissed();
            isStunned = true;
            timeSinceStunned = 0f;
        }
    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;

        Gizmos.matrix = transform.localToWorldMatrix;
            //Vector3 offset = Quaternion.AngleAxis(rotationalDirection.eulerAngles.y, Vector3.up) * meleeCombo[currentMeleeComboStage].hitboxOffset;
        Gizmos.DrawWireCube(hitboxOffset, hitboxSize);



    }
}
