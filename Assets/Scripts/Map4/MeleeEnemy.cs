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
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
            }
            animator.SetBool("Run", false);
            rb.linearVelocity = Vector2.zero;
            currentState = State.StopNearPlayer;
        }
        else if (distToPlayer <= detectRange)
        {
            currentState = State.Chase;
            Vector2 dir = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(dir.x * chaseSpeed, rb.linearVelocity.y);
            animator.SetBool("Run", true);
        }
        else
        {
            currentState = State.Patrol;
            Patrol();
        }

        Flip(rb.linearVelocity.x);
    }

    void Patrol()
    {
        Vector2 dir = (currentTarget.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(dir.x * patrolSpeed, rb.linearVelocity.y);
        animator.SetBool("Run", true);

        if (Vector2.Distance(transform.position, currentTarget.position) < 0.1f)
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

    // 👉 Gọi từ Animation Event
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
