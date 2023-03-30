using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : MonoBehaviour
{
    Rigidbody rb;
    float timeSinceStart;
    
    
    [Header("Controller")]
    [SerializeField] float moveSpeed;
    [SerializeField] float moveRotationSpeed;
    [SerializeField] float aimRotationSpeed;
    [SerializeField] float attackDistance;

    bool isAttacking;

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
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 offsetToTarget = currentTarget.transform.position - transform.position;
        Vector3 directionToTarget = offsetToTarget.normalized;
        float distanceToTarget = offsetToTarget.magnitude;
        bool targetIsInDirectView = false;

        RaycastHit hit;
        Physics.Raycast(transform.position, directionToTarget, out hit, Mathf.Infinity, raycastLayer);
        if(hit.collider != null)
        {
            targetIsInDirectView = (hit.collider.gameObject == currentTarget);
        }
        float minDistanceToTargetCollider = (hit.point - transform.position).magnitude;
        Debug.Log("distance to collider is " + minDistanceToTargetCollider);
        //decide behaviour
        if (minDistanceToTargetCollider <= maxAttackRange)
        {
            //in range

            if(Vector3.Dot(transform.forward, directionToTarget) >= 0.5f)
            {
                //facing right direction
                if(!isAttacking)
                    Attack();
            }
            else
            {
                //need to aim
                AimTowardsTarget();
            }
        }
        else
        {
            RotateTowardsTarget();
            MoveForward(moveSpeed);
        }

        
        

        if (targetPlayer)
        {
            targetPlayer = false;
            currentTarget = FindObjectOfType<PlayerController>().gameObject;
        }
    }


    void MoveForward(float speed)
    {
        Vector3 desiredVelocity = wolfVisuals.transform.forward * moveSpeed;
        rb.velocity = desiredVelocity;
    }

    void RotateTowardsTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(currentTarget.transform.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Lerp(wolfVisuals.transform.rotation, targetRotation, moveRotationSpeed * Time.deltaTime);
        //smoke.SetVector3("EmissionDirection", head.forward * -1);
    }

    void AimTowardsTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(currentTarget.transform.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Lerp(wolfVisuals.transform.rotation, targetRotation, aimRotationSpeed * Time.deltaTime);
        //smoke.SetVector3("EmissionDirection", head.forward * -1);
    }

    void Attack()
    {
        isAttacking = true;
        wolfAnimationScript.StartAttackAnimation();
    }
}
