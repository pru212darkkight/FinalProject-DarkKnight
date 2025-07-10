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

    // Animation parameters
    protected readonly int SpeedHash = Animator.StringToHash("Speed");
    protected readonly int AttackHash = Animator.StringToHash("Attack");
    protected readonly int HurtHash = Animator.StringToHash("Hurt");
    protected readonly int DieHash = Animator.StringToHash("Death");

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
            rb.gravityScale = 0f; // No gravity in water
            rb.linearDamping = 1f; // Some water resistance
            rb.angularDamping = 5f; // Prevent spinning
            rb.freezeRotation = true; // Keep upright
            Debug.Log($"{gameObject.name} Rigidbody2D configured for underwater movement");
        }
        else
        {
            Debug.LogError($"{gameObject.name} missing Rigidbody2D component!");
        }

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
            // Dừng di chuyển nếu player ngoài tầm
            rb.linearVelocity = Vector2.zero;
            if (animator != null)
            {
                animator.SetFloat(SpeedHash, 0);
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
        if (isDead) return;

        float finalDamage = damage;
        if (isMagicDamage)
        {
            finalDamage *= (1 - (magicResist / 100f)); // Giảm sát thương phép thuật dựa trên kháng phép
        }
        else
        {
            finalDamage *= (1 - (armor / 100f)); // Giảm sát thương vật lý dựa trên giáp
        }

        currentHealth = Mathf.Max(0, currentHealth - finalDamage);
        UpdateHealthBar();

        // Trigger hurt animation nếu có
        if (animator != null)
        {
            animator.SetTrigger(HurtHash);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (isDead) return; // Prevent multiple calls

        isDead = true;

        Debug.Log($"{gameObject.name} died and will be destroyed!");

        // Stop all movement immediately
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // Disable collider to prevent further interactions
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Hide health bar if exists
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }

        // Trigger death animation if available
        if (animator != null)
        {
            animator.SetTrigger(DieHash);
            // Destroy after animation time (estimate 1-2 seconds)
            Destroy(gameObject, 1.5f);
        }
        else
        {
            // No animation, destroy immediately
            Destroy(gameObject, 0.1f);
        }
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
                    TakeDamage(damage); // Enemy takes damage when hit by player
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
        TakeDamage(damage);
    }

    void OnDestroy()
    {
        Debug.Log($"{gameObject.name} has been destroyed!");
    }
}
