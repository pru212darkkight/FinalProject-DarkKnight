using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyCombatAI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Detection & Attack Ranges")]
    public float detectionRange = 8f;
    public float stopRange = 1.5f;
    public float moveSpeed = 2f;
    public float attackCooldown = 1.5f;

    [Header("Attack")]
    public float attackDamage = 20f;

    [Header("Chase Limit")]
    public float chaseLimitRange = 10f; // Enemy sẽ không rời khỏi điểm gốc quá xa

    private float lastAttackTime = 0f;
    private Rigidbody2D rb;
    private Animator animator;
    private EnemyHealth enemyHealth;
    private Vector2 homePosition;
    private bool returningToHome = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        homePosition = transform.position;
    }

    private void Update()
    {
        if (enemyHealth.isDead || player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float playerFromHome = Vector2.Distance(player.position, homePosition);
        float selfFromHome = Vector2.Distance(transform.position, homePosition);

        // ✅ Player trong tầm detection và nằm trong phạm vi đuổi
        if (distanceToPlayer <= detectionRange && playerFromHome <= chaseLimitRange)
        {
            returningToHome = false;

            if (distanceToPlayer > stopRange)
            {
                MoveTowards(player.position);
                SetAnimationMove(true);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                SetAnimationMove(false);

                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack();
                }
            }
        }
        else if (selfFromHome > 0.1f)
        {
            // Player quá xa hoặc ngoài vùng đuổi → quay về vị trí ban đầu
            returningToHome = true;
            MoveTowards(homePosition);
            SetAnimationMove(true);
        }
        else
        {
            // Đã về đến vị trí gốc, đứng yên
            rb.linearVelocity = Vector2.zero;
            SetAnimationMove(false);
            returningToHome = false;
        }
    }


    void MoveTowards(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        if (direction.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(direction.x), 1f, 1f);
    }

    void Attack()
    {
        lastAttackTime = Time.time;

        // 🎵 Play attack sound
        if (AudioManager.Instance != null && AudioManager.Instance.dAttackSound != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.dAttackSound);
        }

        if (animator != null)
            animator.SetTrigger("Attack");
    }

    public void DealDamage()
    {
        if (!player) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= stopRange + 0.3f)
        {
            PlayerController1 pc = player.GetComponent<PlayerController1>();
            if (pc != null)
            {
                pc.TakeDamage(attackDamage, false,"Hell Monster");
            }
        }
    }

    void SetAnimationMove(bool isMoving)
    {
        if (animator != null)
            animator.SetBool("isMoving", isMoving);
    }

    // Method để gọi khi enemy chết (từ EnemyHealth script)
    public void OnDeath()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.dAttackDeath != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.dAttackDeath);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Application.isPlaying ? homePosition : transform.position, chaseLimitRange);
    }
}
