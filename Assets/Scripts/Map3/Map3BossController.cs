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
            else
            {

            }
        }


    }
    
    void Update()
    {
        // Check if boss is dead
        if (isDead || player == null) return;

        // Check EnemyHealth component death
        if (enemyHealth != null && enemyHealth.isDead && !isDead)
        {
            Die();
            return;
        }

        // Check if player is in attack range box
        Vector2 attackCenter = (Vector2)transform.position + attackRangeBoxOffset;
        bool playerInAttackBox = Physics2D.OverlapBox(
            attackCenter,
            attackRangeBoxSize,
            0,
            playerLayer
        );

        // Handle state transitions
        HandleBossState(playerInAttackBox);

        // Debug info
        if (showDebug && Time.frameCount % 60 == 0)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            float timeSinceLastAttack = Time.time - lastAttackTime;
            bool canAttack = Time.time >= lastAttackTime + attackCooldown;
            string state = isHurt ? "Hurt" : isAttacking ? "Attacking" : isIdling ? "Idling" : "Walking";
            Debug.Log($"Boss - State: {state}, InAttackBox: {playerInAttackBox}, Distance: {distanceToPlayer:F2}, TimeSinceAttack: {timeSinceLastAttack:F1}s, CanAttack: {canAttack}");
        }
    }

    /// <summary>
    /// Handle boss behavior based on player position
    /// </summary>
    void HandleBossState(bool playerInAttackBox)
    {
        // If currently hurt or attacking, don't change state
        if (isHurt || isAttacking)
        {
            animator.SetBool(IsWalkingHash, false);
            return;
        }

        // Player is in attack range
        if (playerInAttackBox)
        {
            // Stop any idle state
            isIdling = false;

            // Face player and stop moving
            LookAtPlayer();
            animator.SetBool(IsWalkingHash, false);

            // Attack if cooldown is ready
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                DoRandomAttack();
                lastAttackTime = Time.time;
            }
        }
        // Player is NOT in attack range
        else
        {
            // Check if we just transitioned from "player in box" to "player out of box"
            if (wasPlayerInAttackBox && !isIdling)
            {
                // Start idle state
                StartIdle();
            }
            // If we're currently idling
            else if (isIdling)
            {
                // Check if idle time is over
                if (Time.time >= idleStartTime + idleDuration)
                {
                    // End idle, start walking
                    EndIdle();
                }
            }
            // If not idling, walk towards player
            else
            {
                LookAtPlayer();
                animator.SetBool(IsWalkingHash, true);
            }
        }

        // Remember previous state
        wasPlayerInAttackBox = playerInAttackBox;
    }

    void FixedUpdate()
    {
        // Check if SimpleBossMovement is handling movement
        SimpleBossMovement simpleMovement = GetComponent<SimpleBossMovement>();
        if (simpleMovement != null && simpleMovement.enableSimpleMovement)
        {
            // Let SimpleBossMovement handle movement, we only handle attacks
            return;
        }

        // Handle movement in FixedUpdate like PlayerController1
        if (isDead || player == null) return;

        // Don't move if EnemyHealth says we're dead
        if (enemyHealth != null && enemyHealth.isDead) return;

        Vector2 velocity = rb.linearVelocity;

        // Check if should move towards player
        Vector2 attackCenter = (Vector2)transform.position + attackRangeBoxOffset;
        bool playerInAttackBox = Physics2D.OverlapBox(
            attackCenter,
            attackRangeBoxSize,
            0,
            playerLayer
        );

        if (!playerInAttackBox && !isAttacking && !isIdling && !isHurt)
        {
            // Move towards player (only if not attacking, idling, or hurt)
            Vector2 direction = (player.position - transform.position).normalized;
            velocity.x = direction.x * moveSpeed;
        }
        else
        {
            // Stop horizontal movement (attacking, in attack box, idling, or hurt)
            velocity.x = 0;
        }

        rb.linearVelocity = velocity;
    }
    
    void LookAtPlayer()
    {
        if (player == null) return;

        // Face the player
        if (player.position.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (player.position.x < transform.position.x && isFacingRight)
        {
            Flip();
        }
    }

    /// <summary>
    /// Start idle state when player leaves attack range
    /// </summary>
    void StartIdle()
    {
        isIdling = true;
        idleStartTime = Time.time;

        // Stop movement and walking animation
        animator.SetBool(IsWalkingHash, false);

        if (showDebug)
        {
            Debug.Log("Boss: Starting idle state for 1 second");
        }
    }

    /// <summary>
    /// End idle state and prepare to walk towards player
    /// </summary>
    void EndIdle()
    {
        isIdling = false;

        if (showDebug)
        {
            Debug.Log("Boss: Ending idle state - will start walking");
        }
    }

    /// <summary>
    /// Choose and execute random attack (like FinalBossAttack.DoAttack)
    /// </summary>
    public void DoRandomAttack()
    {
        if (isAttacking) return;

        isAttacking = true;

        // Stop walking animation when attacking
        animator.SetBool(IsWalkingHash, false);
        rb.linearVelocity = Vector2.zero;

        // Choose random attack type (1-3)
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

        // Auto reset after attack duration if no animation event
        StartCoroutine(AutoResetAttack());

        // TEMPORARY FIX: Execute attack immediately if no Animation Events
        // Remove this after setting up Animation Events properly
        StartCoroutine(DelayedExecuteAttack(attackType));
    }

    /// <summary>
    /// Auto reset attack state if animation event doesn't call OnAttackEnd
    /// </summary>
    private System.Collections.IEnumerator AutoResetAttack()
    {
        yield return new WaitForSeconds(1.5f); // Wait for attack animation to finish

        if (isAttacking) // Only reset if still attacking (animation event didn't call OnAttackEnd)
        {
            Debug.Log("Boss: Auto-resetting attack state (no animation event)");
            OnAttackEnd();
        }
    }

    /// <summary>
    /// TEMPORARY: Execute attack after delay to simulate Animation Event
    /// </summary>
    private System.Collections.IEnumerator DelayedExecuteAttack(int attackChoice)
    {
        // Wait for animation to reach damage frame (usually middle of animation)
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

    /// <summary>
    /// Reset attack trigger after animation ends (called by Animation Event)
    /// </summary>
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
    
    // ===== ATTACK IMPLEMENTATIONS (called by Animation Events) =====

    /// <summary>
    /// Attack 1 - Basic melee attack (called by Animation Event)
    /// </summary>
    public void ExecuteAttack1()
    {
        if (showDebug)
        {
            Debug.Log("🔥 Boss: ExecuteAttack1 called!");
        }

        if (player == null)
        {
            Debug.LogWarning("Boss: ExecuteAttack1 - Player is null!");
            return;
        }

        // Check if player is in attack box
        Vector2 attackCenter = (Vector2)transform.position + attackRangeBoxOffset;
        bool playerHit = Physics2D.OverlapBox(attackCenter, attackRangeBoxSize, 0, playerLayer);

        if (showDebug)
        {
            Debug.Log($"Boss: ExecuteAttack1 - Player in attack box: {playerHit}");
            Debug.Log($"Boss: Attack center: {attackCenter}, Box size: {attackRangeBoxSize}");
        }

        if (playerHit)
        {
            PlayerController1 playerController = player.GetComponent<PlayerController1>();
            if (playerController != null)
            {
                playerController.TakeDamage(attackDamage, false);
                Debug.Log($"🩸 Boss: ExecuteAttack1 dealt {attackDamage} damage to player!");
            }
            else
            {
                Debug.LogWarning("Boss: ExecuteAttack1 - Player doesn't have PlayerController1!");
            }
        }
        else
        {
            Debug.Log("Boss: ExecuteAttack1 - Player not in attack range, no damage dealt");
        }
    }

    /// <summary>
    /// Attack 2 - Enhanced melee attack (called by Animation Event)
    /// </summary>
    public void ExecuteAttack2()
    {
        if (showDebug)
        {
            Debug.Log("🔥 Boss: ExecuteAttack2 called!");
        }

        if (player == null)
        {
            Debug.LogWarning("Boss: ExecuteAttack2 - Player is null!");
            return;
        }

        // Check if player is in attack box
        Vector2 attackCenter = (Vector2)transform.position + attackRangeBoxOffset;
        bool playerHit = Physics2D.OverlapBox(attackCenter, attackRangeBoxSize, 0, playerLayer);

        if (showDebug)
        {
            Debug.Log($"Boss: ExecuteAttack2 - Player in attack box: {playerHit}");
        }

        if (playerHit)
        {
            PlayerController1 playerController = player.GetComponent<PlayerController1>();
            if (playerController != null)
            {
                float damage = attackDamage * 1.2f; // 20% more damage
                playerController.TakeDamage(damage, false);
                Debug.Log($"🩸 Boss: ExecuteAttack2 dealt {damage} damage to player!");
            }
            else
            {
                Debug.LogWarning("Boss: ExecuteAttack2 - Player doesn't have PlayerController1!");
            }
        }
        else
        {
            Debug.Log("Boss: ExecuteAttack2 - Player not in attack range, no damage dealt");
        }
    }

    /// <summary>
    /// Attack 3 - Heavy attack (called by Animation Event)
    /// </summary>
    public void ExecuteAttack3()
    {
        if (showDebug)
        {
            Debug.Log("🔥 Boss: ExecuteAttack3 called!");
        }

        if (player == null)
        {
            Debug.LogWarning("Boss: ExecuteAttack3 - Player is null!");
            return;
        }

        // Check if player is in attack box
        Vector2 attackCenter = (Vector2)transform.position + attackRangeBoxOffset;
        bool playerHit = Physics2D.OverlapBox(attackCenter, attackRangeBoxSize, 0, playerLayer);

        if (showDebug)
        {
            Debug.Log($"Boss: ExecuteAttack3 - Player in attack box: {playerHit}");
        }

        if (playerHit)
        {
            PlayerController1 playerController = player.GetComponent<PlayerController1>();
            if (playerController != null)
            {
                float damage = attackDamage * 1.5f; // 50% more damage
                playerController.TakeDamage(damage, false);
                Debug.Log($"🩸 Boss: ExecuteAttack3 dealt {damage} damage to player!");
            }
            else
            {
                Debug.LogWarning("Boss: ExecuteAttack3 - Player doesn't have PlayerController1!");
            }
        }
        else
        {
            Debug.Log("Boss: ExecuteAttack3 - Player not in attack range, no damage dealt");
        }
    }

    /// <summary>
    /// Called by animation event when any attack animation ends
    /// </summary>
    public void OnAttackEnd()
    {
        if (showDebug)
        {
            Debug.Log("Boss: OnAttackEnd called - resetting attack state");
        }

        isAttacking = false;

        // Reset animation state
        animator.SetBool(IsWalkingHash, false);

        // Check if should continue attacking, idle, or chase
        if (player != null)
        {
            Vector2 attackCenter = (Vector2)transform.position + attackRangeBoxOffset;
            bool playerStillInAttackBox = Physics2D.OverlapBox(attackCenter, attackRangeBoxSize, 0, playerLayer);

            if (!playerStillInAttackBox)
            {
                // Player moved away - start idle state
                StartIdle();
                if (showDebug)
                {
                    Debug.Log("Boss: Player left attack box after attack - starting idle");
                }
            }
            else
            {
                if (showDebug)
                {
                    Debug.Log("Boss: Player still in attack box - ready for next attack");
                }
            }
        }
    }
    
    // ===== HEALTH & DAMAGE SYSTEM =====

    public void TakeDamage(float damage)
    {
        if (isDead || isHurt) return; // Don't take damage if already hurt

        // Delegate to health component
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }

        // Trigger hurt state and animation
        OnTakeDamage();
    }

    /// <summary>
    /// Called when boss takes damage - triggers hurt animation
    /// </summary>
    public void OnTakeDamage()
    {
        if (isDead || isHurt) return;

        if (showDebug)
        {
            Debug.Log("Boss: Taking damage - triggering hurt animation");
        }

        // Set hurt state
        isHurt = true;
        isAttacking = false; // Cancel any current attack
        isIdling = false; // Cancel any idle state

        // Stop movement
        rb.linearVelocity = Vector2.zero;
        animator.SetBool(IsWalkingHash, false);

        // Trigger hurt animation
        animator.SetTrigger(HurtHash);

        // Auto-recover from hurt state
        StartCoroutine(RecoverFromHurt());
    }

    /// <summary>
    /// Recover from hurt state after animation
    /// </summary>
    private System.Collections.IEnumerator RecoverFromHurt()
    {
        yield return new WaitForSeconds(hurtDuration);

        if (showDebug)
        {
            Debug.Log("Boss: Recovering from hurt state");
        }

        isHurt = false;
    }

    /// <summary>
    /// Called by animation event when hurt animation ends
    /// </summary>
    public void OnHurtEnd()
    {
        if (showDebug)
        {
            Debug.Log("Boss: OnHurtEnd called - recovering from hurt state");
        }

        isHurt = false;
    }

    // ===== PUBLIC PROPERTIES FOR OTHER SCRIPTS =====

    /// <summary>
    /// Check if boss is currently attacking (for BossDamage script)
    /// </summary>
    public bool IsCurrentlyAttacking => isAttacking;

    /// <summary>
    /// Check if boss is currently hurt (for other scripts)
    /// </summary>
    public bool IsCurrentlyHurt => isHurt;

    /// <summary>
    /// Check if boss is dead (for other scripts)
    /// </summary>
    public bool IsCurrentlyDead => isDead;

    // ===== DEBUG/TEST METHODS =====

    /// <summary>
    /// Force execute attack for testing (call from Inspector)
    /// </summary>
    [ContextMenu("Force Execute Attack")]
    public void ForceExecuteAttack()
    {
        Debug.Log("🔥 FORCE EXECUTING ATTACK FOR TEST!");
        ExecuteAttack1();
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        isAttacking = false;

        // Stop all movement
        rb.linearVelocity = Vector2.zero;

        // Reset animator bools
        animator.SetBool(IsWalkingHash, false);

        // Trigger death animation
        animator.SetTrigger(DiedHash);


    }
    
    // ===== UTILITY FUNCTIONS =====

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

        // Attack range box
        Gizmos.color = Color.red;
        Vector3 boxCenter = transform.position + (Vector3)attackRangeBoxOffset;
        Gizmos.DrawWireCube(boxCenter, attackRangeBoxSize);

        // Line to player
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
