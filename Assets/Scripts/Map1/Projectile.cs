using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float damage = 10f;
    public float speed = 8f;
    public float lifetime = 5f;
    public bool isEnemyProjectile = false;
    public bool destroyOnHit = true;
    
    [Header("Visual Effects")]
    public GameObject hitEffect;
    public Color projectileColor = Color.red;
    
    [Header("Physics")]
    public LayerMask hitLayers = -1;
    public bool useGravity = false;
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float timer;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Set projectile color only if not already set in prefab
        if (spriteRenderer != null && projectileColor != Color.white)
        {
            // Only change color if it's different from default
            if (spriteRenderer.color == Color.white)
            {
                spriteRenderer.color = projectileColor;
            }
        }
        
        // Ensure Rigidbody2D is configured correctly
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = useGravity ? 1f : 0f;
            rb.simulated = true;
        }
        
        // Destroy after lifetime
        Destroy(gameObject, lifetime);
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        
        // Optional: Add rotation effect
        transform.Rotate(0, 0, 360f * Time.deltaTime);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the hit object is in our hit layers
        if (!IsInHitLayers(other.gameObject))
        {
            return; // Ignore objects not in hit layers
        }
        
        // Check if we hit the appropriate target
        if (isEnemyProjectile)
        {
            // Enemy projectile hits player
            if (other.CompareTag("Player"))
            {
                PlayerController1 player = other.GetComponent<PlayerController1>();
                if (player != null)
                {
                    player.TakeDamage(damage,true,"Wind Spirit");
                }
                
                if (destroyOnHit)
                {
                    OnHit();
                }
            }
        }
        else
        {
            // Player projectile hits enemy
            if (other.CompareTag("Enemy"))
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
                
                if (destroyOnHit)
                {
                    OnHit();
                }
            }
        }
        
        // Hit walls/obstacles (only if they're in hit layers)
        if (other.CompareTag("Ground"))
        {
            if (destroyOnHit)
            {
                OnHit();
            }
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the hit object is in our hit layers
        if (!IsInHitLayers(collision.gameObject))
        {
            return; // Ignore objects not in hit layers
        }
        
        // Handle collision with walls/obstacles
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Wall"))
        {
            if (destroyOnHit)
            {
                OnHit();
            }
        }
    }
    
    private void OnHit()
    {
        // Spawn hit effect
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, transform.rotation);
        }
        
        // Destroy projectile
        Destroy(gameObject);
    }
    
    // Public method to set projectile properties
    public void SetProjectileProperties(float damage, float speed, bool isEnemyProjectile)
    {
        this.damage = damage;
        this.speed = speed;
        this.isEnemyProjectile = isEnemyProjectile;
        
        // Update velocity if Rigidbody2D exists
        if (rb != null)
        {
            rb.linearVelocity = transform.right * speed;
        }
    }
    
    // Public method to set direction
    public void SetDirection(Vector2 direction)
    {
        if (rb != null)
        {
            // Ensure Rigidbody2D is properly configured
            rb.simulated = true;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
            
            // Set velocity
            rb.linearVelocity = direction * speed;
        }
        
        // Rotate projectile to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    
    // Check if GameObject is in hit layers
    private bool IsInHitLayers(GameObject obj)
    {
        // If hitLayers is set to Everything (-1), allow all collisions
        if (hitLayers == -1)
        {
            return true;
        }
        
        // Check if the object's layer is in our hit layers
        return (hitLayers.value & (1 << obj.layer)) != 0;
    }
    
    // Debug method to show current hit layers
    public void DebugHitLayers()
    {
        Debug.Log($"Projectile hit layers: {hitLayers.value}");
        Debug.Log($"Projectile layer: {gameObject.layer}");
        Debug.Log($"Is enemy projectile: {isEnemyProjectile}");
    }
    
    // Debug method to show projectile properties
    public void DebugProjectileProperties()
    {
        Debug.Log($"=== PROJECTILE PROPERTIES ===");
        Debug.Log($"Damage: {damage}");
        Debug.Log($"Speed: {speed}");
        Debug.Log($"Lifetime: {lifetime}");
        Debug.Log($"Is Enemy Projectile: {isEnemyProjectile}");
        Debug.Log($"Projectile Color: {projectileColor}");
        
        if (spriteRenderer != null)
        {
            Debug.Log($"Actual Sprite Color: {spriteRenderer.color}");
            //Debug.Log($"Sprite Size: {spriteRenderer.sprite != null ? spriteRenderer.sprite.rect.size : Vector2.zero}");
        }
        
        if (rb != null)
        {
            Debug.Log($"Rigidbody Velocity: {rb.linearVelocity}");
            Debug.Log($"Gravity Scale: {rb.gravityScale}");
            Debug.Log($"Rigidbody Simulated: {rb.simulated}");
            Debug.Log($"Rigidbody Body Type: {rb.bodyType}");
        }
        
        Debug.Log($"Transform Scale: {transform.localScale}");
        Debug.Log($"Transform Position: {transform.position}");
        Debug.Log($"Transform Rotation: {transform.rotation.eulerAngles}");
        Debug.Log($"=== END PROJECTILE PROPERTIES ===");
    }
} 