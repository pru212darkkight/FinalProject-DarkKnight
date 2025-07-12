using UnityEngine;
using UnityEngine.UI;

public class Map3EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 4f;  // Con cá chết sau 4 đòn
    public float currentHealth;
    public bool isDead = false;

    [Header("Visual Effects")]
    public float fadeSpeed = 2f;  // Tốc độ mất màu
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Health Bar")]
    public GameObject healthBarPrefab;  // Prefab health bar
    public Vector3 healthBarOffset = new Vector3(0, 1f, 0);
    private GameObject healthBarInstance;
    private Image healthBarFill;

    [Header("Death Effects")]
    public GameObject deathEffect;  // Effect khi chết (optional)

    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D col;

    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;
        
        // Get components
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        Debug.Log($"Map3EnemyHealth: {gameObject.name} initialized with {maxHealth} health");
    }

    void Update()
    {
        // Update health bar position if exists
        if (healthBarInstance != null)
        {
            healthBarInstance.transform.position = Camera.main.WorldToScreenPoint(transform.position + healthBarOffset);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"Map3EnemyHealth: {gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");

        // Update health bar
        UpdateHealthBar();

        // Flash effect
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashEffect());
        }

        // Check if dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    System.Collections.IEnumerator FlashEffect()
    {
        if (spriteRenderer == null) yield break;

        Color flashColor = Color.red;
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        Debug.Log($"🔥 {gameObject.name} has been DESTROYED! 🔥");

        // Drop coins if CoinDrop component exists
        CoinDrop coinDrop = GetComponent<CoinDrop>();
        if (coinDrop != null)
        {
            coinDrop.DropCoin();
        }

        // Ẩn health bar
        if (healthBarInstance != null)
        {
            healthBarInstance.SetActive(false);
        }

        // Spawn death effect
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, transform.rotation);
        }

        // Disable components immediately
        if (col != null) col.enabled = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // Destroy immediately - Map 3 specific behavior
        Destroy(gameObject, 0.1f);
    }

    // Hàm để player attack gọi
    public void OnPlayerAttack()
    {
        TakeDamage(1f);  // Mỗi đòn đánh = 1 damage
    }

    // Create health bar
    public void CreateHealthBar(GameObject healthBarPrefab, Transform parentCanvas)
    {
        if (healthBarInstance != null) return;

        this.healthBarPrefab = healthBarPrefab;
        
        if (healthBarPrefab != null && parentCanvas != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, parentCanvas);
            healthBarFill = healthBarInstance.GetComponent<Image>();
            
            if (healthBarFill != null)
            {
                healthBarFill.color = Color.red;
                UpdateHealthBar();
            }
        }
    }

    // Public properties
    public float HealthPercent => currentHealth / maxHealth;
    public bool IsDead => isDead;

    void OnDestroy()
    {
        Debug.Log($"🔥 {gameObject.name} has been DESTROYED! 🔥");
        
        // Clean up health bar
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance);
        }
    }
}
