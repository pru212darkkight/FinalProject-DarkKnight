using UnityEngine;
using UnityEngine.InputSystem;

public class UnderwaterPlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 0.5f;
    public LayerMask enemyLayer = -1;  // Layer của enemies
    public Vector2 attackOffset = Vector2.zero;  // Offset từ player

    [Header("Visual Effects")]
    public GameObject attackEffect;  // Effect khi attack
    public float effectDuration = 0.2f;

    private PlayerController1 playerController;
    private bool canAttack = true;
    private float lastAttackTime;
    private PlayerInput playerInput;

    private void Start()
    {
        playerController = GetComponent<PlayerController1>();
        if (playerController == null)
        {
            Debug.LogError("UnderwaterPlayerAttack requires PlayerController1 component!");
        }

        // Tìm PlayerInput component
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogWarning("PlayerInput not found, trying to find in parent...");
            playerInput = GetComponentInParent<PlayerInput>();
        }
    }

    private void Update()
    {
        HandleAttackInput();
    }

    void HandleAttackInput()
    {
        // Sử dụng Input System mới
        bool attackInput = false;

        if (playerInput != null)
        {
            // Thử các action có thể có
            try
            {
                attackInput = playerInput.actions["Attack"].WasPressedThisFrame() ||
                             playerInput.actions["Fire"].WasPressedThisFrame() ||
                             playerInput.actions["Submit"].WasPressedThisFrame();
            }
            catch
            {
                // Fallback: dùng Keyboard và Mouse trực tiếp
                attackInput = Keyboard.current.spaceKey.wasPressedThisFrame ||
                             Keyboard.current.xKey.wasPressedThisFrame ||
                             Mouse.current.leftButton.wasPressedThisFrame;
            }
        }
        else
        {
            // Fallback: dùng Keyboard và Mouse trực tiếp
            if (Keyboard.current != null && Mouse.current != null)
            {
                attackInput = Keyboard.current.spaceKey.wasPressedThisFrame ||
                             Keyboard.current.xKey.wasPressedThisFrame ||
                             Mouse.current.leftButton.wasPressedThisFrame;
            }
        }

        if (attackInput && canAttack)
        {
            PerformAttack();
        }

        // Update cooldown
        if (!canAttack && Time.time >= lastAttackTime + attackCooldown)
        {
            canAttack = true;
        }
    }

    void PerformAttack()
    {
        canAttack = false;
        lastAttackTime = Time.time;

        // Tính vị trí attack
        Vector2 attackPosition = (Vector2)transform.position + attackOffset;
        
        // Điều chỉnh attack position theo hướng player
        if (playerController != null)
        {
            // Giả sử player có scale.x âm khi quay trái
            bool facingRight = transform.localScale.x > 0;
            if (!facingRight)
            {
                attackOffset.x = -Mathf.Abs(attackOffset.x);
            }
            else
            {
                attackOffset.x = Mathf.Abs(attackOffset.x);
            }
            attackPosition = (Vector2)transform.position + attackOffset;
        }

        Debug.Log($"Player attacking at position: {attackPosition}");

        // Spawn attack effect
        if (attackEffect != null)
        {
            GameObject effect = Instantiate(attackEffect, attackPosition, Quaternion.identity);
            Destroy(effect, effectDuration);
        }

        // Tìm enemies trong tầm attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, attackRange, enemyLayer);
        
        foreach (Collider2D enemy in hitEnemies)
        {
            // Kiểm tra UnderwaterEnemyHealth
            UnderwaterEnemyHealth enemyHealth = enemy.GetComponent<UnderwaterEnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.OnPlayerAttack();
                Debug.Log($"Hit enemy: {enemy.name}");
                continue;
            }

            // Kiểm tra UnderwaterMine
            UnderwaterMine mine = enemy.GetComponent<UnderwaterMine>();
            if (mine != null)
            {
                mine.OnPlayerAttack();
                Debug.Log($"Hit mine: {enemy.name}");
                continue;
            }

            // Kiểm tra các enemy khác (HurtPlayer script)
            HurtPlayer hurtPlayer = enemy.GetComponent<HurtPlayer>();
            if (hurtPlayer != null)
            {
                // Nếu enemy có HurtPlayer nhưng không có health system, có thể destroy luôn
                Debug.Log($"Hit enemy with HurtPlayer: {enemy.name}");
                // Có thể thêm logic destroy enemy ở đây nếu cần
            }
        }

        // Trigger animation nếu có
        if (playerController != null)
        {
            Animator animator = playerController.GetComponent<Animator>();
            if (animator != null)
            {
                // Có thể trigger attack animation
                // animator.SetTrigger("Attack");
            }
        }
    }

    // Hàm public để script khác có thể gọi
    public void TriggerAttack()
    {
        if (canAttack)
        {
            PerformAttack();
        }
    }

    // Hiển thị attack range trong Scene view
    void OnDrawGizmosSelected()
    {
        Vector2 attackPosition = (Vector2)transform.position + attackOffset;
        
        // Điều chỉnh theo hướng player
        bool facingRight = transform.localScale.x > 0;
        Vector2 adjustedOffset = attackOffset;
        if (!facingRight)
        {
            adjustedOffset.x = -Mathf.Abs(adjustedOffset.x);
        }
        else
        {
            adjustedOffset.x = Mathf.Abs(adjustedOffset.x);
        }
        
        attackPosition = (Vector2)transform.position + adjustedOffset;
        
        Gizmos.color = canAttack ? Color.green : Color.red;
        Gizmos.DrawWireSphere(attackPosition, attackRange);
        
        // Vẽ line từ player đến attack position
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, attackPosition);
    }

    // Hàm để set attack range từ script khác
    public void SetAttackRange(float newRange)
    {
        attackRange = newRange;
    }

    // Hàm để set attack cooldown từ script khác
    public void SetAttackCooldown(float newCooldown)
    {
        attackCooldown = newCooldown;
    }

    // Hàm để check xem có thể attack không
    public bool CanAttack()
    {
        return canAttack;
    }

    // Hàm để get thời gian còn lại của cooldown
    public float GetCooldownRemaining()
    {
        if (canAttack) return 0f;
        return Mathf.Max(0f, (lastAttackTime + attackCooldown) - Time.time);
    }
}
