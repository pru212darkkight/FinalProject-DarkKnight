using UnityEngine;
using UnityEngine.UI;

public class UnderwaterMine : MonoBehaviour
{
    [Header("Mine Settings")]
    public float maxHealth = 2f;  // Mine chết sau 2 đòn
    public float currentHealth;
    public float explosionDamage = 5f;  // Damage khi nổ
    public float explosionRadius = 2f;  // Bán kính nổ
    public bool isDead = false;

    [Header("Visual Effects")]
    public float fadeSpeed = 2f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Health Bar")]
    public Vector3 healthBarOffset = new Vector3(0, 1f, 0);
    private GameObject healthBarInstance;
    private Image healthBarFill;

    [Header("Explosion Effects")]
    public GameObject explosionEffect;  // Effect nổ
    public float explosionForce = 500f;  // Lực đẩy
    
    [Header("Trigger Settings")]
    public bool explodeOnContact = true;  // Nổ khi player đụng vào
    public float contactDamage = 5f;  // Damage khi đụng

    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D col;
    private bool hasExploded = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        
        currentHealth = maxHealth;
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void Start()
    {
        CreateHealthBar();
    }

    private void Update()
    {
        UpdateHealthBar();
        UpdateVisualEffects();
    }

    void CreateHealthBar()
    {
        // Tạo health bar tương tự như UnderwaterEnemyHealth
        GameObject healthBarObj = new GameObject("HealthBar");
        healthBarObj.transform.SetParent(transform);
        healthBarObj.transform.localPosition = healthBarOffset;

        Canvas canvas = healthBarObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        canvas.sortingOrder = 10;

        healthBarObj.transform.localScale = Vector3.one * 0.01f;

        // Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(healthBarObj.transform);
        bgObj.transform.localPosition = Vector3.zero;
        bgObj.transform.localScale = Vector3.one;

        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = Color.black;
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.sizeDelta = new Vector2(100, 10);

        // Fill (màu vàng cho mine)
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(bgObj.transform);
        fillObj.transform.localPosition = Vector3.zero;
        fillObj.transform.localScale = Vector3.one;

        healthBarFill = fillObj.AddComponent<Image>();
        healthBarFill.color = Color.yellow;  // Màu vàng cho mine
        healthBarFill.type = Image.Type.Filled;
        healthBarFill.fillMethod = Image.FillMethod.Horizontal;
        
        RectTransform fillRect = fillObj.GetComponent<RectTransform>();
        fillRect.sizeDelta = new Vector2(100, 10);
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        healthBarInstance = healthBarObj;
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }

        if (healthBarInstance != null)
        {
            healthBarInstance.SetActive(currentHealth < maxHealth);
        }
    }

    void UpdateVisualEffects()
    {
        if (spriteRenderer != null && !isDead)
        {
            float healthPercent = currentHealth / maxHealth;
            Color newColor = originalColor;
            
            // Mine nhấp nháy khi sắp nổ
            if (healthPercent <= 0.5f)
            {
                float blink = Mathf.Sin(Time.time * 10f) * 0.5f + 0.5f;
                newColor.r = Mathf.Lerp(1f, 0.5f, blink);
            }
            
            newColor.a = Mathf.Lerp(0.3f, 1f, healthPercent);
            spriteRenderer.color = newColor;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead || hasExploded) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        
        Debug.Log($"Mine took {damage} damage. Health: {currentHealth}/{maxHealth}");

        // Flash effect
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashEffect());
        }

        if (currentHealth <= 0)
        {
            Explode();
        }
    }

    System.Collections.IEnumerator FlashEffect()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;  // Flash đỏ cho mine
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (explodeOnContact && other.CompareTag("Player") && !hasExploded)
        {
            // Gây damage ngay khi đụng
            PlayerController1 player = other.GetComponent<PlayerController1>();
            if (player != null)
            {
                player.TakeDamage(contactDamage);
                Debug.Log($"Mine contact damage: {contactDamage}");
            }
            
            Explode();
        }
    }

    public void Explode()
    {
        if (hasExploded) return;
        
        hasExploded = true;
        isDead = true;
        
        Debug.Log("Mine exploded!");

        // Ẩn health bar
        if (healthBarInstance != null)
        {
            healthBarInstance.SetActive(false);
        }

        // Spawn explosion effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, transform.rotation);
        }

        // Gây damage cho player trong bán kính nổ
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hitObjects)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerController1 player = hit.GetComponent<PlayerController1>();
                if (player != null)
                {
                    player.TakeDamage(explosionDamage);
                    Debug.Log($"Explosion damage: {explosionDamage}");
                    
                    // Đẩy player ra xa
                    Rigidbody2D playerRb = hit.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        Vector2 direction = (hit.transform.position - transform.position).normalized;
                        playerRb.AddForce(direction * explosionForce);
                    }
                }
            }
        }

        // Disable components và destroy
        if (col != null) col.enabled = false;
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // Destroy ngay (có thể thêm delay nếu muốn)
        Destroy(gameObject, 0.1f);
    }

    // Hàm để player attack gọi
    public void OnPlayerAttack()
    {
        TakeDamage(1f);  // Mỗi đòn = 1 damage
    }

    // Hiển thị explosion radius trong Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
