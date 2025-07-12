using UnityEngine;

public class PlayerPhysicsDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool enableDebug = true;
    public bool fixGravityScale = true;
    public float targetGravityScale = 1f;
    
    private Rigidbody2D rb;
    private PlayerController1 playerController;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController1>();
        
        if (enableDebug)
        {
            Debug.Log("=== PLAYER PHYSICS DEBUG ===");
            LogPhysicsSettings();
        }
        
        if (fixGravityScale && rb != null)
        {
            rb.gravityScale = targetGravityScale;
            Debug.Log($"Fixed gravity scale to {targetGravityScale}");
        }
    }
    
    void Update()
    {
        if (enableDebug && Time.frameCount % 120 == 0) // Every 2 seconds
        {
            LogPhysicsSettings();
        }
        
        // Auto-fix gravity if it gets changed
        if (fixGravityScale && rb != null && rb.gravityScale != targetGravityScale)
        {
            Debug.LogWarning($"Gravity scale changed from {targetGravityScale} to {rb.gravityScale}! Fixing...");
            rb.gravityScale = targetGravityScale;
        }
    }
    
    void LogPhysicsSettings()
    {
        if (rb == null) return;
        
        Debug.Log($"[PHYSICS] Gravity Scale: {rb.gravityScale}");
        Debug.Log($"[PHYSICS] Mass: {rb.mass}");
        Debug.Log($"[PHYSICS] Linear Drag: {rb.linearDamping}");
        Debug.Log($"[PHYSICS] Angular Drag: {rb.angularDamping}");
        Debug.Log($"[PHYSICS] Velocity: {rb.linearVelocity}");
        
        if (playerController != null)
        {
            Debug.Log($"[PLAYER] Jump Force: {playerController.jumpForce}");
            Debug.Log($"[PLAYER] Move Speed: {playerController.moveSpeed}");
        }
    }
    
    [ContextMenu("Force Fix Physics")]
    public void ForceFixPhysics()
    {
        if (rb != null)
        {
            rb.gravityScale = targetGravityScale;
            rb.mass = 1f;
            rb.linearDamping = 0f;
            rb.angularDamping = 0.05f;
            
            Debug.Log("Physics settings reset to default!");
            LogPhysicsSettings();
        }
    }
}
