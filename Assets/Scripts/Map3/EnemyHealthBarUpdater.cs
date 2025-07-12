using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarUpdater : MonoBehaviour
{
    [Header("References")]
    public Image healthBarImage;
    public Map3EnemyHealth map3Health;
    public UnderwaterEnemyHealth fishHealth;
    public UnderwaterMine mineHealth;
    public EnemyWater waterEnemy;
    
    [Header("Settings")]
    public bool hideWhenFullHealth = false;
    public bool hideWhenDead = true;
    public float updateInterval = 0.1f; // Update every 0.1 seconds
    
    [Header("Colors")]
    public Color healthyColor = Color.green;
    public Color damagedColor = Color.yellow;
    public Color criticalColor = Color.red;
    public float criticalThreshold = 0.3f;
    public float damagedThreshold = 0.7f;
    
    private float lastUpdateTime;
    private float lastHealthPercent = 1f;
    
    void Start()
    {
        // Auto-find components if not assigned
        if (healthBarImage == null)
        {
            healthBarImage = GetComponent<Image>();
        }
        
        if (map3Health == null)
        {
            map3Health = GetComponentInParent<Map3EnemyHealth>();
        }

        if (fishHealth == null)
        {
            fishHealth = GetComponentInParent<UnderwaterEnemyHealth>();
        }

        if (mineHealth == null)
        {
            mineHealth = GetComponentInParent<UnderwaterMine>();
        }

        if (waterEnemy == null)
        {
            waterEnemy = GetComponentInParent<EnemyWater>();
        }
        
        // Initial update
        UpdateHealthBar();
    }
    
    void Update()
    {
        // Update at intervals to improve performance
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateHealthBar();
            lastUpdateTime = Time.time;
        }
    }
    
    private void UpdateHealthBar()
    {
        if (healthBarImage == null) return;
        
        float currentHealthPercent = GetCurrentHealthPercent();
        
        // Only update if health changed
        if (Mathf.Abs(currentHealthPercent - lastHealthPercent) > 0.01f)
        {
            // Update fill amount
            healthBarImage.fillAmount = currentHealthPercent;
            
            // Update color based on health
            UpdateHealthBarColor(currentHealthPercent);
            
            // Handle visibility
            UpdateVisibility(currentHealthPercent);
            
            lastHealthPercent = currentHealthPercent;
        }
    }
    
    private float GetCurrentHealthPercent()
    {
        if (map3Health != null)
        {
            return map3Health.HealthPercent;
        }
        else if (waterEnemy != null)
        {
            return waterEnemy.HealthPercent;
        }
        else if (fishHealth != null)
        {
            return fishHealth.currentHealth / fishHealth.maxHealth;
        }
        else if (mineHealth != null)
        {
            return mineHealth.currentHealth / mineHealth.maxHealth;
        }

        return 1f; // Default to full health
    }
    
    private bool IsEnemyDead()
    {
        if (map3Health != null)
        {
            return map3Health.IsDead;
        }
        else if (waterEnemy != null)
        {
            return waterEnemy.IsDead;
        }
        else if (fishHealth != null)
        {
            return fishHealth.isDead;
        }
        else if (mineHealth != null)
        {
            return mineHealth.isDead;
        }

        return false;
    }
    
    private void UpdateHealthBarColor(float healthPercent)
    {
        Color targetColor;
        
        if (healthPercent <= criticalThreshold)
        {
            targetColor = criticalColor;
        }
        else if (healthPercent <= damagedThreshold)
        {
            targetColor = damagedColor;
        }
        else
        {
            targetColor = healthyColor;
        }
        
        healthBarImage.color = targetColor;
    }
    
    private void UpdateVisibility(float healthPercent)
    {
        bool shouldShow = true;
        
        // Hide when full health if enabled
        if (hideWhenFullHealth && healthPercent >= 0.99f)
        {
            shouldShow = false;
        }
        
        // Hide when dead if enabled
        if (hideWhenDead && IsEnemyDead())
        {
            shouldShow = false;
        }
        
        // Update visibility
        if (gameObject.activeSelf != shouldShow)
        {
            gameObject.SetActive(shouldShow);
        }
    }
    
    // Public methods for manual control
    public void ForceUpdate()
    {
        UpdateHealthBar();
    }
    
    public void SetHealthBarImage(Image newHealthBarImage)
    {
        healthBarImage = newHealthBarImage;
        UpdateHealthBar();
    }
    
    public void SetMap3Health(Map3EnemyHealth newMap3Health)
    {
        map3Health = newMap3Health;
        UpdateHealthBar();
    }

    public void SetFishHealth(UnderwaterEnemyHealth newFishHealth)
    {
        fishHealth = newFishHealth;
        UpdateHealthBar();
    }

    public void SetMineHealth(UnderwaterMine newMineHealth)
    {
        mineHealth = newMineHealth;
        UpdateHealthBar();
    }

    public void SetWaterEnemy(EnemyWater newWaterEnemy)
    {
        waterEnemy = newWaterEnemy;
        UpdateHealthBar();
    }
    
    // Debug info
    void OnValidate()
    {
        if (Application.isPlaying)
        {
            UpdateHealthBar();
        }
    }
}
