using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100;
    public float currentHealth = 100;
    public float armor = 5f;          // Giáp vật lý (%)
    public float magicResist = 5f;    // Kháng phép (%)
    public bool isDead = false;

    [SerializeField] private MovingPlatform movingPlatform; // Gán đúng platform ở Inspector

    private Animator animator;
    private Rigidbody2D rb;
    private MiniBossController miniBossController;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        miniBossController = GetComponent<MiniBossController>();

    }

    public void TakeDamage(float damage, bool isMagicDamage = false)
    {
        if (isDead) return;

        float finalDamage = damage;
        if (isMagicDamage)
            finalDamage *= (1 - (magicResist / 100f));
        else
            finalDamage *= (1 - (armor / 100f));

        currentHealth = Mathf.Max(0, currentHealth - finalDamage);
        if (animator != null) animator.SetTrigger("Hurt");

        // Bổ sung dòng sau để gọi controller xử lý state Hurt
        if (miniBossController != null)
        {
            miniBossController.OnTakeDamage();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void OnHurtAnimationEnd()
    {
        if (animator != null)
        {
            animator.ResetTrigger("Hurt");
            // animator.SetBool("Hurt", false); // Nếu bạn dùng bool thay trigger
        }
    }

    void Die()
    {
        isDead = true;
        if (animator != null)
        {
            animator.ResetTrigger("Hurt"); // Reset để không bị conflict với Death
            animator.SetTrigger("Death");  // Sử dụng Trigger thay vì Bool để tránh lỗi logic
        }
        if (rb != null) rb.linearVelocity = Vector2.zero;
        // KHÔNG Destroy ngay! Đợi animation xong sẽ gọi hàm Destroy qua Animation Event
    }

    // Gọi ở cuối animation Death bằng Animation Event
    public void OnDeathAnimationEnd()
    {
        if (movingPlatform != null)
            movingPlatform.UnlockPlatform();
        Invoke(nameof(DestroySelf), 0.3f); // Chờ 0.5 giây
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

}
