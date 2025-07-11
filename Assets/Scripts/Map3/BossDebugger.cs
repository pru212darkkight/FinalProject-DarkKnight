using UnityEngine;

public class BossDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool enableDebug = false; // Disabled to avoid Input System errors
    public bool showDetailedInfo = true;
    public bool forceMovement = false;
    public float forceSpeed = 2f;
    
    private Map3BossController bossController;
    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;
    
    void Start()
    {
        bossController = GetComponent<Map3BossController>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        if (enableDebug)
        {
            Debug.Log("=== BOSS DEBUGGER STARTED ===");
            LogBossSettings();
        }
    }
    
    void Update()
    {
        if (!enableDebug) return;
        
        if (showDetailedInfo && Time.frameCount % 60 == 0) // Every second
        {
            LogDetailedInfo();
        }
        
        // Force movement test (using Input System)
        if (forceMovement && UnityEngine.InputSystem.Keyboard.current.mKey.isPressed)
        {
            Vector2 vel = rb.linearVelocity;
            vel.x = forceSpeed;
            rb.linearVelocity = vel;
            Debug.Log($"Force moving boss with velocity: {vel}");
        }
    }
    
    void LogBossSettings()
    {
        Debug.Log("=== BOSS SETTINGS ===");
        
        if (bossController != null)
        {
            Debug.Log($"Move Speed: {bossController.moveSpeed}");
            Debug.Log($"Attack Cooldown: {bossController.attackCooldown}");
            Debug.Log($"Attack Range Box Size: {bossController.attackRangeBoxSize}");
            Debug.Log($"Attack Range Box Offset: {bossController.attackRangeBoxOffset}");
            Debug.Log($"Player Layer: {bossController.playerLayer.value}");
            Debug.Log($"Show Debug: {bossController.showDebug}");
        }
        
        if (rb != null)
        {
            Debug.Log($"Rigidbody Mass: {rb.mass}");
            Debug.Log($"Rigidbody Gravity Scale: {rb.gravityScale}");
            Debug.Log($"Rigidbody Linear Drag: {rb.linearDamping}");
            Debug.Log($"Rigidbody Freeze Position: {rb.freezeRotation}");
            Debug.Log($"Rigidbody Body Type: {rb.bodyType}");
        }
        
        if (player != null)
        {
            Debug.Log($"Player found: {player.name}");
            Debug.Log($"Player layer: {player.gameObject.layer}");
            Debug.Log($"Player tag: {player.tag}");
        }
        else
        {
            Debug.LogError("Player not found!");
        }
    }
    
    void LogDetailedInfo()
    {
        if (player == null) return;
        
        float distance = Vector2.Distance(transform.position, player.position);
        
        // Check attack box
        Vector2 attackCenter = (Vector2)transform.position + bossController.attackRangeBoxOffset;
        bool playerInAttackBox = Physics2D.OverlapBox(
            attackCenter,
            bossController.attackRangeBoxSize,
            0,
            bossController.playerLayer
        );
        
        Debug.Log("=== BOSS STATUS ===");
        Debug.Log($"Boss Position: {transform.position}");
        Debug.Log($"Player Position: {player.position}");
        Debug.Log($"Distance to Player: {distance:F2}");
        Debug.Log($"Player in Attack Box: {playerInAttackBox}");
        Debug.Log($"Boss Velocity: {rb.linearVelocity}");
        Debug.Log($"Is Attacking: {GetPrivateField("isAttacking")}");
        Debug.Log($"Is Dead: {GetPrivateField("isDead")}");
        
        // Check animator
        if (animator != null)
        {
            bool isWalking = animator.GetBool("IsWalking");
            Debug.Log($"IsWalking Animation: {isWalking}");
        }
        
        // Check what's in player layer
        Collider2D[] objectsInLayer = Physics2D.OverlapBoxAll(
            attackCenter,
            bossController.attackRangeBoxSize * 2f,
            0,
            bossController.playerLayer
        );
        
        Debug.Log($"Objects in Player Layer: {objectsInLayer.Length}");
        foreach (var obj in objectsInLayer)
        {
            Debug.Log($"- {obj.name} (Layer: {obj.gameObject.layer})");
        }
    }
    
    bool GetPrivateField(string fieldName)
    {
        if (bossController == null) return false;
        
        var field = typeof(Map3BossController).GetField(fieldName, 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            return (bool)field.GetValue(bossController);
        }
        return false;
    }
    
    void OnDrawGizmos()
    {
        if (!enableDebug || bossController == null) return;
        
        // Draw attack box
        Gizmos.color = Color.red;
        Vector3 boxCenter = transform.position + (Vector3)bossController.attackRangeBoxOffset;
        Gizmos.DrawWireCube(boxCenter, bossController.attackRangeBoxSize);
        
        // Draw larger detection area
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(boxCenter, bossController.attackRangeBoxSize * 2f);
        
        // Draw line to player
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
    
    [ContextMenu("Force Test Movement")]
    public void ForceTestMovement()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(2f, 0f);
            Debug.Log("Forced boss movement!");
        }
    }
    
    [ContextMenu("Reset Boss Physics")]
    public void ResetBossPhysics()
    {
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1f;
            rb.linearDamping = 0f;
            rb.angularDamping = 0.05f;
            rb.freezeRotation = true;
            Debug.Log("Boss physics reset!");
        }
    }
}
