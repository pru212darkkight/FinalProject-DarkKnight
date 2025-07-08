using UnityEngine;

public class MiniBossController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public float detectRangeX = 5f;
    public float detectRangeY = 2f;
    public float attackRange = 1.5f;
    public float moveSpeed = 2f;
    public float healthRegenRate = 3f;

    [Header("AI Options")]
    public bool returnToOrigin = true;

    [HideInInspector] public Vector3 startPoint;
    private Rigidbody2D rb;
    private Animator animator;
    private EnemyHealth enemyHealth;
    public MiniBossAttack miniBossAttack;

    // Thêm trạng thái Hurt
    private enum State { Idle, MovingToPlayer, Attacking, Hurt, Returning }
    private State state = State.Idle;
    private State stateBeforeHurt = State.Idle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        miniBossAttack = GetComponent<MiniBossAttack>();
        startPoint = transform.position;
    }

    void Update()
    {
        // Nếu đang Hurt thì không làm gì khác
        if (state == State.Hurt)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsRunning", false);
            return;
        }

        if (enemyHealth != null && enemyHealth.isDead)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsRunning", false);
            return;
        }

        bool playerDetectedNow = Mathf.Abs(player.position.x - transform.position.x) <= detectRangeX &&
                                 Mathf.Abs(player.position.y - transform.position.y) <= detectRangeY;

        switch (state)
        {
            case State.Idle:
                animator.SetBool("IsRunning", false);
                if (playerDetectedNow)
                    state = State.MovingToPlayer;
                else
                    RegenerateHealth();
                break;

            case State.MovingToPlayer:
                if (!playerDetectedNow)
                {
                    if (returnToOrigin)
                        state = State.Returning;
                    else
                        state = State.Idle;
                    break;
                }
                MoveToPlayer();
                break;

            case State.Attacking:
                if (!playerDetectedNow)
                {
                    state = State.Returning;
                }
                // Đợi animation gọi EndAttack() để chuyển state
                break;

            case State.Returning:
                float distToStart = Vector2.Distance(transform.position, startPoint);
                if (distToStart > 0.1f)
                {
                    LookAtTarget(startPoint.x);
                    Vector2 newPos = Vector2.MoveTowards(rb.position, startPoint, moveSpeed * Time.deltaTime);
                    rb.MovePosition(newPos);
                    animator.SetBool("IsRunning", true);
                }
                else
                {
                    animator.SetBool("IsRunning", false);
                    state = State.Idle;
                }
                RegenerateHealth();
                break;
        }
    }

    void MoveToPlayer()
    {
        float distToPlayer = Vector2.Distance(transform.position, player.position);
        LookAtTarget(player.position.x);

        if (distToPlayer > attackRange)
        {
            // Tiến về phía player
            Vector2 target = new Vector2(player.position.x, rb.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, moveSpeed * Time.deltaTime);
            rb.MovePosition(newPos);
            animator.SetBool("IsRunning", true);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsRunning", false);
            // Đổi state sang tấn công và gọi hàm attack của MiniBossAttack
            state = State.Attacking;
            miniBossAttack.RandomAttack();
        }
    }

    public void EndAttack() // Gọi từ Animation Event khi animation attack kết thúc
    {
        if (state != State.Attacking) return; // Đề phòng trường hợp bị hurt và đã chuyển state

        if (Mathf.Abs(player.position.x - transform.position.x) <= detectRangeX &&
            Mathf.Abs(player.position.y - transform.position.y) <= detectRangeY)
        {
            state = State.MovingToPlayer;
        }
        else
        {
            if (returnToOrigin)
                state = State.Returning;
            else
                state = State.Idle;
        }
    }

    public void OnTakeDamage() // Gọi khi bị đánh, có thể gọi từ EnemyHealth hoặc Animation Event đầu Hurt
    {
        if (state == State.Hurt) return; // Đang hurt thì thôi

        stateBeforeHurt = state;
        state = State.Hurt;
        animator.SetTrigger("Hurt");
        rb.linearVelocity = Vector2.zero;
    }

    public void OnHurtEnd() // Gọi ở cuối animation Hurt (Animation Event)
    {
        // Quay lại đúng trạng thái trước khi bị Hurt
        if (enemyHealth != null && enemyHealth.isDead) return;
        if (stateBeforeHurt == State.Attacking)
        {
            state = State.Attacking;
            miniBossAttack.RandomAttack(); // Có thể gọi lại tấn công nếu bị ngắt giữa chừng
        }
        else if (stateBeforeHurt == State.MovingToPlayer)
        {
            state = State.MovingToPlayer;
        }
        else if (stateBeforeHurt == State.Returning)
        {
            if (returnToOrigin)
                state = State.Returning;
            else
                state = State.Idle;
        }
        else
        {
            state = State.Idle;
        }
    }

    void RegenerateHealth()
    {
        if (enemyHealth.currentHealth < enemyHealth.maxHealth)
        {
            enemyHealth.currentHealth += healthRegenRate * Time.deltaTime;
            enemyHealth.currentHealth = Mathf.Min(enemyHealth.currentHealth, enemyHealth.maxHealth);
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

        // Vẽ thêm vùng attack range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}
