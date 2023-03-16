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
    Player playerParent;

    [Header("Move Settings")]
    [SerializeField] float moveSpeed;
    Vector2 movementInput;
    
    private Vector3 currentForwardDirection;
    private Vector3 mostRecentMoveDirection;
    
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
    [SerializeField] List<Attack> basicCombo = new List<Attack>();
    int currentComboStage = 0;
    [SerializeField] LayerMask attackLayerMask;
    bool showAttackGizmos;
    private bool doneAttackHit = false;
    Vector3 currentAttackDirection;
    
    bool isAttacking;
    float timeSinceAttack;
    float attackSpeed;

    [Header("Visuals Settings")]
    public GameObject playerVisuals;
    Vector3 lastMovementDirection;
    [SerializeField] [Range(0f, 0.99f)] float squashAmount;
    public int playerNumber = 1;
    PlayerAnimationScript playerAnim;
    Quaternion rotationalDirection;
   

    
    [Header("Movement Lock Settings")]
    public bool movementDashLocked;
    public bool movementMenuLocked;
    public bool movementAttackLocked;

    [Header("Buffering Settings")]
    //[SerializeField] bool dashHasBufferPriority = true;
    bool attackBuffered;
    bool dashBuffered;

    [Header("Focus Settings")]
    public bool triggerFocus;
    public bool isFocusing;
    [SerializeField] GameObject focusObject;
    [SerializeField] float focusMoveSpeedMultiplier = 0.3f;
    [SerializeField] private float focusViewRange = 10f;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        playerManager = FindObjectOfType<PlayerManager>();
        playerAnim = GetComponentInChildren<PlayerAnimationScript>();
        playerParent = GetComponentInParent<Player>();
        lastMovementDirection = Vector3.forward;
    }

    private void FixedUpdate()
    {
        Debug.Log("input is " + movementInput);
        if (isFocusing)
        {
            if (focusObject == null)
            {
                Unfocus();
            }
        }
        
        movementDashLocked = isDashing;
        movementAttackLocked = isAttacking;

        if (!isMovementLocked() && !isActionBuffered() && isFocusing == false)
        {
            //normal movement
            Vector3 moveDirection = GetRotatedDirectionFromInput(movementInput);
            DoMovement(moveDirection, moveSpeed);
        }
        else if(!isMovementLocked() && !isActionBuffered() && isFocusing == true)
        {
            Vector3 moveDirection = GetRotatedDirectionFromInput(movementInput);
            DoMovement(moveDirection, moveSpeed * focusMoveSpeedMultiplier);
            RotateTowardsDirection(currentForwardDirection);
        }
        else if(!isMovementLocked() && isActionBuffered())
        {

            //dash gets priority
            if (dashBuffered)
            {
                Vector3 dashEndLocation = GetDashEndPoint();
                StartDash(dashEndLocation);
                Debug.Log("buffered dash to " + (dashEndLocation - transform.position));
            }
            else if (attackBuffered)
            {
                StartAttack();
            }
            //allow only 1 buffered action for now - TODO - allow for multiple of same action?
            dashBuffered = false;
            attackBuffered = false;
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
        if (isAttacking)
        {
            DoAttackMove();
            if(timeSinceAttack >= basicCombo[currentComboStage].damageTime && doneAttackHit == false)
            {
                DoAttackHit();
            }
        }
        else
        {
            if (isFocusing)
            {
                RotateTowardsDirection(currentForwardDirection);
            }
            else
            {
                if (!isMovementLocked())
                {
                    RotateTowardsDirection(currentForwardDirection);
                }
            }
        }
        
    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        if(showAttackGizmos)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            //Vector3 offset = Quaternion.AngleAxis(rotationalDirection.eulerAngles.y, Vector3.up) * basicCombo[currentComboStage].hitboxOffset;
            Gizmos.DrawWireCube(basicCombo[currentComboStage].hitboxOffset, basicCombo[currentComboStage].hitboxSize);
        }


        Gizmos.color = Color.yellow;
        if (gizmosLocation.Count != 0)
        {
            for (int i = 0; i < gizmosLocation.Count; i++)
            {
                Gizmos.DrawSphere(gizmosLocation[i], 0.3f);
            }
        }
        Gizmos.color = Color.red;



    }

    //Movement Code

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    Vector3 GetRotatedDirectionFromInput(Vector2 inDirection)
    {
        Vector3 inDirection3D = new Vector3(inDirection.x, 0f, inDirection.y);
        Vector3 outDirection = Vector3.zero;
        
        if (!isFocusing || focusObject == null)
        {
            Quaternion rotationAngle = Quaternion.Euler(0f, 45f, 0f);
            Matrix4x4 matrix = Matrix4x4.Rotate(rotationAngle);
            outDirection = matrix.MultiplyPoint3x4(inDirection3D);

        }
        else
        {
            //Vector3 focusPoint = focusObject.transform.position; // replace zero with focus point. currently focusses around origin
            //Vector3 focusPointXZ = new Vector3(focusPoint.x, 0f, focusPoint.z);
            //Vector3 positionXZ = new Vector3(transform.position.x, 0f, transform.position.z);

            //Vector3 focusPointDirection = (focusPointXZ - positionXZ).normalized;
            //Vector3 standardForward = new Vector3(0f, 0f, 1f);
            //Quaternion rotationAngle = Quaternion.FromToRotation(standardForward, focusPointDirection);
            Quaternion rotationAngle = Quaternion.Euler(0f, 45f, 0f);

            Matrix4x4 matrix = Matrix4x4.Rotate(rotationAngle);
            outDirection = matrix.MultiplyPoint3x4(inDirection3D);
        }

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

        if (isFocusing)
        {
            Vector3 focusPoint = focusObject.transform.position; // replace zero with focus point. currently focusses around origin
            Vector3 focusPointXZ = new Vector3(focusPoint.x, 0f, focusPoint.z);
            Vector3 positionXZ = new Vector3(transform.position.x, 0f, transform.position.z);

            currentForwardDirection = (focusPointXZ - positionXZ).normalized;
        }
        else
        {
            currentForwardDirection = mostRecentMoveDirection;
        }


    }

    void RotateTowardsDirection(Vector3 direction)
    {
        Vector3 walkDirection = new Vector3(direction.x, 0f, direction.z);
        if (walkDirection.magnitude >= 0.1f)
        {
            lastMovementDirection = walkDirection.normalized;
        }
        Quaternion targetRotation = Quaternion.LookRotation(lastMovementDirection);
        rotationalDirection = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 20f);
        transform.rotation = rotationalDirection;
    }

    //Attack Code

    public void OnAttackButtonPressed(InputAction.CallbackContext context)
    {
        bool triggered = context.action.triggered;
        if (triggered)
        {
            if(!isMovementLocked() && !isActionBuffered())
            {
                StartAttack();
            }
            else
            {
                attackBuffered = true;
            }
        }
    }

    void StartAttack()
    {
        timeSinceAttack = 0f;
        isAttacking = true;
        doneAttackHit = false;
        playerAnim.StartAttackAnimation();
        Vector2 attackInputDirection;
        currentAttackDirection = currentForwardDirection;

    }

    void DoAttackMove()
    {
        if (timeSinceAttack >= basicCombo[currentComboStage].attackTime)
        {
            isAttacking = false;
            showAttackGizmos = false;
        }
        else if (timeSinceAttack >= basicCombo[currentComboStage].moveTime)
        {
            //finished attack movement
        }
        else
        {
            //move player
            attackSpeed = basicCombo[currentComboStage].moveDistance / basicCombo[currentComboStage].moveTime;
            DoMovement(currentAttackDirection, attackSpeed);
        }
        timeSinceAttack += Time.deltaTime;
    }

    void DoAttackHit()
    {
        showAttackGizmos = true;
        doneAttackHit = true;
        Vector3 offset = Quaternion.AngleAxis(rotationalDirection.eulerAngles.y, Vector3.up) * basicCombo[currentComboStage].hitboxOffset;
        Collider[] hitColliders = Physics.OverlapBox(transform.position + offset, (basicCombo[currentComboStage].hitboxSize / 2f), rotationalDirection, attackLayerMask);
        for(int i = 0; i < hitColliders.Length; i++)
        {
            Rigidbody hitRb = hitColliders[i].gameObject.GetComponent<Rigidbody>();
            if (hitRb != null)
            {
                hitRb.AddForce(currentAttackDirection * basicCombo[currentComboStage].force);
            }
            EnemyHealth enemy = hitColliders[i].gameObject.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(basicCombo[currentComboStage].damage);
            }
        }
    }


    //Dash Code

    public void OnDashButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isMovementLocked() && !isActionBuffered())
            {
                //check dash direction
                Vector3 dashEndLocation = GetDashEndPoint();
                StartDash(dashEndLocation);

            }
            else
            {
                dashBuffered = true;
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
        //Vector3 dashDirection = mostRecentMoveDirection;
        Vector3 totalDashVector = dashDirection * (dashSpeed * dashTime);
        bool foundDashLocation = false;
        float checkValue = 1.0f;

        gizmosLocation.Clear();
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
                Vector3 checkOffset = new Vector3(0f, playerColliderRadius + 0.01f, 0f);
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
                    gizmosLocation.Add(checkLocation);
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
            //gizmosLocation.Clear();
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
                //gizmosLocation.Add(hit.point);
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
                //gizmosLocation.Add(hit.point);
                raycastStart = hit.point + dashOffset.normalized * 0.0001f;
            }
            else
            {
                reachedDestination = true;
            }
        }
        return (!(hitCount == 0));
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

    bool isMovementLocked()
    {
        return (movementDashLocked || playerManager.isPaused || movementAttackLocked);
    }

    bool isActionBuffered()
    {
        return (attackBuffered || dashBuffered);
    }

    //Focus Code

    public void OnFocusButtonPressed(InputAction.CallbackContext context)
    {
        bool triggered = context.action.triggered;
        if (triggered)
        {
            if(isFocusing)
            {
                Unfocus();
            }
            else
            {
                Focus();
            }
        }
    }

    void Focus()
    {
        GameObject newFocusObject = SelectFocusTarget();
        if (newFocusObject != null)
        {
            isFocusing = true;
            focusObject = newFocusObject;
            playerParent.cameraFollow.Focus(focusObject.transform);
        }
        else
        {
            Unfocus();
        }
    }

    GameObject SelectFocusTarget()
    {
        EnemyHealth[] possibleEnemies = FindObjectsOfType<EnemyHealth>();
        List<EnemyHealth> enemiesInRange = new List<EnemyHealth>();
        List<EnemyHealth> enemiesInViewCone = new List<EnemyHealth>();
        if (FindObjectOfType<EnemyHealth>() != null)
        {
            //filter out ones not in range
            foreach (EnemyHealth possibleEnemy in possibleEnemies)
            {
                Vector3 offset = possibleEnemy.transform.position - transform.position;
                if (offset.magnitude < focusViewRange)
                {
                    enemiesInRange.Add(possibleEnemy);
                }
            }

        }

        if (enemiesInRange == null || enemiesInRange.Count == 0)
        {
            //no nearby enemies, don't focus
            return null;
        }
        else
        {
            //prioritise ones in front of player
            foreach (EnemyHealth possibleEnemy in enemiesInRange)
            {   
                Vector3 offset = possibleEnemy.transform.position - transform.position;
                Vector3 direction = offset.normalized;
                if (Vector3.Dot(direction, transform.forward) > 0.1f)
                {
                    
                    enemiesInViewCone.Add(possibleEnemy);
                }
            }

            if (enemiesInViewCone == null || enemiesInViewCone.Count == 0)
            {
                //just focus on nearest enemy
                
                float currentMinDistance = Mathf.Infinity;
                int currentMinIndex = 0;
                int i = 0;
                
                foreach (EnemyHealth possibleEnemy in enemiesInRange)
                {   
                    Vector3 offset = possibleEnemy.transform.position - transform.position;
                    if (offset.magnitude < currentMinDistance)
                    {
                        currentMinDistance = offset.magnitude;
                        currentMinIndex = i;

                    }

                    i++;
                }

                return enemiesInRange[currentMinIndex].gameObject;

            }
            else
            {
                //focus on nearest enemy in view cone
                
                float currentMinDistance = Mathf.Infinity;
                int currentMinIndex = 0;
                int i = 0;
                
                foreach (EnemyHealth possibleEnemy in enemiesInViewCone)
                {   
                    Vector3 offset = possibleEnemy.transform.position - transform.position;
                    if (offset.magnitude < currentMinDistance)
                    {
                        currentMinDistance = offset.magnitude;
                        currentMinIndex = i;

                    }

                    i++;
                }

                return enemiesInViewCone[currentMinIndex].gameObject;
            }
        }
    }

    void Unfocus()
    {
        isFocusing = false;
        playerParent.cameraFollow.ToPlayer();
    }
}