using UnityEngine;

public class Dead4Controller : MonoBehaviour
{
    public Transform player;
    public float detectRange = 8f;
    public float stopRange = 2f;
    public float runSpeed = 3f;

    private Animator animator;
    private Vector3 originPosition;
    private bool facingRight = true;

    private enum State { Idle, RunToPlayer, ReturnToOrigin }
    private State currentState = State.Idle;

    void Start()
    {
        animator = GetComponent<Animator>();
        originPosition = transform.position;
    }

    void Update()
    {
        if (!player) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float distanceToOrigin = Vector2.Distance(transform.position, originPosition);

        if (distanceToPlayer <= stopRange)
        {
            SetState(State.Idle);
            FlipTowards(player.position.x);
        }
        else if (distanceToPlayer <= detectRange)
        {
            SetState(State.RunToPlayer);
            MoveTowards(player.position);
        }
        else if (distanceToOrigin > 0.1f)
        {
            SetState(State.ReturnToOrigin);
            MoveTowards(originPosition);
        }
        else
        {
            SetState(State.Idle);
            FlipTowards(originPosition.x);
        }
    }

    void SetState(State newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        animator.SetBool("isRunning", currentState != State.Idle);
    }

    void MoveTowards(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        transform.position += dir * runSpeed * Time.deltaTime;
        FlipTowards(target.x);
    }

    void FlipTowards(float targetX)
    {
        bool shouldFaceRight = targetX > transform.position.x;
        if (shouldFaceRight != facingRight)
        {
            facingRight = shouldFaceRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
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
