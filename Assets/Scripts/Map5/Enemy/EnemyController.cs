using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public EnemyWeapon enemyWeapon;

    [Header("Guard/Patrol")]
    public float detectRangeX = 5f;
    public float detectRangeY = 2f;
    public float moveSpeed = 2.5f;
    public float attackRange = 1.5f;
    public float healthRegenRate = 5f;
    public float attackCooldown = 1f;

    [Header("AI Options")]
    public bool returnToOrigin = true; // Bật/tắt về gốc khi mất dấu player

    private float lastAttackTime;
    [HideInInspector] public Vector3 startPoint;
    private Rigidbody2D rb;
    private Animator animator;
    private EnemyHealth enemyHealth;

    private bool isReturning = false;
    private bool previousPlayerDetected = false;

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
            animator.SetBool("isRunning", false);
            return;
        }

        float distToPlayer = Vector2.Distance(player.position, transform.position);

        Vector3 center = transform.position;
        bool isPlayerDetectedNow =
            Mathf.Abs(player.position.x - center.x) <= detectRangeX &&
            Mathf.Abs(player.position.y - center.y) <= detectRangeY;

        // Phát hiện khi player vừa rời khỏi vùng phát hiện
        if (!isPlayerDetectedNow && previousPlayerDetected)
        {
            isReturning = true;
        }
        previousPlayerDetected = isPlayerDetectedNow;

        if (isPlayerDetectedNow)
        {
            isReturning = false;
            LookAtTarget(player.position.x);

            if (distToPlayer <= attackRange)
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
        // Chỉ về gốc nếu bật option returnToOrigin
        else if (isReturning && returnToOrigin)
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
                // Flip về trái khi đã về gốc
                Vector3 scale = transform.localScale;
                scale.x = -Mathf.Abs(scale.x);
                transform.localScale = scale;
            }
            // Hồi máu dần khi đã về gốc mà chưa đủ máu
            if (distToStart <= 0.15f && enemyHealth.currentHealth < enemyHealth.maxHealth)
            {
                enemyHealth.currentHealth += healthRegenRate * Time.deltaTime;
                enemyHealth.currentHealth = Mathf.Min(enemyHealth.currentHealth, enemyHealth.maxHealth);
            }
        }
        else // Đứng yên tại chỗ (nếu không về gốc)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isRunning", false);
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(detectRangeX * 2, detectRangeY * 2, 0.1f));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
