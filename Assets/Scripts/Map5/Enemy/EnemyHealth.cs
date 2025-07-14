using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100;
    public float currentHealth = 100;
    public float armor = 5f;          // Giáp vật lý (%)
    public float magicResist = 5f;    // Kháng phép (%)
    public bool isDead = false;

    [Header("Resurrection")]
    public bool canResurrect = false;        // Bật/tắt hồi sinh
    public bool hasResurrected = false;      // Đã từng hồi sinh chưa
    public float resurrectHealth = 50f;      // Máu sau khi hồi sinh
    public bool isInvulnerable = false;      // Miễn nhiễm sát thương khi true

    [SerializeField] private MovingPlatform movingPlatform; // Nếu cần unlock platform

    private Animator animator;
    private Rigidbody2D rb;
    private MiniBossController miniBossController;
    private FinalBossController finalBossController;
    private DemonRedController DemonRedController;

    [Header("Chest Drop Settings")]
    public bool enableChestDrop = false;      // Bật tắt chức năng rơi rương
    public GameObject chestPrefab;            // Prefab rương (kéo vào Inspector)
    public Vector3 chestSpawnOffset = Vector3.zero; // Offset spawn rương (nếu muốn lệch vị trí)
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        miniBossController = GetComponent<MiniBossController>();
        finalBossController = GetComponent<FinalBossController>();
        DemonRedController = GetComponent<DemonRedController>();
    }

    /// <summary>
    /// Gọi hàm này khi enemy bị nhận sát thương.
    /// </summary>
    public void TakeDamage(float damage, bool isMagicDamage = false)
    {
        // Không nhận sát thương khi đã chết hoặc đang bất tử (trong quá trình hồi sinh)
        if (isDead || isInvulnerable) return;

        float finalDamage = damage;
        if (isMagicDamage)
            finalDamage *= (1 - (magicResist / 100f));
        else
            finalDamage *= (1 - (armor / 100f));

        currentHealth = Mathf.Max(0, currentHealth - finalDamage);

        if (animator != null) animator.SetTrigger("Hurt");

        // Gọi controller nếu có
        if (miniBossController != null)
            miniBossController.OnTakeDamage();
        if (finalBossController != null)
            finalBossController.OnTakeDamage(finalDamage);

        // Kiểm tra điều kiện hồi sinh hoặc chết thật sự
        if (currentHealth <= 0 && !isDead)
        {
            if (canResurrect && !hasResurrected)
            {
                StartResurrect();
            }
            else
            {
                Die();
            }
        }
    }

    /// <summary>
    /// Bắt đầu quá trình hồi sinh (ngã, sau đó đứng dậy).
    /// </summary>
    void StartResurrect()
    {
        hasResurrected = true;
        isInvulnerable = true;   // Bật bất tử ngay khi bắt đầu quá trình hồi sinh
        isDead = true;           // Chặn AI, animation, di chuyển
        rb.linearVelocity = Vector2.zero;

        // Chuyển sang animation ngã (fallBack)
        if (animator != null)
        {
            animator.ResetTrigger("Hurt");
            animator.SetTrigger("fallBack");
        }
    }

    /// <summary>
    /// Gọi bằng Animation Event ở cuối animation "fallBack"
    /// </summary>
    public void OnFallBackEnd()
    {
        // Chuyển sang animation đứng dậy (standUp)
        if (animator != null)
            animator.SetTrigger("standUp");
        // Lưu ý: Vẫn giữ isInvulnerable = true ở đây!
    }

    /// <summary>
    /// Gọi bằng Animation Event ở cuối animation "standUp"
    /// </summary>
    public void OnStandUpEnd()
    {
        // Hồi lại máu, tắt trạng thái bất tử và chết
        currentHealth = resurrectHealth;
        isDead = false;

        // Chỉ **sau khi đứng dậy hoàn toàn** mới tắt bất tử
        isInvulnerable = false;

        if (animator != null)
        {
            animator.ResetTrigger("standUp");
        }
        // Gọi lại AI Controller nếu có
        if (DemonRedController != null)
            DemonRedController.OnResurrect();
    }

    /// <summary>
    /// Gọi ở cuối animation Hurt (Animation Event)
    /// </summary>
    public void OnHurtAnimationEnd()
    {
        if (animator != null)
            animator.ResetTrigger("Hurt");
    }

    /// <summary>
    /// Kết liễu enemy thật sự (không hồi sinh nữa).
    /// </summary>
    void Die()
    {
        isDead = true;
        CoinDrop coinDrop = GetComponent<CoinDrop>();
        if (coinDrop != null)
        {
            coinDrop.DropCoin();
        }
        if (animator != null)
        {
            animator.ResetTrigger("Hurt");
            animator.SetTrigger("Death");

        }
        if (rb != null) rb.linearVelocity = Vector2.zero;
        // Không destroy ngay, chờ Animation Event gọi OnDeathAnimationEnd
        // Thêm chức năng spawn rương
        if (enableChestDrop && chestPrefab != null)
        {
            // Spawn rương tại vị trí boss, cộng thêm offset nếu có
            Instantiate(chestPrefab, transform.position + chestSpawnOffset, Quaternion.identity);
        }
    }

    /// <summary>
    /// Gọi ở cuối animation Death (Animation Event)
    /// </summary>
    public void OnDeathAnimationEnd()
    {
        if (movingPlatform != null)
            movingPlatform.UnlockPlatform();
        Invoke(nameof(DestroySelf), 0.3f);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
