using UnityEngine;
using System.Collections;

public class Map3BossController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public Rigidbody2D rb;
    public EnemyHealth enemyHealth;

    [Header("Detection & Attack Area")]
    public float detectionRange = 8f; // Phạm vi phát hiện player
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
    private bool playerDetected = false; // Trạng thái phát hiện player

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

        // Đảm bảo Rigidbody2D settings đúng
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.freezeRotation = true; // Không xoay
            rb.linearDamping = 0f; // Không có drag
        }
        else
        {
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

        // SIMPLE MOVEMENT LOGIC (từ SimpleBossMovement)
        HandleSimpleMovement();

        // Attack logic - chỉ check attack range
        Vector2 attackCenter = (Vector2)transform.position + attackRangeBoxOffset;
        bool playerInAttackBox = Physics2D.OverlapBox(
            attackCenter,
            attackRangeBoxSize,
            0,
            playerLayer
        );

        if (playerInAttackBox && !isAttacking && !isHurt)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                DoRandomAttack();
                lastAttackTime = Time.time;
            }
        }
    }

    /// <summary>
    /// Simple movement logic (từ SimpleBossMovement) - ĐƠN GIẢN VÀ HIỆU QUẢ
    /// </summary>
    void HandleSimpleMovement()
    {
        if (rb == null || player == null) return;

        // Dừng di chuyển nếu boss đã chết hoặc đang attack hoặc hurt
        if (isDead || isAttacking || isHurt)
        {
            SimpleStopMoving();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        // Nếu player ngoài detection range - đứng yên
        if (distance > detectionRange)
        {
            SimpleStopMoving();
            return;
        }

        // Nếu đã đến gần - dừng để attack
        if (distance <= stopDistance)
        {
            SimpleStopMoving();
            return;
        }

        // DI CHUYỂN TỚI PLAYER - LOGIC ĐƠN GIẢN
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        // Set animation - dùng string như SimpleBossMovement
        if (animator != null) animator.SetBool("IsWalking", true);

        // Flip boss hướng về player - logic đơn giản
        if (direction.x > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        // Debug
        if (showDebug && Time.frameCount % 60 == 0)
        {
            Debug.Log($"🚶 Simple Boss Movement: Distance={distance:F2}, Velocity={rb.linearVelocity.x:F2}, Speed={moveSpeed}");
        }
    }

    /// <summary>
    /// Stop movement - đơn giản như SimpleBossMovement
    /// </summary>
    void SimpleStopMoving()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
        }
    }

    void HandleBossState_OLD(bool playerInAttackBox, bool playerInDetectionRange)
    {
        // DISABLED - không dùng nữa, thay bằng HandleSimpleMovement()
        /*
        if (isHurt || isAttacking)
        {
            animator.SetBool(IsWalkingHash, false);
            return;
        }

        // Nếu player không trong detection range, boss đứng yên
        if (!playerInDetectionRange)
        {
            animator.SetBool(IsWalkingHash, false);
            if (showDebug && Time.frameCount % 120 == 0)
            return;
        }

        // Player trong detection range
        if (playerInAttackBox)
        {
            // Player trong attack range - tấn công
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
            // Player trong detection range nhưng ngoài attack range
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
                // Di chuyển tới player
                LookAtPlayer();
                animator.SetBool(IsWalkingHash, true);
            }
        }

        wasPlayerInAttackBox = playerInAttackBox;
        */
    }

    // FixedUpdate DISABLED - sử dụng Update với logic đơn giản
    /*
    void FixedUpdate()
    {
        // Handle movement in FixedUpdate
        if (isDead || player == null) return;

        // Don't move if EnemyHealth says we're dead
        if (enemyHealth != null && enemyHealth.isDead) return;

        // FORCE MOVEMENT - Đảm bảo boss di chuyển
        HandleBossMovement();
    }
    */

    /// <summary>
    /// OLD movement logic - DISABLED
    /// </summary>
    void HandleBossMovement_OLD()
    {
        // DISABLED - Using HandleSimpleMovement() instead
        /*
        // Không di chuyển nếu đang attack, hurt, hoặc dead
        if (isAttacking || isHurt || isDead)
        {
            StopMoving();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        // Nếu player ngoài detection range - đứng yên
        if (distance > detectionRange)
        {
            StopMoving();
            return;
        }

        // Nếu quá gần player - đứng yên để attack
        if (distance <= stopDistance)
        {
            StopMoving();
            return;
        }

        // DI CHUYỂN TỚI PLAYER
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 velocity = rb.linearVelocity;
        velocity.x = direction.x * moveSpeed;
        rb.linearVelocity = velocity;

        // Set animation
        animator.SetBool(IsWalkingHash, true);

        // Face player
        LookAtPlayer();
        */

        if (showDebug && Time.frameCount % 60 == 0) // Debug mỗi giây
        {
        }
    }

    /// <summary>
    /// Simple movement logic (merged from SimpleBossMovement) - DISABLED
    /// </summary>
    void HandleSimpleMovement_OLD()
    {
        // DISABLED - Using HandleBossMovement() instead
        /*
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
        */

        float distance = Vector2.Distance(transform.position, player.position);

        // Kiểm tra detection range trước
        if (distance > detectionRange)
        {
            if (showDebug && Time.frameCount % 120 == 0)
                Debug.Log($"🚶 Simple Movement: Player out of detection range - Distance:{distance:F2}, DetectionRange:{detectionRange}");
            StopMoving();
            return;
        }

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
    /// Advanced movement logic (original Map3BossController logic) - DISABLED
    /// </summary>
    void HandleAdvancedMovement_OLD()
    {
        // DISABLED - Using HandleBossMovement() instead
        /*
        Vector2 velocity = rb.linearVelocity;
        Vector2 attackCenter = (Vector2)transform.position + attackRangeBoxOffset;
        bool playerInAttackBox = Physics2D.OverlapBox(
            attackCenter,
            attackRangeBoxSize,
            0,
            playerLayer
        );

        float distance = Vector2.Distance(transform.position, player.position);

        // Kiểm tra detection range
        if (distance > detectionRange)
        {
            velocity.x = 0; // Đứng yên nếu player ngoài detection range
        }
        else if (!playerInAttackBox && !isAttacking && !isIdling && !isHurt)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            velocity.x = direction.x * moveSpeed;
        }
        else
        {
            velocity.x = 0;
        }

        rb.linearVelocity = velocity;
        */
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

    // IDLE METHODS DISABLED - không cần với simple movement
    /*
    void StartIdle()
    {
        isIdling = true;
        idleStartTime = Time.time;
        animator.SetBool(IsWalkingHash, false);
    }

    void EndIdle()
    {
        isIdling = false;
    }
    */

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


    public void ExecuteAttack1()
    {
        if (showDebug)
            Debug.Log("🔥 Boss: ExecuteAttack1 called!");

        Vector2 attackCenter = (Vector2)transform.position + attackRangeBoxOffset;
        Collider2D playerCollider = Physics2D.OverlapBox(attackCenter, attackRangeBoxSize, 0, playerLayer);

        if (playerCollider != null)
        {
            var playerController = playerCollider.GetComponentInParent<PlayerController1>();
            if (playerController != null)
            {
                playerController.TakeDamage(attackDamage, false);
            }
            else
            {
            }
        }
        else
        {
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
            if (playerController != null)
            {
                float damage = attackDamage * 1.2f;
                playerController.TakeDamage(damage, false);
            }
            else
            {
            }
        }
        else
        {
        }
    }

    public void ExecuteAttack3()
    {
        Vector2 attackCenter = (Vector2)transform.position + attackRangeBoxOffset;
        Collider2D playerCollider = Physics2D.OverlapBox(attackCenter, attackRangeBoxSize, 0, playerLayer);

        if (playerCollider != null)
        {
            var playerController = playerCollider.GetComponentInParent<PlayerController1>();
            if (playerController != null)
            {
                float damage = attackDamage * 1.5f;
                playerController.TakeDamage(damage, false);
            }
            else
            {
            }
        }
        else
        {
        }
    }

    public void OnAttackEnd()
    {
        isAttacking = false;
        animator.SetBool(IsWalkingHash, false);

        // DISABLED - không cần idle logic với simple movement
        /*
        if (player != null)
        {
            Vector2 attackCenter = (Vector2)transform.position + attackRangeBoxOffset;
            bool playerStillInAttackBox = Physics2D.OverlapBox(attackCenter, attackRangeBoxSize, 0, playerLayer);

            if (!playerStillInAttackBox)
            {
                StartIdle();
            }
        }
        */
    }


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

        isHurt = false;
    }

    public void OnHurtEnd()
    {
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

        // Draw detection range circle (vàng)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw attack range box (đỏ)
        Gizmos.color = Color.red;
        Vector3 boxCenter = transform.position + (Vector3)attackRangeBoxOffset;
        Gizmos.DrawWireCube(boxCenter, attackRangeBoxSize);

        // Draw line to player if detected (xanh lá)
        if (player != null && playerDetected)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }

        // Draw center point (trắng)
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}
