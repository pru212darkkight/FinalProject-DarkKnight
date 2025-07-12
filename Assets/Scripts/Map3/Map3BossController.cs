using UnityEngine;
using System.Collections;

public class Map3BossController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public Rigidbody2D rb;
    public EnemyHealth enemyHealth;

    [Header("Attack Area")]
    public Vector2 attackRangeBoxSize = new Vector2(1.2f, 1.5f); // Smaller attack box
    public Vector2 attackRangeBoxOffset = new Vector2(0.8f, 0f); // Closer to boss

    [Header("Movement Settings")]
    public float moveSpeed = 3f;

    [Header("Simple Movement Settings (Merged from SimpleBossMovement)")]
    public bool enableSimpleMovement = true; // Use simple movement logic (DEFAULT: TRUE)
    public float simpleTestSpeed = 3f; // Speed for simple movement
    public bool moveTowardsPlayer = true; // Enable movement towards player
    public float stopDistance = 2f; // Stop distance for simple movement

    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    public float attackDamage = 15f;
    public LayerMask playerLayer = -1;

    [Header("Debug")]
    public bool showDebug = true;
    public bool showGizmos = true;

    // Private variables
    private bool isDead = false;
    private bool isAttacking = false;
    private float lastAttackTime = 0f;
    private bool isFacingRight = true;

    // New state management
    private bool isIdling = false;
    private float idleStartTime = 0f;
    private float idleDuration = 1f;
    private bool wasPlayerInAttackBox = false;

    // Hurt state management
    private bool isHurt = false;
    private float hurtDuration = 0.5f; // Hurt animation duration

    // Animator parameter hashes (matching your existing animator)
    private readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    private readonly int AttackHash = Animator.StringToHash("attack");
    private readonly int Attack2Hash = Animator.StringToHash("attack2");
    private readonly int Attack3Hash = Animator.StringToHash("attack3");
    private readonly int HurtHash = Animator.StringToHash("hurt");
    private readonly int DiedHash = Animator.StringToHash("died");

    void Start()
    {
        // Initialize components
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (enemyHealth == null) enemyHealth = GetComponent<EnemyHealth>();

        // Auto find player
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        // Debug Rigidbody2D settings
        if (showDebug && rb != null)
        {
            Debug.Log($"🔧 Boss Rigidbody2D Settings:");
            Debug.Log($"   - Body Type: {rb.bodyType}");
            Debug.Log($"   - Mass: {rb.mass}");
            Debug.Log($"   - Linear Drag: {rb.linearDamping}");
            Debug.Log($"   - Freeze Position X: {rb.freezeRotation}");
            Debug.Log($"   - Is Kinematic: {rb.isKinematic}");
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        if (enemyHealth != null && enemyHealth.isDead && !isDead)
        {
            Die();
            return;
        }

        Vector2 attackCenter = (Vector2)transform.position + attackRangeBoxOffset;
        bool playerInAttackBox = Physics2D.OverlapBox(
            attackCenter,
            attackRangeBoxSize,
            0,
            playerLayer
        );

        HandleBossState(playerInAttackBox);

        // Debug info
        if (showDebug && Time.frameCount % 60 == 0)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            float timeSinceLastAttack = Time.time - lastAttackTime;
            bool canAttack = Time.time >= lastAttackTime + attackCooldown;
            string state = isHurt ? "Hurt" : isAttacking ? "Attacking" : isIdling ? "Idling" : "Walking";
        }
    }

    void HandleBossState(bool playerInAttackBox)
    {
        if (isHurt || isAttacking)
        {
            animator.SetBool(IsWalkingHash, false);
            return;
        }

        if (playerInAttackBox)
        {
            isIdling = false;
            LookAtPlayer();
            animator.SetBool(IsWalkingHash, false);

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                DoRandomAttack();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            if (wasPlayerInAttackBox && !isIdling)
            {
                StartIdle();
            }
            else if (isIdling)
            {
                if (Time.time >= idleStartTime + idleDuration)
                {
                    EndIdle();
                }
            }
            else
            {
                LookAtPlayer();
                animator.SetBool(IsWalkingHash, true);
            }
        }

        wasPlayerInAttackBox = playerInAttackBox;
    }

    void FixedUpdate()
    {
        // Handle movement in FixedUpdate
        if (isDead || player == null) return;

        // Don't move if EnemyHealth says we're dead
        if (enemyHealth != null && enemyHealth.isDead) return;

        // Debug movement mode
        if (showDebug && Time.frameCount % 120 == 0) // Every 2 seconds
        {
            Debug.Log($"🚶 Boss Movement Mode: {(enableSimpleMovement ? "SIMPLE" : "ADVANCED")}");
        }

        // Choose movement logic based on enableSimpleMovement
        if (enableSimpleMovement)
        {
            HandleSimpleMovement();
        }
        else
        {
            HandleAdvancedMovement();
        }
    }

    /// <summary>
    /// Simple movement logic (merged from SimpleBossMovement)
    /// </summary>
    void HandleSimpleMovement()
    {
        if (!moveTowardsPlayer)
        {
            if (showDebug && Time.frameCount % 120 == 0)
                Debug.Log("🚶 Simple Movement: moveTowardsPlayer is FALSE");
            return;
        }

        // Stop if dead, attacking, or hurt
        if (isDead || isAttacking || isHurt)
        {
            if (showDebug && Time.frameCount % 120 == 0)
                Debug.Log($"🚶 Simple Movement: Stopping - Dead:{isDead}, Attacking:{isAttacking}, Hurt:{isHurt}");
            StopMoving();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        // Stop if too close (let boss attack)
        if (distance <= stopDistance)
        {
            if (showDebug && Time.frameCount % 120 == 0)
                Debug.Log($"🚶 Simple Movement: Too close to player - Distance:{distance:F2}, StopDistance:{stopDistance}");
            StopMoving();
            return;
        }

        // Move towards player
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 velocity = rb.linearVelocity;
        velocity.x = direction.x * simpleTestSpeed;
        rb.linearVelocity = velocity;

        // Debug movement
        if (showDebug && Time.frameCount % 120 == 0)
        {
            Debug.Log($"🚶 Simple Movement: Moving to player - Distance:{distance:F2}, Velocity:{velocity.x:F2}, Speed:{simpleTestSpeed}");
        }

        // Set walking animation
        animator.SetBool(IsWalkingHash, true);

        // Face player (simple flip logic)
        if (direction.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    /// <summary>
    /// Advanced movement logic (original Map3BossController logic)
    /// </summary>
    void HandleAdvancedMovement()
    {
        Vector2 velocity = rb.linearVelocity;
        Vector2 attackCenter = (Vector2)transform.position + attackRangeBoxOffset;
        bool playerInAttackBox = Physics2D.OverlapBox(
            attackCenter,
            attackRangeBoxSize,
            0,
            playerLayer
        );

        if (!playerInAttackBox && !isAttacking && !isIdling && !isHurt)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            velocity.x = direction.x * moveSpeed;
        }
        else
        {
            velocity.x = 0;
        }

        rb.linearVelocity = velocity;
    }

    /// <summary>
    /// Stop movement (merged from SimpleBossMovement)
    /// </summary>
    void StopMoving()
    {
        // Keep Y velocity (for gravity/jumping)
        Vector2 velocity = rb.linearVelocity;
        velocity.x = 0;
        rb.linearVelocity = velocity;
        animator.SetBool(IsWalkingHash, false);
    }

    void LookAtPlayer()
    {
        if (player == null) return;

        if (player.position.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (player.position.x < transform.position.x && isFacingRight)
        {
            Flip();
        }
    }

    void StartIdle()
    {
        isIdling = true;
        idleStartTime = Time.time;
        animator.SetBool(IsWalkingHash, false);

        if (showDebug)
        {
            Debug.Log("Boss: Starting idle state for 1 second");
        }
    }

    void EndIdle()
    {
        isIdling = false;

        if (showDebug)
        {
            Debug.Log("Boss: Ending idle state - will start walking");
        }
    }

    public void DoRandomAttack()
    {
        if (isAttacking) return;

        isAttacking = true;
        animator.SetBool(IsWalkingHash, false);
        rb.linearVelocity = Vector2.zero;

        int attackType = Random.Range(1, 4);

        switch (attackType)
        {
            case 1:
                animator.SetTrigger(AttackHash);
                break;
            case 2:
                animator.SetTrigger(Attack2Hash);
                break;
            case 3:
                animator.SetTrigger(Attack3Hash);
                break;
        }

        StartCoroutine(AutoResetAttack());
        StartCoroutine(DelayedExecuteAttack(attackType));
    }

    private IEnumerator AutoResetAttack()
    {
        yield return new WaitForSeconds(1.5f);
        if (isAttacking)
        {
            Debug.Log("Boss: Auto-resetting attack state (no animation event)");
            OnAttackEnd();
        }
    }

    private IEnumerator DelayedExecuteAttack(int attackChoice)
    {
        yield return new WaitForSeconds(0.5f);

        Debug.Log($"🔥 TEMPORARY: Executing attack {attackChoice} via DelayedExecuteAttack");

        switch (attackChoice)
        {
            case 1:
                ExecuteAttack1();
                break;
            case 2:
                ExecuteAttack2();
                break;
            case 3:
                ExecuteAttack3();
                break;
        }
    }

    public void ResetAttackTrigger(int attackType)
    {
        switch (attackType)
        {
            case 1:
                animator.ResetTrigger(AttackHash);
                break;
            case 2:
                animator.ResetTrigger(Attack2Hash);
                break;
            case 3:
                animator.ResetTrigger(Attack3Hash);
                break;
        }
    }

    // ===== ATTACK IMPLEMENTATIONS (Animation Events) =====

    public void ExecuteAttack1()
    {
        if (showDebug)
            Debug.Log("🔥 Boss: ExecuteAttack1 called!");

        Vector2 attackCenter = (Vector2)transform.position + attackRangeBoxOffset;
        Collider2D playerCollider = Physics2D.OverlapBox(attackCenter, attackRangeBoxSize, 0, playerLayer);

        if (playerCollider != null)
        {
            var playerController = playerCollider.GetComponentInParent<PlayerController1>();
            Debug.Log($"Boss: ExecuteAttack1 hit object: {playerCollider.name}, has PlayerController1: {playerController != null}");
            if (playerController != null)
            {
                playerController.TakeDamage(attackDamage, false);
                Debug.Log($"🩸 Boss: ExecuteAttack1 dealt {attackDamage} damage to player!");
            }
            else
            {
                Debug.LogWarning("Boss: ExecuteAttack1 - Player doesn't have PlayerController1!", playerCollider.gameObject);
            }
        }
        else
        {
            Debug.Log("Boss: ExecuteAttack1 - Player not in attack range, no damage dealt");
        }
    }

    public void ExecuteAttack2()
    {
        if (showDebug)
            Debug.Log("🔥 Boss: ExecuteAttack2 called!");

        Vector2 attackCenter = (Vector2)transform.position + attackRangeBoxOffset;
        Collider2D playerCollider = Physics2D.OverlapBox(attackCenter, attackRangeBoxSize, 0, playerLayer);

        if (playerCollider != null)
        {
            var playerController = playerCollider.GetComponentInParent<PlayerController1>();
            Debug.Log($"Boss: ExecuteAttack2 hit object: {playerCollider.name}, has PlayerController1: {playerController != null}");
            if (playerController != null)
            {
                float damage = attackDamage * 1.2f;
                playerController.TakeDamage(damage, false);
                Debug.Log($"🩸 Boss: ExecuteAttack2 dealt {damage} damage to player!");
            }
            else
            {
                Debug.LogWarning("Boss: ExecuteAttack2 - Player doesn't have PlayerController1!", playerCollider.gameObject);
            }
        }
        else
        {
            Debug.Log("Boss: ExecuteAttack2 - Player not in attack range, no damage dealt");
        }
    }

    public void ExecuteAttack3()
    {
        if (showDebug)
            Debug.Log("🔥 Boss: ExecuteAttack3 called!");

        Vector2 attackCenter = (Vector2)transform.position + attackRangeBoxOffset;
        Collider2D playerCollider = Physics2D.OverlapBox(attackCenter, attackRangeBoxSize, 0, playerLayer);

        if (playerCollider != null)
        {
            var playerController = playerCollider.GetComponentInParent<PlayerController1>();
            Debug.Log($"Boss: ExecuteAttack3 hit object: {playerCollider.name}, has PlayerController1: {playerController != null}");
            if (playerController != null)
            {
                float damage = attackDamage * 1.5f;
                playerController.TakeDamage(damage, false);
                Debug.Log($"🩸 Boss: ExecuteAttack3 dealt {damage} damage to player!");
            }
            else
            {
                Debug.LogWarning("Boss: ExecuteAttack3 - Player doesn't have PlayerController1!", playerCollider.gameObject);
            }
        }
        else
        {
            Debug.Log("Boss: ExecuteAttack3 - Player not in attack range, no damage dealt");
        }
    }

    public void OnAttackEnd()
    {
        if (showDebug)
            Debug.Log("Boss: OnAttackEnd called - resetting attack state");

        isAttacking = false;
        animator.SetBool(IsWalkingHash, false);

        if (player != null)
        {
            Vector2 attackCenter = (Vector2)transform.position + attackRangeBoxOffset;
            bool playerStillInAttackBox = Physics2D.OverlapBox(attackCenter, attackRangeBoxSize, 0, playerLayer);

            if (!playerStillInAttackBox)
            {
                StartIdle();
                if (showDebug)
                    Debug.Log("Boss: Player left attack box after attack - starting idle");
            }
            else
            {
                if (showDebug)
                    Debug.Log("Boss: Player still in attack box - ready for next attack");
            }
        }
    }

    // ===== HEALTH & DAMAGE SYSTEM =====

    public void TakeDamage(float damage)
    {
        if (isDead || isHurt) return;

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }

        OnTakeDamage();
    }

    public void OnTakeDamage()
    {
        if (isDead || isHurt) return;

        if (showDebug)
            Debug.Log("Boss: Taking damage - triggering hurt animation");

        isHurt = true;
        isAttacking = false;
        isIdling = false;

        rb.linearVelocity = Vector2.zero;
        animator.SetBool(IsWalkingHash, false);
        animator.SetTrigger(HurtHash);

        StartCoroutine(RecoverFromHurt());
    }

    private IEnumerator RecoverFromHurt()
    {
        yield return new WaitForSeconds(hurtDuration);

        if (showDebug)
            Debug.Log("Boss: Recovering from hurt state");

        isHurt = false;
    }

    public void OnHurtEnd()
    {
        if (showDebug)
            Debug.Log("Boss: OnHurtEnd called - recovering from hurt state");

        isHurt = false;
    }

    public bool IsCurrentlyAttacking => isAttacking;
    public bool IsCurrentlyHurt => isHurt;
    public bool IsCurrentlyDead => isDead;

    [ContextMenu("Force Execute Attack")]
    public void ForceExecuteAttack()
    {
        Debug.Log("🔥 FORCE EXECUTING ATTACK FOR TEST!");
        ExecuteAttack1();
    }

    /// <summary>
    /// Toggle between simple and advanced movement (merged from SimpleBossMovement)
    /// </summary>
    [ContextMenu("Toggle Movement Mode")]
    public void ToggleMovementMode()
    {
        enableSimpleMovement = !enableSimpleMovement;
        Debug.Log($"Boss Movement Mode: {(enableSimpleMovement ? "Simple" : "Advanced")}");
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        isAttacking = false;

        rb.linearVelocity = Vector2.zero;
        animator.SetBool(IsWalkingHash, false);
        animator.SetTrigger(DiedHash);
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.red;
        Vector3 boxCenter = transform.position + (Vector3)attackRangeBoxOffset;
        Gizmos.DrawWireCube(boxCenter, attackRangeBoxSize);

        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
