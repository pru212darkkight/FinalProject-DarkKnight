using UnityEngine;
using UnityEngine.UI;

public class EnemyWater : MonoBehaviour
{
    [Header("Basic Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float damage = 10f;
    public float moveSpeed = 3f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float detectionRange = 5f;
    public float minFlipDistance = 0.5f; // Khoảng cách tối thiểu để quay đầu

    [Header("Patrol Settings")]
    public bool enablePatrol = true;      // Bật/tắt patrol
    public float patrolSpeed = 1.5f;      // Tốc độ patrol (chậm hơn chase)
    public float patrolDistance = 3f;     // Khoảng cách patrol từ vị trí ban đầu
    public float patrolWaitTime = 2f;     // Thời gian đợi ở mỗi điểm patrol

    [Header("Defense Stats")]
    public float armor = 5f;          // Giáp
    public float magicResist = 5f;    // Kháng phép

    [Header("UI")]
    public Image healthBar;           // UI health bar (nếu có)

    // Components
    protected Rigidbody2D rb;
    protected Animator animator;
    protected Transform player;
    protected bool isFacingRight = true;
    protected float lastAttackTime;
    protected bool isDead = false;

    // Patrol variables
    protected Vector3 startPosition;      // Vị trí ban đầu
    protected Vector3 patrolTargetLeft;   // Điểm patrol bên trái
    protected Vector3 patrolTargetRight;  // Điểm patrol bên phải
    protected Vector3 currentPatrolTarget; // Điểm patrol hiện tại
    protected float patrolWaitTimer;      // Timer đợi tại điểm patrol
    protected bool isWaitingAtPatrolPoint = false;

    // Animation parameters
    protected readonly int SpeedHash = Animator.StringToHash("Speed");
    protected readonly int AttackHash = Animator.StringToHash("Attack");
    protected readonly int HurtHash = Animator.StringToHash("Hurt");
    protected readonly int DieHash = Animator.StringToHash("Death");

    // Helper function to check if animator parameter exists
    protected bool HasParameter(Animator animator, string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentHealth = maxHealth;
        UpdateHealthBar();

        // Setup Rigidbody2D for underwater movement
        if (rb != null)
        {
            rb.gravityScale = 1f; // No gravity in water
            rb.linearDamping = 1f; // Some water resistance
            rb.angularDamping = 5f; // Prevent spinning
            rb.freezeRotation = true; // Keep upright
            Debug.Log($"{gameObject.name} Rigidbody2D configured for underwater movement");
        }
        else
        {
            Debug.LogError($"{gameObject.name} missing Rigidbody2D component!");
        }

        // Initialize patrol system
        InitializePatrol();

        // Set initial facing direction based on player position
        if (player != null)
        {
            isFacingRight = player.position.x > transform.position.x;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (isFacingRight ? 1 : -1);
            transform.localScale = scale;
            Debug.Log($"{gameObject.name} initialized. Player found at {player.position}");
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} could not find Player!");
        }
    }

    protected virtual void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Nếu player trong tầm phát hiện
        if (distanceToPlayer <= detectionRange)
        {
            // Nếu trong tầm tấn công
            if (distanceToPlayer <= attackRange)
            {
                // Dừng di chuyển và tấn công
                rb.linearVelocity = Vector2.zero;
                if (animator != null)
                {
                    animator.SetFloat(SpeedHash, 0);
                }

                // Tấn công nếu đã hết cooldown
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack();
                }
            }
            else
            {
                // Di chuyển về phía player - đơn giản hóa
                Vector2 directionToPlayer = (player.position - transform.position).normalized;
                Vector2 newVelocity = directionToPlayer * moveSpeed;
                rb.linearVelocity = newVelocity;

                // Flip sprite dựa trên hướng di chuyển
                if (directionToPlayer.x > 0 && !isFacingRight)
                {
                    Flip();
                }
                else if (directionToPlayer.x < 0 && isFacingRight)
                {
                    Flip();
                }

                if (animator != null)
                {
                    animator.SetFloat(SpeedHash, moveSpeed);
                }

                Debug.Log($"{gameObject.name} moving towards player. Velocity: {newVelocity}, Distance: {distanceToPlayer:F2}");
            }
        }
        else
        {
            // Player ngoài tầm phát hiện - thực hiện patrol
            if (enablePatrol)
            {
                PatrolBehavior();
            }
            else
            {
                // Dừng di chuyển nếu không patrol
                rb.linearVelocity = Vector2.zero;
                if (animator != null)
                {
                    animator.SetFloat(SpeedHash, 0);
                }
            }
        }
    }

    protected virtual void Attack()
    {
        lastAttackTime = Time.time;
        if (animator != null)
        {
            animator.SetTrigger(AttackHash);
        }

        // Kiểm tra va chạm với player
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D hitPlayer in hitPlayers)
        {
            if (hitPlayer.CompareTag("Player"))
            {
                PlayerController1 playerController = hitPlayer.GetComponent<PlayerController1>();
                if (playerController != null)
                {
                    playerController.TakeDamage(damage);
                }
                break;
            }
        }
    }

    public virtual void TakeDamage(float damage, bool isMagicDamage = false)
    {
        if (isDead)
        {
            Debug.Log($"{gameObject.name} is already dead, ignoring damage");
            return;
        }

        Debug.Log($"{gameObject.name} TakeDamage called with {damage} damage. Current health: {currentHealth}/{maxHealth}");

        float finalDamage = damage;
        if (isMagicDamage)
        {
            finalDamage *= (1 - (magicResist / 100f)); // Giảm sát thương phép thuật dựa trên kháng phép
        }
        else
        {
            finalDamage *= (1 - (armor / 100f)); // Giảm sát thương vật lý dựa trên giáp
        }

        float healthBefore = currentHealth;
        currentHealth = Mathf.Max(0, currentHealth - finalDamage);
        UpdateHealthBar();

        Debug.Log($"{gameObject.name} took {finalDamage} damage. Health: {healthBefore} -> {currentHealth}/{maxHealth}");

        // Trigger hurt animation nếu có
        if (animator != null)
        {
            animator.SetTrigger(HurtHash);
        }

        if (currentHealth <= 0)
        {
            Debug.Log($"{gameObject.name} health reached 0, calling Die()");
            Die();
        }
    }

    protected virtual void Die()
    {
        if (isDead)
        {
            Debug.Log($"{gameObject.name} Die() called but already dead");
            return; // Prevent multiple calls
        }

        Debug.Log($"{gameObject.name} Die() method called - setting isDead = true");
        isDead = true;

        // Stop all movement immediately
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
            Debug.Log($"{gameObject.name} stopped movement");
        }

        // Disable collider to prevent further interactions
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
            Debug.Log($"{gameObject.name} disabled collider");
        }

        // Hide health bar if exists
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
            Debug.Log($"{gameObject.name} hid health bar");
        }

        // Trigger death animation if available
        if (animator != null)
        {
            animator.SetTrigger(DieHash);
            Debug.Log($"{gameObject.name} triggered death animation, will destroy in 1.5s");
            // Destroy after animation time (estimate 1-2 seconds)
            Destroy(gameObject, 1.5f);
        }
        else
        {
            Debug.Log($"{gameObject.name} no animator found, will destroy in 0.1s");
            // No animation, destroy immediately
            Destroy(gameObject, 0.1f);
        }

        Debug.Log($"{gameObject.name} Die() method completed");
    }

    protected virtual void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    protected virtual void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            PlayerController1 playerController = other.GetComponent<PlayerController1>();
            if (playerController != null)
            {
                // Check if player is attacking
                if (playerController.IsAttacking || playerController.IsAttacking2 || playerController.IsAttacking3)
                {
                    Debug.Log($"{gameObject.name} hit by player attack!");

                    // Enemy takes damage from player attack (use a fixed damage amount)
                    float playerDamage = 25f; // Player deals 25 damage per hit
                    TakeDamage(playerDamage);

                    Debug.Log($"{gameObject.name} took {playerDamage} damage from player. Health: {currentHealth}/{maxHealth}");
                    return;
                }

                // Deal damage to player if not attacking and cooldown is ready
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    playerController.TakeDamage(damage);
                    lastAttackTime = Time.time;
                    Debug.Log($"{gameObject.name} dealt {damage} damage to player");
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Vẽ tầm phát hiện
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Vẽ tầm tấn công
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Vẽ patrol area
        if (enablePatrol)
        {
            Gizmos.color = Color.green;
            Vector3 start = Application.isPlaying ? startPosition : transform.position;
            Vector3 left = start + Vector3.left * patrolDistance;
            Vector3 right = start + Vector3.right * patrolDistance;

            // Vẽ đường patrol
            Gizmos.DrawLine(left, right);

            // Vẽ điểm patrol
            Gizmos.DrawWireSphere(left, 0.3f);
            Gizmos.DrawWireSphere(right, 0.3f);

            // Vẽ vị trí ban đầu
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(start, 0.2f);
        }
    }

    // Public properties
    public bool IsDead => isDead;
    public float HealthPercent => currentHealth / maxHealth;

    // Debug methods
    [ContextMenu("Kill Enemy")]
    public void KillEnemy()
    {
        Debug.Log($"Manually killing {gameObject.name}");
        TakeDamage(maxHealth);
    }

    [ContextMenu("Damage Enemy")]
    public void DamageEnemy()
    {
        Debug.Log($"Manually damaging {gameObject.name}");
        TakeDamage(25f); // Use same damage as player attack
    }

    [ContextMenu("Check Enemy Status")]
    public void CheckEnemyStatus()
    {
        Debug.Log($"=== {gameObject.name} Status ===");
        Debug.Log($"Health: {currentHealth}/{maxHealth}");
        Debug.Log($"Is Dead: {isDead}");
        Debug.Log($"GameObject Active: {gameObject.activeInHierarchy}");
        Debug.Log($"Has Rigidbody2D: {rb != null}");
        Debug.Log($"Has Collider2D: {GetComponent<Collider2D>() != null}");
        Debug.Log($"Collider Enabled: {GetComponent<Collider2D>()?.enabled}");
        Debug.Log($"Has Animator: {animator != null}");
        Debug.Log($"========================");
    }

    void OnDestroy()
    {
        Debug.Log($"🔥 {gameObject.name} has been DESTROYED! 🔥");
    }

    // Patrol system methods
    protected void InitializePatrol()
    {
        if (!enablePatrol) return;

        startPosition = transform.position;
        patrolTargetLeft = startPosition + Vector3.left * patrolDistance;
        patrolTargetRight = startPosition + Vector3.right * patrolDistance;

        // Bắt đầu patrol về phía phải
        currentPatrolTarget = patrolTargetRight;

        Debug.Log($"{gameObject.name} Patrol initialized. Left: {patrolTargetLeft}, Right: {patrolTargetRight}");
    }

    protected void PatrolBehavior()
    {
        if (!enablePatrol) return;

        // Nếu đang đợi tại điểm patrol
        if (isWaitingAtPatrolPoint)
        {
            patrolWaitTimer -= Time.deltaTime;
            rb.linearVelocity = Vector2.zero;

            if (animator != null)
            {
                animator.SetFloat(SpeedHash, 0);
            }

            if (patrolWaitTimer <= 0)
            {
                isWaitingAtPatrolPoint = false;
                // Chuyển sang điểm patrol tiếp theo
                if (currentPatrolTarget == patrolTargetRight)
                {
                    currentPatrolTarget = patrolTargetLeft;
                }
                else
                {
                    currentPatrolTarget = patrolTargetRight;
                }
            }
            return;
        }

        // Di chuyển đến điểm patrol
        Vector2 directionToTarget = (currentPatrolTarget - transform.position).normalized;
        float distanceToTarget = Vector2.Distance(transform.position, currentPatrolTarget);

        // Nếu đã đến gần điểm patrol
        if (distanceToTarget <= 0.5f)
        {
            isWaitingAtPatrolPoint = true;
            patrolWaitTimer = patrolWaitTime;
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            // Di chuyển về phía điểm patrol
            Vector2 patrolVelocity = directionToTarget * patrolSpeed;
            rb.linearVelocity = patrolVelocity;

            // Flip sprite dựa trên hướng di chuyển
            if (directionToTarget.x > 0 && !isFacingRight)
            {
                Flip();
            }
            else if (directionToTarget.x < 0 && isFacingRight)
            {
                Flip();
            }

            if (animator != null && HasParameter(animator, "Speed"))
            {
                animator.SetFloat(SpeedHash, patrolSpeed);
            }
        }
    }

}
