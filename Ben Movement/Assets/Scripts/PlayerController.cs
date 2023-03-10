using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    Collider col;
    PlayerManager playerManager;
    

    [Header("Move Settings")]
    [SerializeField] float moveSpeed;
    Vector2 movementInput;

    [Header("Dash Settings")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;
    [SerializeField] LayerMask environmentLayer;
    [SerializeField] float dashCheckRadius = 0.1f;
    [SerializeField] float dashCheckResolution = 0.05f;
    [SerializeField] float playerColliderRadius = 0.25f;

    bool isDashing;
    float yMin;
    float xMax;
    float enableColliderTime;
    Vector3 currentDashDirection;
    List<Vector3> gizmosLocation = new List<Vector3>();
    Vector3 dashEnd;
    float timeSinceDash;

    [Header("Attack Settings")]

    [SerializeField] Attack testAttack;
    //[SerializeField] float attackMoveDistance;
    //[SerializeField] float attackMoveTime;
    //[SerializeField] float postAttackDelay;

    Vector3 currentAttackDirection;
    Vector3 mostRecentMoveDirection;
    bool isAttacking;
    float timeSinceAttack;
    float attackSpeed;

    [Header("Visuals Settings")]
    public GameObject playerVisuals;
    Vector3 lastMovementDirection;
    [SerializeField] [Range(0f, 0.999f)] float squashAmount;
    public int playerNumber = 1;
    PlayerAnimationScript playerAnim;
   

    
    [Header("Movement Lock Settings")]
    public bool movementDashLocked;
    public bool movementMenuLocked;
    public bool movementAttackLocked;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        playerManager = FindObjectOfType<PlayerManager>();
        playerAnim = GetComponentInChildren<PlayerAnimationScript>();
    }

    private void FixedUpdate()
    {
        movementDashLocked = isDashing;
        movementAttackLocked = isAttacking;

        if (!isMovementLocked())
        {
            //normal movement
            Vector3 moveDirection = GetRotatedDirectionFromInput(movementInput);
            DoMovement(moveDirection, moveSpeed);
        }
        else
        {
            Vector3 moveDirection = Vector3.zero;
            DoMovement(moveDirection, moveSpeed);
        }
        if (isDashing)
        {
            DoDashMove();
        }
        if(isAttacking)
        {
            DoAttackMove();
        }
    }

    //Movement Code

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    Vector3 GetRotatedDirectionFromInput(Vector2 inDirection)
    {
        Vector3 inDirection3D = new Vector3(inDirection.x, 0f, inDirection.y);
        Quaternion rotationAngle = Quaternion.Euler(0f, 45f, 0f);
        Matrix4x4 matrix = Matrix4x4.Rotate(rotationAngle);
        Vector3 outDirection = matrix.MultiplyPoint3x4(inDirection3D);
        return outDirection;
    }

    void DoMovement(Vector3 direction, float speed)
    {
        if (direction.magnitude >= 1f)
        {
            direction = direction.normalized;

        }
        if(direction.magnitude >= 0.1f)
        {
            mostRecentMoveDirection = direction;
        }
        
        Vector3 desiredVelocity = new Vector3(direction.x * speed, rb.velocity.y, direction.z * speed);
        rb.velocity = desiredVelocity;


        RotateTowardsDirection(direction);
    }

    void RotateTowardsDirection(Vector3 direction)
    {
        Vector3 walkDirection = new Vector3(direction.x, 0f, direction.z);
        if (walkDirection.magnitude >= 0.1f)
        {
            lastMovementDirection = walkDirection.normalized;
        }
        Quaternion targetRotation = Quaternion.LookRotation(lastMovementDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 20f);
    }

    //Attack Code

    public void OnAttackButtonPressed(InputAction.CallbackContext context)
    {
        bool triggered = context.action.triggered;
        if (triggered)
        {
            if(!isMovementLocked())
            {
                StartAttack();
            }
        }
    }

    void StartAttack()
    {
        timeSinceAttack = 0f;
        isAttacking = true;
        playerAnim.StartAttackAnimation();
        Vector2 attackInputDirection;
        currentAttackDirection = mostRecentMoveDirection;

    }

    void DoAttackMove()
    {
        if (timeSinceAttack >= testAttack.attackTime)
        {
            isAttacking = false;
        }
        else if (timeSinceAttack >= testAttack.moveTime)
        {
            //finished attack movement
        }
        else
        {
            //move player
            attackSpeed = testAttack.moveDistance / testAttack.moveTime;
            DoMovement(currentAttackDirection, attackSpeed);
        }
        timeSinceAttack += Time.deltaTime;

    }


    //Dash Code

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isMovementLocked())
            {
                //check dash direction
                Vector3 dashEndLocation = GetDashEndPoint();
                StartDash(dashEndLocation);

            }
        }
    }

    Vector3 GetDashEndPoint()
    {
        Vector2 dashInputDirection;
        if (movementInput.magnitude > 0.1f)
        {
            dashInputDirection = movementInput.normalized;
        }
        else
        {
            dashInputDirection = Vector2.zero;
        }
        Vector3 dashDirection = GetRotatedDirectionFromInput(dashInputDirection);
        Vector3 totalDashVector = dashDirection * dashSpeed * dashTime;
        bool foundDashLocation = false;
        float checkValue = 1.0f;

        
        while (!foundDashLocation)
        {
            if (checkValue <= 0f)
            {
                foundDashLocation = true;
                checkValue = 0f;

                return transform.position;
            }
            else
            {
                Vector3 directionOffset = totalDashVector * checkValue;
                Vector3 checkOffset = new Vector3(0f, playerColliderRadius + 0.001f, 0f);
                Vector3 playerLocationWithOffset = transform.position + checkOffset;
                Vector3 checkLocation = transform.position + checkOffset + directionOffset;
                Vector3 dashEndLocation = transform.position + directionOffset;

                if (checkValue == 1f)
                {
                    DashEndsInCollider(playerLocationWithOffset, checkLocation, true);
                }
                //check if it is in a collider
                //probably a way to have to avoid doing this check for each location
                //and instead just do this once, and compare the values

                if (!DashEndsInCollider(playerLocationWithOffset, checkLocation))
                {
                    //check if near enough to collider
                    Collider[] collidersAtPoint = Physics.OverlapSphere(checkLocation, playerColliderRadius, environmentLayer);

                    if (collidersAtPoint.Length == 0)
                    {
                        foundDashLocation = true;
                        dashEnd = dashEndLocation;
                        return dashEndLocation;
                    }
                }


                checkValue -= dashCheckResolution;
            }
        }
        return Vector3.zero;
    }

    List<Vector3> GetPossibleDashEndPoints(Vector3 location, Vector3 dashDirection)
    {
        List<Vector3> points = new List<Vector3>();

        bool reachedDestination = false;
        Vector3 raycastStart = new Vector3(location.x, location.y + 0.1f, location.z);
        Vector3 dashOffset = new Vector3(dashDirection.x * dashSpeed * dashTime, 0.0f, dashDirection.z * dashSpeed * dashTime);
        Vector3 endPoint = location + dashOffset + new Vector3(0f, 0.1f, 0f);

        while (!reachedDestination)
        {
            Vector3 raycastDirection = endPoint - raycastStart;
            RaycastHit hit;
            bool hasHit = Physics.Raycast(raycastStart, raycastDirection.normalized, out hit, raycastDirection.magnitude, environmentLayer);

            if (hasHit)
            {
                raycastStart = hit.point + dashOffset.normalized * 0.0001f;
                Vector3 point = hit.point - dashOffset.normalized * 0.0001f;
                points.Add(point);
            }
            else
            {
                reachedDestination = true;
                points.Add(endPoint);
            }
        }
        return points;
    }

    bool DashEndsInCollider(Vector3 location, Vector3 endPoint, bool withDebug = false)
    {
        int hitCount = 0;

        //raycast positive direction
        Vector3 raycastStart = location;
        Vector3 raycastEnd = endPoint;
        bool reachedDestination = false;
        Vector3 dashOffset = raycastEnd - raycastStart;

        if (withDebug)
        {
            gizmosLocation.Clear();
            Debug.DrawLine(raycastStart, raycastEnd, Color.red, 1f);
        }

        while (!reachedDestination)
        {
            Vector3 raycastDirection = raycastEnd - raycastStart;
            RaycastHit hit;
            bool hasHit = Physics.Raycast(raycastStart, raycastDirection.normalized, out hit, raycastDirection.magnitude, environmentLayer);

            if (hasHit)
            {
                hitCount++;
                gizmosLocation.Add(hit.point);
                raycastStart = hit.point + dashOffset.normalized * 0.0001f;

            }
            else
            {
                reachedDestination = true;
            }
        }

        //raycast backwards

        //swap
        raycastStart = endPoint;
        raycastEnd = location;
        reachedDestination = false;
        dashOffset = raycastEnd - raycastStart;

        while (!reachedDestination)
        {
            Vector3 raycastDirection = raycastEnd - raycastStart;
            RaycastHit hit;
            bool hasHit = Physics.Raycast(raycastStart, raycastDirection.normalized, out hit, raycastDirection.magnitude, environmentLayer);


            if (hasHit)
            {
                hitCount--;
                gizmosLocation.Add(hit.point);
                raycastStart = hit.point + dashOffset.normalized * 0.0001f;
            }
            else
            {
                reachedDestination = true;
            }
        }


        return (!(hitCount == 0));
    }

    bool isMovementLocked()
    {
        return (movementDashLocked || playerManager.isPaused || movementAttackLocked);
    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.yellow;
        if (gizmosLocation.Count != 0)
        {
            for (int i = 0; i < gizmosLocation.Count; i++)
            {
                Gizmos.DrawSphere(gizmosLocation[i], 0.1f);
            }
        }
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(dashEnd, 0.25f);

    }

    void StartDash(Vector3 dashEndLocation)
    {
        timeSinceDash = 0f;
        isDashing = true;

        Vector3 offset = new Vector3(dashEndLocation.x, 0f, dashEndLocation.z) - transform.position;
        currentDashDirection = offset.normalized;

        float finishAfterPercent = offset.magnitude / (dashSpeed * dashTime);
        //dumb solution to occasional clipping, if it works it works
        enableColliderTime = finishAfterPercent * dashTime - 0.01f;

        yMin = 1f - squashAmount;
        xMax = Mathf.Sqrt(1f / yMin);
        col.enabled = false;
    }

    void DoDashMove()
    {
        float horizontalScale = 1f;
        float yScale = 1f;
        if (timeSinceDash >= dashTime)
        {
            //finished dash
            isDashing = false;
            yScale = 1f;
            horizontalScale = 1f;
            col.enabled = true;

        }

        else if (timeSinceDash >= enableColliderTime)
        {

            col.enabled = true;
            yScale = Mathf.SmoothStep(yMin, 1f, timeSinceDash / dashTime);
            horizontalScale = Mathf.SmoothStep(xMax, 1f, timeSinceDash / dashTime);
        }

        else
        {
            //do little animation
            yScale = Mathf.SmoothStep(yMin, 1f, timeSinceDash / dashTime);
            horizontalScale = Mathf.SmoothStep(xMax, 1f, timeSinceDash / dashTime);

            //move player
            DoMovement(currentDashDirection, dashSpeed);
        }
        timeSinceDash += Time.deltaTime;
        playerVisuals.transform.localScale = new Vector3(horizontalScale, yScale, horizontalScale);

    }


}