using UnityEngine;

public class Map3EnemyDestroyer : MonoBehaviour
{
    [Header("References")]
    public EnemyHealth enemyHealth;  // Reference to Map 5's EnemyHealth
    public EnemyWater enemyWater;    // Reference to Underwater Diving's EnemyWater
    
    [Header("Settings")]
    public bool destroyOnDeath = true;
    public float destroyDelay = 0.1f;
    public bool showDebugLogs = true;
    
    private bool hasBeenDestroyed = false;
    
    void Start()
    {
        // Auto-find components if not assigned
        if (enemyHealth == null)
        {
            enemyHealth = GetComponent<EnemyHealth>();
        }
        
        if (enemyWater == null)
        {
            enemyWater = GetComponent<EnemyWater>();
        }
        
        if (showDebugLogs)
        {
            Debug.Log($"Map3EnemyDestroyer: Monitoring {gameObject.name} for death");
        }
    }
    
    void Update()
    {
        if (hasBeenDestroyed) return;
        
        // Check if enemy is dead from either script
        bool isDead = false;
        string deathSource = "";
        
        // Check EnemyHealth (Map 5)
        if (enemyHealth != null && enemyHealth.isDead)
        {
            isDead = true;
            deathSource = "EnemyHealth (Map 5)";
        }
        
        // Check EnemyWater (Underwater Diving)
        if (enemyWater != null && enemyWater.IsDead)
        {
            isDead = true;
            deathSource = "EnemyWater (Underwater Diving)";
        }
        
        // If dead, destroy the object
        if (isDead && destroyOnDeath)
        {
            DestroyEnemy(deathSource);
        }
    }
    
    void DestroyEnemy(string source)
    {
        if (hasBeenDestroyed) return;
        
        hasBeenDestroyed = true;
        
        if (showDebugLogs)
        {
            Debug.Log($"🔥 {gameObject.name} has been DESTROYED! (Death detected from: {source}) 🔥");
        }
        
        // Disable components to prevent further interactions
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) 
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }
        
        // Destroy the object
        Destroy(gameObject, destroyDelay);
    }
    
    // Public method to manually destroy enemy
    [ContextMenu("Force Destroy Enemy")]
    public void ForceDestroy()
    {
        DestroyEnemy("Manual Force");
    }
    
    // Public method to check if enemy should be destroyed
    public bool ShouldBeDestroyed()
    {
        if (enemyHealth != null && enemyHealth.isDead) return true;
        if (enemyWater != null && enemyWater.IsDead) return true;
        return false;
    }
    
    void OnDestroy()
    {
        if (showDebugLogs)
        {
            Debug.Log($"🔥 {gameObject.name} GameObject destroyed! 🔥");
        }
    }
}
