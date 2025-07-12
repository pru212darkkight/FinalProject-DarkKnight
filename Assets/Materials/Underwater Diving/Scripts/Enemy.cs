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
    public float minFlipDistance = 0.5f;

    [Header("Patrol Settings")]
    public bool enablePatrol = true;
    public float patrolSpeed = 1.5f;
    public float patrolDistance = 3f;
    public float patrolWaitTime = 2f;

    [Header("Defense Stats")]
    public float armor = 5f;
    public float magicResist = 5f;

    [Header("UI")]
    public Image healthBar;

    // Components
    protected Rigidbody2D rb;
    protected Animator animator;
    protected Transform player;
    protected bool isFacingRight = true;
    protected float lastAttackTime;
    protected bool isDead = false;

    // Patrol variables
    protected Vector3 startPosition;
    protected Vector3 patrolTargetLeft;
    protected Vector3 patrolTargetRight;
    protected Vector3 currentPatrolTarget;
    protected float patrolWaitTimer;
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

        if (rb != null)
        {
            rb.gravityScale = 1f;
            rb.linearDamping = 1f;
            rb.angularDamping = 5f;
            rb.freezeRotation = true;
        }

        InitializePatrol();

        if (player != null)
        {
            isFacingRight = player.position.x > transform.position.x;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (isFacingRight ? 1 : -1);
            transform.localScale = scale;
        }
    }

    protected virtual void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer <= attackRange)
            {
                rb.linearVelocity = Vector2.zero;
                if (animator != null)
                {
                    animator.SetFloat(SpeedHash, 0);
                }
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack();
                }
            }
            else
            {
                Vector2 directionToPlayer = (player.position - transform.position).normalized;
                Vector2 newVelocity = directionToPlayer * moveSpeed;
                rb.linearVelocity = newVelocity;

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
            }
        }
        else
        {
            if (enablePatrol)
            {
                PatrolBehavior();
            }
            else
            {
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

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D hitPlayer in hitPlayers)
        {
            if (hitPlayer.CompareTag("Player"))
            {
                PlayerController1 playerController = hitPlayer.GetComponentInParent<PlayerController1>();
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
            finalDamage *= (1 - (magicResist / 100f));
        }
        else
        {
            finalDamage *= (1 - (armor / 100f));
        }

        currentHealth = Mathf.Max(0, currentHealth - finalDamage);
        UpdateHealthBar();

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
        if (isDead) return;
        isDead = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }

        if (animator != null)
        {
            animator.SetTrigger(DieHash);
            Destroy(gameObject, 1.5f);
        }
        else
        {
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
            PlayerController1 playerController = other.GetComponentInParent<PlayerController1>();
            if (playerController != null)
            {
                if (playerController.IsAttacking || playerController.IsAttacking2 || playerController.IsAttacking3)
                {
                    float playerDamage = 25f;
                    TakeDamage(playerDamage);
                    return;
                }

                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    playerController.TakeDamage(damage);
                    lastAttackTime = Time.time;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (enablePatrol)
        {
            Gizmos.color = Color.green;
            Vector3 start = Application.isPlaying ? startPosition : transform.position;
            Vector3 left = start + Vector3.left * patrolDistance;
            Vector3 right = start + Vector3.right * patrolDistance;
            Gizmos.DrawLine(left, right);
            Gizmos.DrawWireSphere(left, 0.3f);
            Gizmos.DrawWireSphere(right, 0.3f);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(start, 0.2f);
        }
    }

    // Public properties
    public bool IsDead => isDead;
    public float HealthPercent => currentHealth / maxHealth;

    // Debug methods (có thể xóa nếu muốn)
    [ContextMenu("Kill Enemy")]
    public void KillEnemy()
    {
        TakeDamage(maxHealth);
    }

    [ContextMenu("Damage Enemy")]
    public void DamageEnemy()
    {
        TakeDamage(25f);
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
        // Không cần debug ở đây
    }

    protected void InitializePatrol()
    {
        if (!enablePatrol) return;

        startPosition = transform.position;
        patrolTargetLeft = startPosition + Vector3.left * patrolDistance;
        patrolTargetRight = startPosition + Vector3.right * patrolDistance;
        currentPatrolTarget = patrolTargetRight;
    }

    protected void PatrolBehavior()
    {
        if (!enablePatrol) return;

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

        Vector2 directionToTarget = (currentPatrolTarget - transform.position).normalized;
        float distanceToTarget = Vector2.Distance(transform.position, currentPatrolTarget);

        if (distanceToTarget <= 0.5f)
        {
            isWaitingAtPatrolPoint = true;
            patrolWaitTimer = patrolWaitTime;
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            Vector2 patrolVelocity = directionToTarget * patrolSpeed;
            rb.linearVelocity = patrolVelocity;

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
