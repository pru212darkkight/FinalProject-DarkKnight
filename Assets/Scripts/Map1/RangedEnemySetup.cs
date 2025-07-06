//using UnityEngine;

//public class RangedEnemySetup : MonoBehaviour
//{
//    [Header("Setup Instructions")]
//    [TextArea(10, 20)]
//    public string setupInstructions = @"
//=== RANGED ENEMY SETUP INSTRUCTIONS ===

//1. CREATE ENEMY GAMEOBJECT:
//   - Create empty GameObject
//   - Add SpriteRenderer component
//   - Add Animator component
//   - Add Rigidbody2D component (set to Kinematic)
//   - Add Collider2D component (BoxCollider2D or CircleCollider2D)
//   - Add RangedEnemy script

//2. SETUP ANIMATOR:
//   - Create Animator Controller
//   - Add parameters: Idle (bool), Attack (trigger), Hurt (trigger), Die (trigger)
//   - Create states: Idle, Attack, Hurt, Die
//   - Set Idle as default state
//   - Add transitions between states

//3. SETUP PROJECTILE:
//   - Create projectile GameObject
//   - Add SpriteRenderer
//   - Add Rigidbody2D (set to Kinematic)
//   - Add Collider2D (set as Trigger)
//   - Add Projectile script
//   - Set tag to 'Projectile' or 'EnemyProjectile'

//4. CONFIGURE RANGED ENEMY:
//   - Assign projectile prefab
//   - Set firePoint (child GameObject or self)
//   - Adjust ranges and cooldowns
//   - Set animation speeds

//5. TAGS AND LAYERS:
//   - Set enemy tag to 'Enemy'
//   - Set projectile tag appropriately
//   - Configure collision layers

//6. TESTING:
//   - Ensure player has 'Player' tag
//   - Check detection and attack ranges
//   - Verify projectile behavior

//=== END SETUP INSTRUCTIONS ===";

//    [Header("Quick Setup")]
//    public bool autoSetup = false;
//    public GameObject projectilePrefab;
//    public Transform firePoint;
    
//    void Start()
//    {
//        if (autoSetup)
//        {
//            SetupRangedEnemy();
//        }
//    }
    
//    [ContextMenu("Setup Ranged Enemy")]
//    public void SetupRangedEnemy()
//    {
//        Debug.Log("Setting up Ranged Enemy...");
        
//        // Check if RangedEnemy component exists
//        RangedEnemy rangedEnemy = GetComponent<RangedEnemy>();
//        if (rangedEnemy == null)
//        {
//            Debug.LogError("RangedEnemy component not found! Please add RangedEnemy script first.");
//            return;
//        }
        
//        // Check required components
//        CheckRequiredComponents();
        
//        // Setup projectile if provided
//        if (projectilePrefab != null)
//        {
//            rangedEnemy.projectilePrefab = projectilePrefab;
//            Debug.Log("Projectile prefab assigned.");
//        }
        
//        // Setup fire point if provided
//        if (firePoint != null)
//        {
//            rangedEnemy.firePoint = firePoint;
//            Debug.Log("Fire point assigned.");
//        }
        
//        Debug.Log("Ranged Enemy setup completed!");
//    }
    
//    private void CheckRequiredComponents()
//    {
//        // Check SpriteRenderer
//        if (GetComponent<SpriteRenderer>() == null)
//        {
//            Debug.LogWarning("Missing SpriteRenderer component!");
//        }
        
//        // Check Animator
//        if (GetComponent<Animator>() == null)
//        {
//            Debug.LogWarning("Missing Animator component!");
//        }
        
//        // Check Rigidbody2D
//        Rigidbody2D rb = GetComponent<Rigidbody2D>();
//        if (rb == null)
//        {
//            Debug.LogWarning("Missing Rigidbody2D component!");
//        }
//        else if (rb.bodyType != RigidbodyType2D.Kinematic)
//        {
//            Debug.LogWarning("Rigidbody2D should be set to Kinematic for ranged enemies!");
//        }
        
//        // Check Collider2D
//        if (GetComponent<Collider2D>() == null)
//        {
//            Debug.LogWarning("Missing Collider2D component!");
//        }
        
//        // Check tag
//        if (gameObject.tag != "Enemy")
//        {
//            Debug.LogWarning("Enemy should have 'Enemy' tag!");
//        }
//    }
    
//    [ContextMenu("Create Projectile Prefab")]
//    public void CreateProjectilePrefab()
//    {
//        Debug.Log("Creating projectile prefab...");
        
//        // Create projectile GameObject
//        GameObject projectile = new GameObject("EnemyProjectile");
        
//        // Add components
//        projectile.AddComponent<SpriteRenderer>();
//        projectile.AddComponent<Rigidbody2D>();
//        projectile.AddComponent<CircleCollider2D>();
//        projectile.AddComponent<Projectile>();
        
//        // Configure components
//        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
//        projectileRb.bodyType = RigidbodyType2D.Kinematic;
//        projectileRb.gravityScale = 0f;
        
//        CircleCollider2D projectileCollider = projectile.GetComponent<CircleCollider2D>();
//        projectileCollider.isTrigger = true;
//        projectileCollider.radius = 0.1f;
        
//        // Set tag
//        projectile.tag = "EnemyProjectile";
        
//        // Set layer
//        projectile.layer = LayerMask.NameToLayer("EnemyProjectile");
        
//        // Configure Projectile component
//        Projectile projectileScript = projectile.GetComponent<Projectile>();
//        if (projectileScript != null)
//        {
//            projectileScript.isEnemyProjectile = true;
//            projectileScript.hitLayers = LayerMask.GetMask("Player", "Ground", "Wall");
//            projectileScript.projectileColor = Color.red;
//        }
        
//        Debug.Log("Projectile prefab created! Please assign a sprite and save as prefab.");
//        Debug.Log("IMPORTANT: Make sure Player is on 'Player' layer and Ground/Walls are on appropriate layers!");
//        Debug.Log("PROJECTILE SETUP TIPS:");
//        Debug.Log("1. Set the desired sprite in SpriteRenderer");
//        Debug.Log("2. Adjust Transform scale to set projectile size");
//        Debug.Log("3. Set projectileColor in Projectile component");
//        Debug.Log("4. Configure hitLayers (Player, Ground, Wall)");
//        Debug.Log("5. Save as prefab and assign to RangedEnemy");
//    }
//    }
    
//    [ContextMenu("Create Fire Point")]
//    public void CreateFirePoint()
//    {
//        Debug.Log("Creating fire point...");
        
//        // Create fire point as child
//        GameObject firePoint = new GameObject("FirePoint");
//        firePoint.transform.SetParent(transform);
//        firePoint.transform.localPosition = new Vector3(1f, 0.5f, 0f); // Adjust position as needed
        
//        // Assign to RangedEnemy
//        RangedEnemy rangedEnemy = GetComponent<RangedEnemy>();
//        if (rangedEnemy != null)
//        {
//            rangedEnemy.firePoint = firePoint.transform;
//            Debug.Log("Fire point created and assigned!");
//        }
//    }
    
//    void OnDrawGizmosSelected()
//    {
//        // Draw setup info
//        Gizmos.color = Color.cyan;
//        Gizmos.DrawWireCube(transform.position, Vector3.one);
        
//        // Draw fire point if exists
//        RangedEnemy rangedEnemy = GetComponent<RangedEnemy>();
//        if (rangedEnemy != null && rangedEnemy.firePoint != null)
//        {
//            Gizmos.color = Color.red;
//            Gizmos.DrawWireSphere(rangedEnemy.firePoint.position, 0.1f);
//            Gizmos.DrawLine(transform.position, rangedEnemy.firePoint.position);
//        }
//    }
//} 