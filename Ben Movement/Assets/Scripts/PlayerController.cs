using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEditor;

public class PlayerController : MonoBehaviour
{
    AudioManager AM;
    Rigidbody rb;
    Collider mainCol;
    List<Collider> fabricCols = new List<Collider>();
    PlayerManager playerManager;
    Player playerParent;

    [Header("Move Settings")]
    [SerializeField] float moveSpeed;
    Vector2 movementInput;
    
    private Vector3 currentForwardDirection;
    private Vector3 mostRecentMoveDirection;
    bool isWalking;
    
    [Header("Dash Settings")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;
    [SerializeField] LayerMask environmentLayer;
    [SerializeField] float dashCheckRadius = 0.1f;
    [SerializeField] float dashCheckResolution = 0.05f;
    [SerializeField] float playerColliderRadius = 0.25f;
    [SerializeField] SnowDust dustVFX;

    bool isDashing;
    float yMin;
    float xMax;
    float enableColliderTime;
    Vector3 currentDashDirection;
    List<Vector3> gizmosLocation = new List<Vector3>();
    Vector3 dashEnd;
    float timeSinceDash;
    bool hasMadeDashSound;

    [Header("Melee Attack Settings")]
    [SerializeField] List<MeleeAttack> meleeCombo = new List<MeleeAttack>();
    
    [SerializeField] LayerMask attackLayerMask;
    bool showMeleeAttackGizmos;
    private bool doneMeleeAttackHit = false;
    Vector3 currentMeleeAttackDirection;
    float lastMeleeAttackingTime;
    bool neededMeleeComboReset;
    int currentMeleeComboStage = 0;
    int nextMeleeComboStage;

    bool isMeleeAttacking;
    [SerializeField] float maxTimeBetweenMeleeAttackForCombo;
    float timeSinceMeleeAttackEnd;
    float attackSpeed;

    [Header("Ranged Attack Settings")]
    //[SerializeField] List<RangedAttack> rangedCombo = new List<RangedAttack>();
    [SerializeField] RangedAttack rangedAttack;
    private bool doneRangedProjectileSpawn = false;
    Vector3 currentRangedAttackDirection;
    float lastRangedAttackingTime;
    bool isRangedAttackCharging;
    float rangedAttackChargingTime;
    float currentRangedAttackChargeFactor;

    //bool neededRangedComboReset;
    //int currentRangedComboStage = 0;
    //int nextRangedComboStage;

    bool isRangedAttacking;
    //[SerializeField] float maxTimeBetweenRangedAttackForCombo;
    float timeSinceRangedAttackEnd;

    [Header("Interact Settings")]
    public bool interacting = false;
    public bool cycleLeftPressed = false;
    public bool cycleRightPressed = false;


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
    public bool movementMeleeAttackLocked;
    public bool movementRangedAttackLocked;

    [Header("Buffering Settings")]
    //[SerializeField] bool dashHasBufferPriority = true;
    bool meleeAttackBuffered;
    bool rangedAttackBuffered;
    bool dashBuffered;

    [Header("Focus Settings")]
    public bool triggerFocus;
    public bool isFocusing;
    [SerializeField] GameObject focusObject;
    [SerializeField] float focusMoveSpeedMultiplier = 0.3f;
    [SerializeField] private float focusViewRange = 10f;
    [SerializeField] GameObject focusReticule;

    [Header("Stun Settings")]
    [SerializeField] float damageStunTime;
    float timeSinceDamaged;
    bool isDamageStunned;

    bool isDead = false;

    private void Start()
    {
        AM = FindObjectOfType<AudioManager>();
        rb = GetComponent<Rigidbody>();
        mainCol = GetComponent<Collider>();
        playerManager = FindObjectOfType<PlayerManager>();
        playerAnim = GetComponentInChildren<PlayerAnimationScript>();
        playerParent = GetComponentInParent<Player>();
        lastMovementDirection = Vector3.forward;
        lastMeleeAttackingTime = 0f;
        fabricCols = GetComponentsInChildren<Collider>().ToList();

        ResetMeleeCombo();

        isDead = false;
    }
    private void FixedUpdate()
    {
        if (isMeleeAttacking)
        {
            lastMeleeAttackingTime = 0f;
            neededMeleeComboReset = true;
        }
        else
        {
            lastMeleeAttackingTime += Time.deltaTime;

            if (lastMeleeAttackingTime >= maxTimeBetweenMeleeAttackForCombo && neededMeleeComboReset == true)
            {
                ResetMeleeCombo();
                neededMeleeComboReset = false;
                playerAnim.ComboTimeout();
            }
        }



        if (isRangedAttacking)
        {
            lastRangedAttackingTime = 0f;
        }
        else
        {
            lastRangedAttackingTime += Time.deltaTime;
        }

        if(isRangedAttackCharging)
        {
            rangedAttackChargingTime += Time.deltaTime;
        }
        else
        {
            rangedAttackChargingTime = 0f;
        }

        if(isDamageStunned)
        {
            timeSinceDamaged += Time.deltaTime;
            if(timeSinceDamaged >= damageStunTime)
            {
                isDamageStunned = false;
            }
        }



        if (isFocusing)
        {
            if (focusObject == null)
            {
                Unfocus();
            }

            else if ((focusObject.transform.position - transform.position).magnitude > focusViewRange)
            {
                Unfocus();
            }
            else
            {
                focusReticule.SetActive(true);
                Vector3 worldPos = focusObject.transform.position + new Vector3(0f, 1.5f, 0f);
                focusReticule.transform.position = worldPos;
            }
        }
        else
        {
            focusReticule.SetActive(false);
        }
        
        movementDashLocked = isDashing;
        movementMeleeAttackLocked = isMeleeAttacking;
        movementRangedAttackLocked = isRangedAttacking || isRangedAttackCharging;



        if (!isMovementLocked() && !isActionBuffered() && isFocusing == false)
        {
            //normal movement
            Vector3 moveDirection = GetRotatedDirectionFromInput(movementInput);
            DoMovement(moveDirection, moveSpeed);

            if (moveDirection.magnitude >= 0.01f)
            {
                playerAnim.isMoving = true;

                if (neededMeleeComboReset)
                {
                    neededMeleeComboReset = false;
                    ResetMeleeCombo();
                }
            }
            else
                playerAnim.isMoving = false;
        }
        else if (!isMovementLocked() && !isActionBuffered() && isFocusing == true)
        {
            Vector3 moveDirection = GetRotatedDirectionFromInput(movementInput);
            DoMovement(moveDirection, moveSpeed * focusMoveSpeedMultiplier);
            RotateTowardsDirection(currentForwardDirection);

            if (moveDirection.magnitude >= 0.01f)
            {
                playerAnim.isMoving = true;

                if (neededMeleeComboReset)
                {
                    neededMeleeComboReset = false;
                    ResetMeleeCombo();
                }
            }
            else
                playerAnim.isMoving = false;
        }
        else if (!isMovementLocked() && isActionBuffered())
        {

            //dash gets priority
            if (dashBuffered)
            {
                Vector3 dashEndLocation = GetDashEndPoint();
                StartDash(dashEndLocation);

                if (neededMeleeComboReset)
                {
                    neededMeleeComboReset = false;
                    ResetMeleeCombo();
                }
            }
            else if (meleeAttackBuffered)
            {
                StartMeleeAttack();
            }
            else if (rangedAttackBuffered)
            {
                StartRangedAttack();
            }
            //allow only 1 buffered action for now - TODO - allow for multiple of same action?
            dashBuffered = false;
            meleeAttackBuffered = false;
            rangedAttackBuffered = false;

            playerAnim.isMoving = false;
        }
        else
        {
            //movement is defined by dash or attack
            Vector3 moveDirection = Vector3.zero;
            DoMovement(moveDirection, moveSpeed);

            playerAnim.isMoving = false;
        }


        if (isDashing)
        {
            DoDashMove();

            if (neededMeleeComboReset)
            {
                neededMeleeComboReset = false;
                ResetMeleeCombo();
            }
        }
        if (isMeleeAttacking)
        {
            DoMeleeAttackMove();
            if(timeSinceMeleeAttackEnd >= meleeCombo[currentMeleeComboStage].damageTime && doneMeleeAttackHit == false)
            {
                DoMeleeAttackHit();
            }
        }
        else if(isRangedAttacking)
        {
            DoRangedAttackMove();
            if (timeSinceRangedAttackEnd >= rangedAttack.projectileSpawnTime && doneRangedProjectileSpawn == false)
            {
                DoRangedProjectileSpawn();
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

    public void Damage()
    {
        timeSinceDamaged = 0f;
        isDamageStunned = true;
    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        if(showMeleeAttackGizmos)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            //Vector3 offset = Quaternion.AngleAxis(rotationalDirection.eulerAngles.y, Vector3.up) * meleeCombo[currentMeleeComboStage].hitboxOffset;
            Gizmos.DrawWireCube(meleeCombo[currentMeleeComboStage].hitboxOffset, meleeCombo[currentMeleeComboStage].hitboxSize);
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

    public void OnInteract(InputAction.CallbackContext context)
    {
        bool triggered = context.action.triggered;

        if (triggered)
        {
            if (!isMovementLocked() && !isActionBuffered())
            {
                interacting = true;
            }
        }
        else
        {
            interacting = false;
        }
    }

    public void OnCycleLeft(InputAction.CallbackContext context)
    {
        bool performed = context.performed;

        if (performed)
        {
            if (!isMovementLocked() && !isActionBuffered())
            {
                cycleLeftPressed = true;
            }
        }
        else
        {
            cycleLeftPressed = false;
        }
    }

    public void OnCycleRight(InputAction.CallbackContext context)
    {
        bool performed = context.performed;

        if (performed)
        {
            if (!isMovementLocked() && !isActionBuffered())
            {
                cycleRightPressed = true;
            }
        }
        else
        {
            cycleRightPressed = false;
        }
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
        //TODO - Smooth out input rather than just setting directly? Might not be what we want though
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

    //Melee Attack Code

    public void OnMeleeAttackButtonPressed(InputAction.CallbackContext context)
    {
        bool triggered = context.action.triggered;
        if (triggered)
        {
            if(!isMovementLocked() && !isActionBuffered())
            {
                StartMeleeAttack();
            }
            else
            {
                meleeAttackBuffered = true;
            }
        }
    }

    void StartMeleeAttack()
    {
        currentMeleeComboStage = nextMeleeComboStage;
        if (currentMeleeComboStage < 2)
        {
            AM.PlayInChannel($"sheep_melee-{currentMeleeComboStage+1}", ChannelType.SFX, 2);
        }
        else if (currentMeleeComboStage == 2)
        {
            AM.PlayInChannel("sheep_combo-end", ChannelType.SFX, 2);
        }

        timeSinceMeleeAttackEnd = 0f;
        isMeleeAttacking = true;
        doneMeleeAttackHit = false;
        if (meleeCombo[currentMeleeComboStage].vfx != null)
        {
            meleeCombo[currentMeleeComboStage].vfx.StartEffect();
        }
        playerAnim.StartMeleeAttackAnimation(currentMeleeComboStage);
        Vector2 attackInputDirection;
        currentMeleeAttackDirection = currentForwardDirection;

        IncrementMeleeCombo();

    }

    void IncrementMeleeCombo()
    {
        nextMeleeComboStage = currentMeleeComboStage + 1;

        if (nextMeleeComboStage >= meleeCombo.Count)
        {
            ResetMeleeCombo();
        }
    }

    void ResetMeleeCombo()
    {
        nextMeleeComboStage = 0;
    }

    void DoMeleeAttackMove()
    {
        if (timeSinceMeleeAttackEnd >= meleeCombo[currentMeleeComboStage].attackTime)
        {
            isMeleeAttacking = false;
            showMeleeAttackGizmos = false;

            //time you were last  attacking
            
        }
        else if (timeSinceMeleeAttackEnd >= meleeCombo[currentMeleeComboStage].moveTime)
        {
            //finished attack movement
        }
        else
        {
            //move player
            attackSpeed = meleeCombo[currentMeleeComboStage].moveDistance / meleeCombo[currentMeleeComboStage].moveTime;
            DoMovement(currentMeleeAttackDirection, attackSpeed);
        }
        timeSinceMeleeAttackEnd += Time.deltaTime;
    }

    void DoMeleeAttackHit()
    {
        showMeleeAttackGizmos = true;
        doneMeleeAttackHit = true;
        Vector3 offset = Quaternion.AngleAxis(rotationalDirection.eulerAngles.y, Vector3.up) * meleeCombo[currentMeleeComboStage].hitboxOffset;
        Collider[] hitColliders = Physics.OverlapBox(transform.position + offset, (meleeCombo[currentMeleeComboStage].hitboxSize / 2f), rotationalDirection, attackLayerMask);
        for(int i = 0; i < hitColliders.Length; i++)
        {
            Rigidbody hitRb = hitColliders[i].gameObject.GetComponent<Rigidbody>();
            if (hitRb != null)
            {
                hitRb.AddForce(currentMeleeAttackDirection * meleeCombo[currentMeleeComboStage].force);
            }
            EnemyHealth enemy = hitColliders[i].gameObject.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(meleeCombo[currentMeleeComboStage].damage, playerParent.gameObject);
            }
        }
    }

    //Ranged Attack Code

    public void OnRangedAttackButtonPressed(InputAction.CallbackContext context)
    {
        bool triggered = context.action.triggered;
        if (triggered)
        {
            if (!isMovementLocked() && !isActionBuffered())
            {
                //start charging
                rangedAttackChargingTime = 0f;
                isRangedAttackCharging = true;
            }
            else
            {
                //ranged attack probably doesn't need bufferinging but mabye re-add later

                //rangedAttackBuffered = true;
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            if (isRangedAttackCharging)
            {
                //trigger attack on button release
                isRangedAttackCharging = false;
                StartRangedAttack();
            }
        }
    }

    void StartRangedAttack()
    {
        AM.PlayInChannel("sheep_shoot", ChannelType.SFX, 2);
        currentRangedAttackChargeFactor = Mathf.Clamp(rangedAttackChargingTime / rangedAttack.fullChargeTime, 0f, 1f);

        timeSinceRangedAttackEnd = 0f;
        isRangedAttacking = true;
        doneRangedProjectileSpawn = false;
        playerAnim.StartRangedAttackAnimation();
        Vector2 attackInputDirection;
        currentRangedAttackDirection = currentForwardDirection;

    }

    void DoRangedAttackMove()
    {
        if (timeSinceRangedAttackEnd >= rangedAttack.attackTime)
        {
            isRangedAttacking = false;

            //time you were last  attacking
            
        }
        else if (timeSinceRangedAttackEnd >= rangedAttack.moveTime)
        {
            //finished attack movement
        }
        else
        {
            //move player
            attackSpeed = rangedAttack.moveDistance / rangedAttack.moveTime;
            DoMovement(currentRangedAttackDirection, attackSpeed);
        }
        timeSinceRangedAttackEnd += Time.deltaTime;
    }

    void DoRangedProjectileSpawn()
    {
        doneRangedProjectileSpawn = true;
        //Vector3 spawnOffset = Quaternion.AngleAxis(rotationalDirection.eulerAngles.y, Vector3.up) * rangedCombo[currentRangedComboStage].projectileSpawnOffset;
        GameObject newProjectile = Instantiate(rangedAttack.projectile);
        newProjectile.transform.position = rangedAttack.projectileSpawnPosition.position;
        newProjectile.transform.forward = currentRangedAttackDirection;
        Projectile projectileScript = newProjectile.GetComponent<Projectile>();
        if(projectileScript != null)
        {
            if(currentRangedAttackChargeFactor != 1f)
            {
                projectileScript.damage = rangedAttack.baseDamage * Mathf.Lerp(1f, rangedAttack.fullChargeDamageMultiplier, currentRangedAttackChargeFactor);
                projectileScript.force = rangedAttack.baseForce * Mathf.Lerp(1f, rangedAttack.fullChargeForceMultiplier, currentRangedAttackChargeFactor);
                projectileScript.scaleMultiplier = Mathf.Lerp(1f, rangedAttack.fullChargeScaleFactor, currentRangedAttackChargeFactor);
                projectileScript.projectileSpeed *= Mathf.Lerp(1f, rangedAttack.fullChargeSpeedMultiplier, currentRangedAttackChargeFactor);
            }
            else
            {
                //mabye add bonus?
                projectileScript.damage = rangedAttack.baseDamage * Mathf.Lerp(1f, rangedAttack.fullChargeDamageMultiplier, currentRangedAttackChargeFactor);
                projectileScript.force = rangedAttack.baseForce * Mathf.Lerp(1f, rangedAttack.fullChargeForceMultiplier, currentRangedAttackChargeFactor);
                projectileScript.scaleMultiplier = Mathf.Lerp(1f, rangedAttack.fullChargeScaleFactor, currentRangedAttackChargeFactor);
                projectileScript.projectileSpeed *= Mathf.Lerp(1f, rangedAttack.fullChargeSpeedMultiplier, currentRangedAttackChargeFactor);
            }

            projectileScript.owner = playerParent;
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
                //check if it is in a mainCollider
                //probably a way to have to avoid doing this check for each location
                //and instead just do this once, and compare the values

                if (!DashEndsInCollider(playerLocationWithOffset, checkLocation))
                {
                    
                    //check if near enough to mainCollider
                    Collider[] mainCollidersAtPoint = Physics.OverlapSphere(checkLocation, playerColliderRadius, environmentLayer, QueryTriggerInteraction.Ignore);
                    gizmosLocation.Add(checkLocation);
                    if (mainCollidersAtPoint.Length == 0)
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
        playerAnim.StartDashAnimation();
        timeSinceDash = 0f;
        isDashing = true;

        Vector3 offset = new Vector3(dashEndLocation.x, 0f, dashEndLocation.z) - transform.position;
        currentDashDirection = offset.normalized;

        dustVFX.OnDash();

        float finishAfterPercent = offset.magnitude / (dashSpeed * dashTime);
        //dumb solution to occasional clipping, if it works it works
        enableColliderTime = finishAfterPercent * dashTime - 0.01f;

        yMin = 1f - squashAmount;
        xMax = Mathf.Sqrt(1f / yMin);
        mainCol.enabled = false;
        SetClothCollidersActive(false);
    }

    void SetClothCollidersActive(bool on)
    {
        foreach(Collider collider in fabricCols)
        {
            collider.enabled = on;
        }
    }

    void DoDashMove()
    {
        float horizontalScale = 1f;
        float yScale = 1f;
        if (timeSinceDash >= dashTime * 0.5f && !hasMadeDashSound)
        {
            hasMadeDashSound = true;
            AM.PlayInChannel("sheep_dash", ChannelType.SFX, 2);
        }
        if (timeSinceDash >= dashTime)
        {
            //finished dash
            isDashing = false;
            hasMadeDashSound = false;
            playerAnim.EndDashAnimation();
            dustVFX.OnDashEnd();
            yScale = 1f;
            horizontalScale = 1f;
            mainCol.enabled = true;
            SetClothCollidersActive(true);

        }

        else if (timeSinceDash >= enableColliderTime)
        {
            mainCol.enabled = true;
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
        return (movementDashLocked || playerManager.isPaused || movementMeleeAttackLocked || movementRangedAttackLocked || isDamageStunned || isDead);
    }

    bool isActionBuffered()
    {
        return (meleeAttackBuffered || dashBuffered || rangedAttackBuffered);
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
                if(!isDead)
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
        List<EnemyHealth> enemiesInDirectView = new List<EnemyHealth>();
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
                //check if any are right in front
                foreach (EnemyHealth possibleEnemy in enemiesInViewCone)
                {
                    Vector3 offset = possibleEnemy.transform.position - transform.position;
                    Vector3 direction = offset.normalized;
                    if (Vector3.Dot(direction, transform.forward) > 0.7f)
                    {

                        enemiesInDirectView.Add(possibleEnemy);
                    }
                }

                if (enemiesInDirectView != null && enemiesInDirectView.Count != 0)
                {
                    float currentMinDistance = Mathf.Infinity;
                    int currentMinIndex = 0;
                    int i = 0;

                    foreach (EnemyHealth possibleEnemy in enemiesInDirectView)
                    {
                        Vector3 offset = possibleEnemy.transform.position - transform.position;
                        if (offset.magnitude < currentMinDistance)
                        {
                            currentMinDistance = offset.magnitude;
                            currentMinIndex = i;

                        }

                        i++;
                    }
                    return enemiesInDirectView[currentMinIndex].gameObject;
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
    }

    void Unfocus()
    {
        isFocusing = false;
        playerParent.cameraFollow.ToPlayer();
    }

    public void Die()
    {
        isDead = true;
        Unfocus();
    }

    public void Revive()
    {
        isDead = false;
        Unfocus();
    }    

    public bool IsDead()
    {
        return isDead;
    }
}