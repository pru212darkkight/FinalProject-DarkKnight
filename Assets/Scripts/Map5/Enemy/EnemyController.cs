using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public EnemyWeapon enemyWeapon;  // Kéo reference EnemyWeapon

    [Header("Guard/Patrol")]
    public float detectRange = 5f;    // Vùng canh gác
    public float moveSpeed = 2.5f;
    public float healthRegenRate = 5f;
    public float attackCooldown = 1f;
    private float lastAttackTime;

    [HideInInspector]
    public Vector3 startPoint;
    private Rigidbody2D rb;
    private Animator animator;
    private EnemyHealth enemyHealth;

    private bool isPlayerDetected = false;
    private bool isReturning = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        startPoint = transform.position;
    }

    void Update()
    {
        if (enemyHealth != null && enemyHealth.isDead)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isRunning", false); // Đảm bảo không set lại các state khác
            return;
        }
        float playerDist = Vector2.Distance(player.position, startPoint);
        float distToPlayer = Vector2.Distance(player.position, transform.position);

        if (playerDist <= detectRange)
        {
            isPlayerDetected = true;
            isReturning = false;
        }
        else if (isPlayerDetected)
        {
            isPlayerDetected = false;
            isReturning = true;
        }

        // Đuổi/tấn công player
        if (isPlayerDetected)
        {
            LookAtTarget(player.position.x);

            if (distToPlayer <= enemyWeapon.attackRange)
            {
                rb.linearVelocity = Vector2.zero;
                animator.SetBool("isRunning", false);
                if (Time.time > lastAttackTime + attackCooldown)
                {
                    animator.SetTrigger("Attack");
                    lastAttackTime = Time.time;
                }
            }
            else
            {
                Vector2 target = new Vector2(player.position.x, rb.position.y);
                Vector2 newPos = Vector2.MoveTowards(rb.position, target, moveSpeed * Time.deltaTime);
                rb.MovePosition(newPos);
                animator.SetBool("isRunning", true);
            }
        }
        // Quay về chỗ cũ, hồi máu
        else if (isReturning)
        {
            float distToStart = Vector2.Distance(transform.position, startPoint);
            if (distToStart > 0.1f)
            {
                LookAtTarget(startPoint.x);
                Vector2 newPos = Vector2.MoveTowards(rb.position, startPoint, moveSpeed * Time.deltaTime);
                rb.MovePosition(newPos);
                animator.SetBool("isRunning", true);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                animator.SetBool("isRunning", false);
                isReturning = false;
            }

            // Hồi máu dần
            if (!isPlayerDetected && distToStart <= 0.15f && enemyHealth.currentHealth < enemyHealth.maxHealth)
            {
                enemyHealth.currentHealth += healthRegenRate * Time.deltaTime;
                enemyHealth.currentHealth = Mathf.Min(enemyHealth.currentHealth, enemyHealth.maxHealth);
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isRunning", false);
            // Hồi máu khi đã ở chỗ gốc
            if (enemyHealth.currentHealth < enemyHealth.maxHealth)
            {
                enemyHealth.currentHealth += healthRegenRate * Time.deltaTime;
                enemyHealth.currentHealth = Mathf.Min(enemyHealth.currentHealth, enemyHealth.maxHealth);
            }
        }
    }

    void LookAtTarget(float targetX)
    {
        if (targetX > transform.position.x && transform.localScale.x < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
        else if (targetX < transform.position.x && transform.localScale.x > 0)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    // Vẽ vùng canh gác (detect range) để nhìn rõ trên scene
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}
