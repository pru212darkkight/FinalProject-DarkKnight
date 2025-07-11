using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Map3BossHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("Health Bar")]
    public GameObject healthBarPrefab;
    public Transform healthBarParent;
    public Vector3 healthBarOffset = new Vector3(0, 2f, 0);
    public bool createHealthBarOnStart = true;
    
    [Header("Visual Effects")]
    public bool showFlashEffect = true;
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;
    
    [Header("Death Settings")]
    public bool destroyOnDeath = false;
    public float destroyDelay = 3f;
    
    [Header("References")]
    public Map3BossController bossController;
    
    [Header("Debug")]
    public bool showDebug = false;
    
    // Private variables
    private bool isDead = false;
    private GameObject healthBarInstance;
    private Image healthBarFill;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    // Events
    public System.Action OnBossDeath;
    
    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;
        
        // Get components
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        // Get boss controller if not assigned
        if (bossController == null)
        {
            bossController = GetComponent<Map3BossController>();
        }
        
        // Create health bar
        if (createHealthBarOnStart)
        {
            CreateHealthBar();
        }
        
        if (showDebug) Debug.Log($"Map3BossHealth: Boss health initialized with {maxHealth} HP");
    }
    
    void Update()
    {
        // Update health bar position
        if (healthBarInstance != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + healthBarOffset);
            healthBarInstance.transform.position = screenPos;
        }
    }
    
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        currentHealth = Mathf.Max(0, currentHealth - damage);
        
        if (showDebug) Debug.Log($"Map3BossHealth: Boss took {damage} damage! Health: {currentHealth}/{maxHealth}");
        
        // Update health bar
        UpdateHealthBar();
        
        // Flash effect
        if (showFlashEffect && spriteRenderer != null)
        {
            StartCoroutine(FlashEffect());
        }
        
        // Boss controller will check health through this component
        // No need to sync currentHealth
        
        // Check if dead
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }
    
    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        if (showDebug) Debug.Log("Map3BossHealth: Boss died!");
        
        // Notify boss controller
        if (bossController != null)
        {
            bossController.Die();
        }
        
        // Hide health bar
        if (healthBarInstance != null)
        {
            healthBarInstance.SetActive(false);
        }
        
        // Trigger death event
        OnBossDeath?.Invoke();
        
        // Destroy after delay if requested
        if (destroyOnDeath)
        {
            StartCoroutine(DestroyAfterDelay());
        }
    }
    
    void CreateHealthBar()
    {
        // Try to find canvas if not assigned
        if (healthBarParent == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                healthBarParent = canvas.transform;
            }
            else
            {
                // Create a canvas
                GameObject canvasObj = new GameObject("BossHealthCanvas");
                Canvas newCanvas = canvasObj.AddComponent<Canvas>();
                newCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                newCanvas.sortingOrder = 10;
                canvasObj.AddComponent<GraphicRaycaster>();
                healthBarParent = canvasObj.transform;
            }
        }
        
        // Create health bar from prefab or create simple one
        if (healthBarPrefab != null && healthBarParent != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, healthBarParent);
            healthBarFill = healthBarInstance.GetComponent<Image>();
            
            if (healthBarFill == null)
            {
                // Try to find fill image in children
                Image[] images = healthBarInstance.GetComponentsInChildren<Image>();
                foreach (Image img in images)
                {
                    if (img.name.ToLower().Contains("fill") || img.type == Image.Type.Filled)
                    {
                        healthBarFill = img;
                        break;
                    }
                }
            }
        }
        else
        {
            // Create simple health bar
            CreateSimpleHealthBar();
        }
        
        // Update initial health bar
        UpdateHealthBar();
        
        if (showDebug) Debug.Log("Map3BossHealth: Health bar created");
    }
    
    void CreateSimpleHealthBar()
    {
        if (healthBarParent == null) return;
        
        // Create background
        GameObject bgObj = new GameObject("BossHealthBar_BG");
        bgObj.transform.SetParent(healthBarParent);
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = Color.black;
        
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.sizeDelta = new Vector2(200, 20);
        bgRect.anchoredPosition = Vector2.zero;
        
        // Create fill
        GameObject fillObj = new GameObject("BossHealthBar_Fill");
        fillObj.transform.SetParent(bgObj.transform);
        healthBarFill = fillObj.AddComponent<Image>();
        healthBarFill.color = Color.red;
        healthBarFill.type = Image.Type.Filled;
        healthBarFill.fillMethod = Image.FillMethod.Horizontal;
        
        RectTransform fillRect = fillObj.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        fillRect.anchoredPosition = Vector2.zero;
        
        healthBarInstance = bgObj;
    }
    
    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            float healthPercent = currentHealth / maxHealth;
            healthBarFill.fillAmount = healthPercent;
        }
    }
    
    IEnumerator FlashEffect()
    {
        if (spriteRenderer == null) yield break;
        
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }
    
    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
    
    // Public methods for external use
    public void Heal(float amount)
    {
        if (isDead) return;
        
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateHealthBar();
        
        if (showDebug) Debug.Log($"Map3BossHealth: Boss healed for {amount}. Health: {currentHealth}/{maxHealth}");
    }
    
    public void SetMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = maxHealth;
        UpdateHealthBar();
        
        if (showDebug) Debug.Log($"Map3BossHealth: Max health set to {maxHealth}");
    }
    
    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }
    
    public bool IsDead()
    {
        return isDead;
    }
    
    // Method for player attacks to call
    public void OnPlayerAttack(float damage = 10f)
    {
        TakeDamage(damage);
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw health bar position
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + healthBarOffset, new Vector3(2f, 0.3f, 0));
    }
}
