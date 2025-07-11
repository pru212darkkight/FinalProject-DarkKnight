using UnityEngine;
using UnityEngine.InputSystem;

public class JumpDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool enableDebug = true;
    public bool showGroundCheck = true;
    public bool showJumpInput = true;
    public bool showVelocity = true;
    
    [Header("Override Settings")]
    public bool overrideJump = false;
    public float overrideJumpForce = 15f;
    public KeyCode overrideJumpKey = KeyCode.J;
    
    private PlayerController1 playerController;
    private Rigidbody2D rb;
    private bool wasJumpPressed = false;
    
    void Start()
    {
        playerController = GetComponent<PlayerController1>();
        rb = GetComponent<Rigidbody2D>();
        
        if (enableDebug)
        {
            Debug.Log("=== JUMP DEBUGGER STARTED ===");
            LogPlayerSettings();
        }
    }
    
    void Update()
    {
        if (!enableDebug) return;
        
        // Check jump input
        bool jumpPressed = false;
        if (playerController != null && playerController.jumpAction != null)
        {
            jumpPressed = playerController.jumpAction.WasPressedThisFrame();
        }
        
        // Fallback input check
        bool spacePressed = Input.GetKeyDown(KeyCode.Space);
        
        if (showJumpInput && (jumpPressed || spacePressed))
        {
            Debug.Log($"[JUMP INPUT] InputAction: {jumpPressed}, Space: {spacePressed}");
        }
        
        // Ground check debug
        if (showGroundCheck && playerController != null)
        {
            bool isGrounded = Physics2D.OverlapCircle(
                playerController.groundCheck.position, 
                playerController.groundCheckRadius, 
                playerController.groundLayer
            );
            
            if (Time.frameCount % 60 == 0) // Every second
            {
                Debug.Log($"[GROUND CHECK] IsGrounded: {isGrounded}, Position: {playerController.groundCheck.position}, Radius: {playerController.groundCheckRadius}");
            }
        }
        
        // Velocity debug
        if (showVelocity && rb != null && Time.frameCount % 30 == 0)
        {
            Debug.Log($"[VELOCITY] Current: {rb.linearVelocity}, Gravity Scale: {rb.gravityScale}");
        }
        
        // Override jump
        if (overrideJump && Input.GetKeyDown(overrideJumpKey))
        {
            Debug.Log($"[OVERRIDE JUMP] Force: {overrideJumpForce}");
            Vector2 vel = rb.linearVelocity;
            vel.y = overrideJumpForce;
            rb.linearVelocity = vel;
        }
        
        // Check if jump was attempted but failed
        if ((jumpPressed || spacePressed) && !wasJumpPressed)
        {
            wasJumpPressed = true;
            StartCoroutine(CheckJumpResult());
        }
        
        if (!jumpPressed && !spacePressed)
        {
            wasJumpPressed = false;
        }
    }
    
    System.Collections.IEnumerator CheckJumpResult()
    {
        float initialY = transform.position.y;
        yield return new WaitForSeconds(0.1f);
        
        float newY = transform.position.y;
        float yDifference = newY - initialY;
        
        if (yDifference < 0.1f)
        {
            Debug.LogWarning($"[JUMP FAILED] Y difference: {yDifference:F3}. Possible issues:");
            Debug.LogWarning("- Not grounded");
            Debug.LogWarning("- Jump force too low");
            Debug.LogWarning("- Gravity scale too high");
            Debug.LogWarning("- Input not registered");
            
            LogDetailedState();
        }
        else
        {
            Debug.Log($"[JUMP SUCCESS] Y difference: {yDifference:F3}");
        }
    }
    
    void LogPlayerSettings()
    {
        if (playerController == null) return;
        
        Debug.Log($"[PLAYER SETTINGS]");
        Debug.Log($"Jump Force: {playerController.jumpForce}");
        Debug.Log($"Move Speed: {playerController.moveSpeed}");
        Debug.Log($"Ground Check Radius: {playerController.groundCheckRadius}");
        Debug.Log($"Ground Layer: {playerController.groundLayer.value}");
        
        if (rb != null)
        {
            Debug.Log($"[RIGIDBODY SETTINGS]");
            Debug.Log($"Mass: {rb.mass}");
            Debug.Log($"Gravity Scale: {rb.gravityScale}");
            Debug.Log($"Linear Drag: {rb.linearDamping}");
            Debug.Log($"Angular Drag: {rb.angularDamping}");
        }
    }
    
    void LogDetailedState()
    {
        if (playerController == null || rb == null) return;
        
        bool isGrounded = Physics2D.OverlapCircle(
            playerController.groundCheck.position, 
            playerController.groundCheckRadius, 
            playerController.groundLayer
        );
        
        Debug.Log($"[DETAILED STATE]");
        Debug.Log($"Is Grounded: {isGrounded}");
        Debug.Log($"Current Velocity: {rb.linearVelocity}");
        Debug.Log($"Position: {transform.position}");
        Debug.Log($"Ground Check Position: {playerController.groundCheck.position}");
        
        // Check what's in ground layer
        Collider2D[] groundObjects = Physics2D.OverlapCircleAll(
            playerController.groundCheck.position, 
            playerController.groundCheckRadius * 2f, 
            playerController.groundLayer
        );
        
        Debug.Log($"Ground objects found: {groundObjects.Length}");
        foreach (var obj in groundObjects)
        {
            Debug.Log($"- {obj.name} (Layer: {obj.gameObject.layer})");
        }
    }
    
    void OnDrawGizmos()
    {
        if (!enableDebug || playerController == null) return;
        
        // Draw ground check circle
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerController.groundCheck.position, playerController.groundCheckRadius);
        
        // Draw larger detection area
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(playerController.groundCheck.position, playerController.groundCheckRadius * 2f);
    }
}
