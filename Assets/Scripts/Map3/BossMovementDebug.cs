using UnityEngine;

public class BossMovementDebug : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool enableDebug = true;
    public bool overrideBossMovement = false;
    public float overrideSpeed = 3f;
    
    private Map3BossController bossController;
    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;
    private EnemyHealth enemyHealth;
    
    void Start()
    {
        bossController = GetComponent<Map3BossController>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        if (enableDebug)
        {
            Debug.Log("=== BOSS MOVEMENT DEBUG STARTED ===");
            LogInitialState();
        }
    }
    
    void Update()
    {
        if (!enableDebug) return;
        
        // Log detailed state every second
        if (Time.frameCount % 60 == 0)
        {
            LogCurrentState();
        }
        
        // Override movement test
        if (overrideBossMovement && player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            Vector2 velocity = rb.linearVelocity;
            velocity.x = direction.x * overrideSpeed;
            rb.linearVelocity = velocity;
            
            if (animator != null)
            {
                animator.SetBool("IsWalking", true);
            }
        }
    }
    
    void LogInitialState()
    {
        Debug.Log("=== INITIAL STATE ===");
        
        // Boss Controller
        if (bossController != null)
        {
            Debug.Log($"Boss Controller: Found");
            Debug.Log($"Move Speed: {bossController.moveSpeed}");
            Debug.Log($"Show Debug: {bossController.showDebug}");
        }
        else
        {
            Debug.LogError("Boss Controller: NOT FOUND!");
        }
        
        // Rigidbody2D
        if (rb != null)
        {
            Debug.Log($"Rigidbody2D: Found");
            Debug.Log($"Body Type: {rb.bodyType}");
            Debug.Log($"Mass: {rb.mass}");
            Debug.Log($"Gravity Scale: {rb.gravityScale}");
            Debug.Log($"Linear Drag: {rb.linearDamping}");
            Debug.Log($"Freeze Position X: {rb.freezeRotation}");
            Debug.Log($"Is Kinematic: {rb.isKinematic}");
        }
        else
        {
            Debug.LogError("Rigidbody2D: NOT FOUND!");
        }
        
        // EnemyHealth
        if (enemyHealth != null)
        {
            Debug.Log($"EnemyHealth: Found");
            Debug.Log($"Is Dead: {enemyHealth.isDead}");
            Debug.Log($"Current Health: {enemyHealth.currentHealth}");
            Debug.Log($"Max Health: {enemyHealth.maxHealth}");
        }
        else
        {
            Debug.LogWarning("EnemyHealth: NOT FOUND!");
        }
        
        // Player
        if (player != null)
        {
            Debug.Log($"Player: Found at {player.position}");
            Debug.Log($"Player Layer: {player.gameObject.layer}");
        }
        else
        {
            Debug.LogError("Player: NOT FOUND!");
        }
        
        // Animator
        if (animator != null)
        {
            Debug.Log($"Animator: Found");
            Debug.Log($"Has IsWalking parameter: {HasParameter("IsWalking")}");
        }
        else
        {
            Debug.LogError("Animator: NOT FOUND!");
        }
    }
    
    void LogCurrentState()
    {
        Debug.Log("=== CURRENT STATE ===");
        
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            Debug.Log($"Distance to Player: {distance:F2}");
        }
        
        if (rb != null)
        {
            Debug.Log($"Current Velocity: {rb.linearVelocity}");
            Debug.Log($"Is Kinematic: {rb.isKinematic}");
        }
        
        if (bossController != null)
        {
            // Use reflection to get private fields
            var isDead = GetPrivateField<bool>("isDead");
            var isAttacking = GetPrivateField<bool>("isAttacking");
            
            Debug.Log($"Boss isDead: {isDead}");
            Debug.Log($"Boss isAttacking: {isAttacking}");
        }
        
        if (enemyHealth != null)
        {
            Debug.Log($"EnemyHealth isDead: {enemyHealth.isDead}");
            Debug.Log($"EnemyHealth currentHealth: {enemyHealth.currentHealth}");
        }
        
        if (animator != null && HasParameter("IsWalking"))
        {
            bool isWalking = animator.GetBool("IsWalking");
            Debug.Log($"IsWalking Animation: {isWalking}");
        }
    }
    
    T GetPrivateField<T>(string fieldName)
    {
        if (bossController == null) return default(T);
        
        var field = typeof(Map3BossController).GetField(fieldName, 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            return (T)field.GetValue(bossController);
        }
        return default(T);
    }
    
    bool HasParameter(string paramName)
    {
        if (animator == null) return false;
        
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
    
    [ContextMenu("Force Enable Movement")]
    public void ForceEnableMovement()
    {
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
            Debug.Log("Forced Rigidbody2D to Dynamic!");
        }
        
        overrideBossMovement = true;
        Debug.Log("Override movement enabled!");
    }
    
    [ContextMenu("Test Direct Movement")]
    public void TestDirectMovement()
    {
        if (rb != null && player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * 5f, rb.linearVelocity.y);
            Debug.Log($"Direct movement test: {rb.linearVelocity}");
        }
    }
}
