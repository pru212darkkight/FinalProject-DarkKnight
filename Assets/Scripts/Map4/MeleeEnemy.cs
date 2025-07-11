using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    public Transform player;
    public Transform pointA;
    public Transform pointB;

    [Header("Thuộc tính")]
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 3f;
    public float detectRange = 6f;
    public float stopRange = 1.2f;
    public float attackCooldown = 1.5f;
    public float attackDamage = 20f;

    private float lastAttackTime = -Mathf.Infinity;
    private Transform currentTarget;
    private Animator animator;
    private Rigidbody2D rb;
    private bool facingRight = true;

    private enum State { Patrol, Chase, StopNearPlayer }
    private State currentState = State.Patrol;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentTarget = pointB;
    }

    void Update()
    {
        if (!player) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer <= stopRange)
        {
            HandleAttack();
        }
        else if (distToPlayer <= detectRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        Flip(rb.linearVelocity.x); // Dùng linearVelocity nếu Unity báo cần nâng cấp
    }

    void HandleAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }

        animator.SetBool("Run", false);
        rb.linearVelocity = Vector2.zero;
        currentState = State.StopNearPlayer;
    }

    void ChasePlayer()
    {
        currentState = State.Chase;
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(dir.x * chaseSpeed, rb.linearVelocity.y);
        animator.SetBool("Run", true);
    }

    void Patrol()
    {
        currentState = State.Patrol;

        Vector2 dir = (currentTarget.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(dir.x * patrolSpeed, rb.linearVelocity.y);
        animator.SetBool("Run", true);

        Debug.DrawLine(transform.position, currentTarget.position, Color.green);

        // Dùng khoảng cách lớn hơn để tránh kẹt khi enemy không chạm chính xác
        if ((currentTarget.position - transform.position).sqrMagnitude < 0.2f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
    }

    void Flip(float moveX)
    {
        if ((moveX > 0 && !facingRight) || (moveX < 0 && facingRight))
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    // Gọi từ animation event
    public void DealDamage()
    {
        if (!player) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= stopRange + 0.3f)
        {
            PlayerController1 pc = player.GetComponent<PlayerController1>();
            if (pc != null)
            {
                pc.TakeDamage(attackDamage, false);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopRange);
    }
}
