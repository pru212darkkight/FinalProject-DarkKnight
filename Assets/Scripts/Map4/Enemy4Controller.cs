using UnityEngine;

public class Enemy4Controller : MonoBehaviour
{
    public Transform player;
    public Transform pointA;
    public Transform pointB;

    [Header("Phạm vi phát hiện")]
    public float detectRange = 10f;     // phát hiện player
    public float idleRange = 3f;     // vùng gần thì Idle

    [Header("Tốc độ di chuyển")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 3f;

    private Vector3 currentTarget;
    private Animator animator;
    private bool facingRight = true;

    private enum State { Patrolling, Chasing, IdleNearPlayer }
    private State currentState = State.Patrolling;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentTarget = pointB.position;
    }

    void Update()
    {
        float xDistanceToPlayer = Mathf.Abs(transform.position.x - player.position.x);

        // 👉 Nếu trong vùng rất gần → IdleNearPlayer
        if (xDistanceToPlayer <= idleRange)
        {
            if (currentState != State.IdleNearPlayer)
            {
                currentState = State.IdleNearPlayer;
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", false);
                Debug.Log("Enemy: GẦN player → IDLE");
            }

            FlipTowards(player.position.x);
            return;
        }

        // 👉 Nếu trong vùng phát hiện → Chasing
        if (xDistanceToPlayer <= detectRange)
        {
            if (currentState != State.Chasing)
            {
                currentState = State.Chasing;
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", true);
                Debug.Log("Enemy: PHÁT HIỆN player → RUN");
            }

            MoveTowards(player.position, runSpeed);
            FlipTowards(player.position.x);
        }
        else
        {
            // 👉 Ngoài vùng → tuần tra
            if (currentState != State.Patrolling)
            {
                currentState = State.Patrolling;
                animator.SetBool("isWalking", true);
                animator.SetBool("isRunning", false);
                Debug.Log("Enemy: KHÔNG THẤY player → PATROL");
            }

            Patrol();
        }
    }

    void Patrol()
    {
        MoveTowards(currentTarget, walkSpeed);
        FlipTowards(currentTarget.x);

        if (Vector2.Distance(transform.position, currentTarget) < 0.1f)
        {
            currentTarget = (currentTarget == pointA.position) ? pointB.position : pointA.position;
        }
    }

    void MoveTowards(Vector3 target, float speed)
    {
        if (currentState == State.IdleNearPlayer) return;

        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    void FlipTowards(float targetX)
    {
        if (targetX > transform.position.x && !facingRight)
        {
            Flip();
        }
        else if (targetX < transform.position.x && facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // 👉 Vẽ vùng phát hiện và vùng dừng để dễ debug
    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, idleRange);
    }
}
