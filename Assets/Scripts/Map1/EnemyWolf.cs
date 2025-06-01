using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Basic Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float damage = 10f;
    public float moveSpeed = 3f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float detectionRange = 5f;

    [Header("Defense Stats")]
    public float armor = 5f;          // Giáp
    public float magicResist = 5f;    // Kháng phép

    [Header("UI")]
    public Image healthBar;           // UI health bar (nếu có)

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    // Components
    protected Rigidbody2D rb;
    protected Animator animator;
    protected Transform player;
    protected bool isFacingRight = true;
    protected float lastAttackTime;
    protected bool isGrounded;
    protected bool isDead = false;

    // Animation parameters
    protected readonly int SpeedHash = Animator.StringToHash("Speed");
    protected readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    protected readonly int AttackHash = Animator.StringToHash("Attack");
    protected readonly int HurtHash = Animator.StringToHash("Hurt");
    protected readonly int DieHash = Animator.StringToHash("Death");

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentHealth = maxHealth;
        UpdateHealthBar();

        // Set initial facing direction based on player position
        if (player != null)
        {
            isFacingRight = player.position.x > transform.position.x;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (isFacingRight ? -1 : 1);
            transform.localScale = scale;
        }
    }

    protected virtual void Update()
    {
        if (isDead || player == null) return;

        // Kiểm tra mặt đất
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (animator != null)
        {
            animator.SetBool(IsGroundedHash, isGrounded);
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Debug.Log($"Distance to player: {distanceToPlayer}, Detection Range: {detectionRange}");

        // Nếu player trong tầm phát hiện
        if (distanceToPlayer <= detectionRange)
        {
            // Xác định hướng di chuyển
            float direction = player.position.x > transform.position.x ? 1 : -1;
            Debug.Log($"Direction to player: {direction}");

            // Lật sprite nếu cần
            if (direction > 0 && !isFacingRight)
            {
                Flip();
            }
            else if (direction < 0 && isFacingRight)
            {
                Flip();
            }

            // Nếu trong tầm tấn công
            if (distanceToPlayer <= attackRange)
            {
                // Dừng di chuyển
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                if (animator != null)
                {
                    animator.SetFloat(SpeedHash, 0);
                }

                // Tấn công nếu đã hết cooldown
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack();
                }
            }
            else
            {
                // Di chuyển về phía player
                Vector2 newVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
                rb.linearVelocity = newVelocity;
                Debug.Log($"Moving with velocity: {newVelocity}");

                if (animator != null)
                {
                    animator.SetFloat(SpeedHash, Mathf.Abs(newVelocity.x));
                }
            }
        }
        else
        {
            // Dừng di chuyển nếu player ngoài tầm
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (animator != null)
            {
                animator.SetFloat(SpeedHash, 0);
            }
        }
    }

    protected virtual void Attack()
    {
        lastAttackTime = Time.time;
        if (animator != null)
        {
            animator.SetTrigger(AttackHash);
        }

        // Kiểm tra va chạm với player
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D hitPlayer in hitPlayers)
        {
            if (hitPlayer.CompareTag("Player"))
            {
                PlayerController1 playerController = hitPlayer.GetComponent<PlayerController1>();
                if (playerController != null)
                {
                    playerController.TakeDamage(damage);
                }
                break;
            }
        }
    }

    public virtual void TakeDamage(float damage, bool isMagicDamage = false)
    {
        if (isDead) return;

        float finalDamage = damage;
        if (isMagicDamage)
        {
            finalDamage *= (1 - (magicResist / 100f)); // Giảm sát thương phép thuật dựa trên kháng phép
        }
        else
        {
            finalDamage *= (1 - (armor / 100f)); // Giảm sát thương vật lý dựa trên giáp
        }

        currentHealth = Mathf.Max(0, currentHealth - finalDamage);
        UpdateHealthBar();

        // Trigger hurt animation nếu có
        if (animator != null)
        {
            animator.SetTrigger(HurtHash);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        
        // Trigger death animation nếu có
        if (animator != null)
        {
            animator.SetTrigger(DieHash);
        }

        // Disable các component không cần thiết
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // Destroy sau khi animation kết thúc
        Destroy(gameObject, 2f);
    }

    protected virtual void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (isFacingRight ? -1 : 1);
        transform.localScale = scale;
        Debug.Log($"Flipped: isFacingRight = {isFacingRight}, scale.x = {scale.x}");
    }

    protected virtual void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        // Vẽ tầm phát hiện
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Vẽ tầm tấn công
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Vẽ kiểm tra mặt đất
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
} 