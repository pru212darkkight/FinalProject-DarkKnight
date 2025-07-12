using UnityEngine;

public class SimplePlayerTest : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    
    [Header("Debug")]
    public bool enableDebug = true;
    
    private Rigidbody2D rb;
    private bool isGrounded = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (enableDebug)
        {
            Debug.Log("SimplePlayerTest: Started!");
        }
    }
    
    void Update()
    {
        // Get input
        float horizontal = Input.GetAxisRaw("Horizontal");
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);
        
        if (enableDebug && (horizontal != 0 || jumpPressed))
        {
            Debug.Log($"Input - Horizontal: {horizontal}, Jump: {jumpPressed}");
        }
        
        // Move horizontally
        Vector2 velocity = rb.linearVelocity;
        velocity.x = horizontal * moveSpeed;
        
        // Jump
        if (jumpPressed && isGrounded)
        {
            velocity.y = jumpForce;
            if (enableDebug) Debug.Log("Jumping!");
        }
        
        rb.linearVelocity = velocity;
        
        if (enableDebug && Time.frameCount % 60 == 0)
        {
            Debug.Log($"Position: {transform.position}, Velocity: {rb.linearVelocity}");
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            if (enableDebug) Debug.Log("Grounded!");
        }
    }
    
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            if (enableDebug) Debug.Log("Left ground!");
        }
    }
}
