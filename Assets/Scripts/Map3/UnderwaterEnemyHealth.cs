using UnityEngine;
using UnityEngine.UI;

public class UnderwaterEnemyHealth : MonoBehaviour
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
        // Tạo health bar đơn giản bằng code
        GameObject healthBarObj = new GameObject("HealthBar");
        healthBarObj.transform.SetParent(transform);
        healthBarObj.transform.localPosition = healthBarOffset;

        // Tạo Canvas cho health bar
        Canvas canvas = healthBarObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        canvas.sortingOrder = 10;

        // Scale canvas nhỏ lại
        healthBarObj.transform.localScale = Vector3.one * 0.01f;

        // Tạo background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(healthBarObj.transform);
        bgObj.transform.localPosition = Vector3.zero;
        bgObj.transform.localScale = Vector3.one;

        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = Color.black;
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.sizeDelta = new Vector2(100, 10);

        // Tạo fill
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(bgObj.transform);
        fillObj.transform.localPosition = Vector3.zero;
        fillObj.transform.localScale = Vector3.one;

        healthBarFill = fillObj.AddComponent<Image>();
        healthBarFill.color = Color.red;
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

        // Ẩn health bar khi full health
        if (healthBarInstance != null)
        {
            healthBarInstance.SetActive(currentHealth < maxHealth);
        }
    }

    void UpdateVisualEffects()
    {
        if (spriteRenderer != null && !isDead)
        {
            // Tính toán alpha dựa trên health
            float healthPercent = currentHealth / maxHealth;
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(0.3f, 1f, healthPercent);  // Alpha từ 0.3 đến 1.0
            spriteRenderer.color = newColor;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");

        // Flash effect
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashEffect());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    System.Collections.IEnumerator FlashEffect()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
    }

    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        Debug.Log($"{gameObject.name} died!");

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

        // Disable components
        if (col != null) col.enabled = false;
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // Fade out và destroy
        StartCoroutine(FadeOutAndDestroy());
    }

    System.Collections.IEnumerator FadeOutAndDestroy()
    {
        float timer = 0f;
        Color startColor = spriteRenderer.color;
        
        while (timer < 1f)
        {
            timer += Time.deltaTime * fadeSpeed;
            if (spriteRenderer != null)
            {
                Color newColor = startColor;
                newColor.a = Mathf.Lerp(startColor.a, 0f, timer);
                spriteRenderer.color = newColor;
            }
            yield return null;
        }

        Destroy(gameObject);
    }

    // Hàm để player attack gọi
    public void OnPlayerAttack()
    {
        TakeDamage(1f);  // Mỗi đòn = 1 damage
    }
}
