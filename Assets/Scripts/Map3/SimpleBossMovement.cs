using UnityEngine;

public class SimpleBossMovement : MonoBehaviour
{
    [Header("Test Settings")]
    public bool enableSimpleMovement = true;
    public float testSpeed = 3f;
    public bool moveTowardsPlayer = true;
    public float stopDistance = 2f;
    public float detectionRange = 8f; // Thêm detection range

    private Rigidbody2D rb;
    private Transform player;
    private Animator animator;
    private Map3BossController bossController;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bossController = GetComponent<Map3BossController>();

        // Tìm player theo tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (!enableSimpleMovement || player == null || rb == null) return;
        if (bossController != null)
        {
            // Dừng di chuyển nếu boss đã chết hoặc đang attack
            if (bossController.IsCurrentlyDead || bossController.IsCurrentlyAttacking) // Dùng property public của controller
            {
                StopMoving();
                return;
            }
        }

        if (moveTowardsPlayer)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance > detectionRange)
            {
                StopMoving();
                return;
            }

            if (distance <= stopDistance)
            {
                StopMoving();
                return;
            }

            // Move tới player
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * testSpeed, rb.linearVelocity.y);

            // Set animation
            if (animator != null) animator.SetBool("IsWalking", true);

            // Flip boss hướng về player
            if (direction.x > 0)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else if (direction.x < 0)
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

            // Debug movement

        }
    }

    void StopMoving()
    {
        // Giữ Y velocity (nếu có nhảy rơi gì đó)
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        if (animator != null) animator.SetBool("IsWalking", false);
    }

    [ContextMenu("Toggle Simple Movement")]
    public void ToggleSimpleMovement()
    {
        enableSimpleMovement = !enableSimpleMovement;
    }

    void OnDrawGizmosSelected()
    {
        // Draw detection range (vàng)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw stop distance (đỏ)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        // Draw line to player if moving
        if (player != null && enableSimpleMovement && moveTowardsPlayer)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance <= detectionRange && distance > stopDistance)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, player.position);
            }
        }
    }
}
