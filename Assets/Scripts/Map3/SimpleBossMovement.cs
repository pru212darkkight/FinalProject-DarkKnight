using UnityEngine;

public class SimpleBossMovement : MonoBehaviour
{
    [Header("Test Settings")]
    public bool enableSimpleMovement = true; // Enable by default
    public float testSpeed = 3f;
    public bool moveTowardsPlayer = true;
    public float stopDistance = 2f; // Stop when close to player for attacks

    private Rigidbody2D rb;
    private Transform player;
    private Animator animator;
    private Map3BossController bossController;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bossController = GetComponent<Map3BossController>();

        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log("SimpleBossMovement: Player found!");
        }
        else
        {
            Debug.LogError("SimpleBossMovement: Player not found!");
        }
    }
    
    void Update()
    {
        if (!enableSimpleMovement || player == null || rb == null) return;

        // Check if boss is dead or attacking
        bool isDead = false;
        bool isAttacking = false;

        if (bossController != null)
        {
            // Use reflection to get private fields
            var isDeadField = typeof(Map3BossController).GetField("isDead",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var isAttackingField = typeof(Map3BossController).GetField("isAttacking",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (isDeadField != null) isDead = (bool)isDeadField.GetValue(bossController);
            if (isAttackingField != null) isAttacking = (bool)isAttackingField.GetValue(bossController);
        }

        // Don't move if dead
        if (isDead) return;

        if (moveTowardsPlayer)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            // Stop moving if too close (let boss attack) or if attacking
            if (distance <= stopDistance || isAttacking)
            {
                // Stop movement
                Vector2 stopVelocity = rb.linearVelocity;
                stopVelocity.x = 0;
                rb.linearVelocity = stopVelocity;

                // Stop walking animation
                if (animator != null)
                {
                    animator.SetBool("IsWalking", false);
                }

                return;
            }

            // Move towards player
            Vector2 direction = (player.position - transform.position).normalized;
            Vector2 moveVelocity = rb.linearVelocity;
            moveVelocity.x = direction.x * testSpeed;
            rb.linearVelocity = moveVelocity;

            // Set walking animation
            if (animator != null)
            {
                animator.SetBool("IsWalking", true);
            }

            // Face player
            if (direction.x > 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (direction.x < 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }

            if (Time.frameCount % 60 == 0)
            {
                Debug.Log($"Simple Boss Movement - Distance: {distance:F2}, Velocity: {rb.linearVelocity}, Attacking: {isAttacking}");
            }
        }
    }
    
    [ContextMenu("Toggle Simple Movement")]
    public void ToggleSimpleMovement()
    {
        enableSimpleMovement = !enableSimpleMovement;
        Debug.Log($"Simple Boss Movement: {(enableSimpleMovement ? "Enabled" : "Disabled")}");
    }
}
