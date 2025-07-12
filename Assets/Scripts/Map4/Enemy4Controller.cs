using UnityEngine;

public class Enemy4Controller : MonoBehaviour
{
    public Transform player;
    public Transform pointA;
    public Transform pointB;

    [Header("Phạm vi phát hiện")]
    public float detectRange = 10f;    // Bán kính phát hiện Player
    public float idleRange = 3f;       // Bán kính đứng yên gần Player

    [Header("Tốc độ di chuyển")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 3f;

    private Transform currentTarget;   // Điểm tuần tra hiện tại
    private Animator animator;
    private bool facingRight = true;

    private enum State { Patrolling, Chasing, IdleNearPlayer }
    private State currentState = State.Patrolling;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentTarget = pointB;
    }

    void Update()
    {
        if (!player) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= idleRange)
        {
            // IDLE khi quá gần Player
            if (currentState != State.IdleNearPlayer)
            {
                currentState = State.IdleNearPlayer;
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", false);
                Debug.Log("Enemy: GẦN player → IDLE");
            }

            FlipTowards(player.position.x);
        }
        else if (distanceToPlayer <= detectRange)
        {
            // CHASE khi trong vùng phát hiện
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
            // PATROL khi Player ngoài vùng phát hiện
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
        MoveTowards(currentTarget.position, walkSpeed);
        FlipTowards(currentTarget.position.x);

        if (Vector2.Distance(transform.position, currentTarget.position) < 0.2f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
            Debug.Log("Enemy: Chuyển hướng tuần tra sang " + currentTarget.name);
        }
    }

    void MoveTowards(Vector3 target, float speed)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    void FlipTowards(float targetX)
    {
        if ((targetX > transform.position.x && !facingRight) ||
            (targetX < transform.position.x && facingRight))
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

    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, idleRange);
    }
}
