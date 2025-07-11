using UnityEngine;

public class PlayerMovementDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool enableDebug = true;
    public bool showVelocity = true;
    public bool showInput = true;
    public bool showPosition = true;
    public bool showCollisions = true;
    
    private PlayerController1 playerController;
    private Rigidbody2D rb;
    private Vector3 lastPosition;
    private float debugTimer = 0f;
    
    void Start()
    {
        playerController = GetComponent<PlayerController1>();
        rb = GetComponent<Rigidbody2D>();
        lastPosition = transform.position;
    }
    
    void Update()
    {
        if (!enableDebug) return;
        
        debugTimer += Time.deltaTime;
        
        // Debug every 0.5 seconds
        if (debugTimer >= 0.5f)
        {
            debugTimer = 0f;
            
            if (showInput && playerController != null)
            {
                Vector2 input = playerController.MoveInput;
                Debug.Log($"[PLAYER DEBUG] Input: {input}");
            }
            
            if (showVelocity && rb != null)
            {
                Debug.Log($"[PLAYER DEBUG] Velocity: {rb.linearVelocity}");
            }
            
            if (showPosition)
            {
                Vector3 currentPos = transform.position;
                Vector3 movement = currentPos - lastPosition;
                Debug.Log($"[PLAYER DEBUG] Position: {currentPos}, Movement: {movement}");
                lastPosition = currentPos;
            }
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (showCollisions)
        {
            Debug.Log($"[PLAYER DEBUG] Collision Enter: {collision.gameObject.name} (Layer: {collision.gameObject.layer})");
        }
    }
    
    void OnCollisionStay2D(Collision2D collision)
    {
        if (showCollisions && debugTimer >= 1f)
        {
            Debug.Log($"[PLAYER DEBUG] Collision Stay: {collision.gameObject.name} (Layer: {collision.gameObject.layer})");
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (showCollisions)
        {
            Debug.Log($"[PLAYER DEBUG] Trigger Enter: {other.gameObject.name} (Layer: {other.gameObject.layer})");
        }
    }
    
    void OnDrawGizmos()
    {
        if (!enableDebug) return;
        
        // Draw velocity vector
        if (rb != null && showVelocity)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, rb.linearVelocity);
        }
        
        // Draw input vector
        if (playerController != null && showInput)
        {
            Gizmos.color = Color.green;
            Vector2 input = playerController.MoveInput;
            Gizmos.DrawRay(transform.position, new Vector3(input.x, input.y, 0) * 2f);
        }
    }
}
