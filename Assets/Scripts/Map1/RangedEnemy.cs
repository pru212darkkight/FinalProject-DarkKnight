using UnityEngine;
using System.Collections;

public class RangedEnemy : Enemy
{
    [Header("Ranged Attack Settings")]
    public float rangedAttackRange = 4f;        // Tầm tấn công từ xa
    public float rangedDetectionRange = 6f;     // Tầm phát hiện player
    public float rangedAttackCooldown = 2f;     // Thời gian chờ giữa các lần tấn công
    public float projectileSpeed = 8f;    // Tốc độ projectile
    public GameObject projectilePrefab;   // Prefab của projectile
    public Transform firePoint;           // Điểm bắn projectile
    
    [Header("Animation Settings")]
    public float idleAnimationSpeed = 1f;
    public float attackAnimationSpeed = 1.2f;
    
    [Header("Visual Effects")]
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;
    public int flashCount = 3;
    
    // Private variables
    private bool isAttacking = false;
    private bool isPlayerInRange = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    // Animator parameter hash
    private readonly int IdleHash = Animator.StringToHash("Idle");
    private readonly int AttackHash = Animator.StringToHash("Attack");
    private readonly int HurtHash = Animator.StringToHash("Hurt");
    private readonly int DieHash = Animator.StringToHash("Die");
    
    protected override void Start()
    {
        base.Start();
        
        // Override base stats for ranged enemy
        this.attackRange = rangedAttackRange;
        this.detectionRange = rangedDetectionRange;
        this.attackCooldown = rangedAttackCooldown;
        
        // Get SpriteRenderer for flash effect
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        // Set initial animation to idle
        if (animator != null)
        {
            animator.SetBool(IdleHash, true);
            animator.speed = idleAnimationSpeed;
        }
        
        // Ensure firePoint exists
        if (firePoint == null)
        {
            firePoint = transform;
        }
    }
    
    protected override void Update()
    {
        if (isDead || player == null) return;
        
        // Check if player is in detection range
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool wasInRange = isPlayerInRange;
        isPlayerInRange = distanceToPlayer <= rangedDetectionRange;
        
        // Handle player entering/leaving range
        if (isPlayerInRange && !wasInRange)
        {
            OnPlayerEnterRange();
        }
        else if (!isPlayerInRange && wasInRange)
        {
            OnPlayerLeaveRange();
        }
        
        // Update facing direction when player is in range
        if (isPlayerInRange)
        {
            UpdateFacingDirection();
        }
        
        // Attack logic
        if (isPlayerInRange && distanceToPlayer <= rangedAttackRange && !isAttacking)
        {
            if (Time.time >= lastAttackTime + rangedAttackCooldown)
            {
                StartCoroutine(PerformRangedAttack());
            }
        }
    }
    
    private void OnPlayerEnterRange()
    {
        // Could add sound effect or visual indicator here
    }
    
    private void OnPlayerLeaveRange()
    {
        // Return to idle state
        if (animator != null)
        {
            animator.SetBool(IdleHash, true);
            animator.speed = idleAnimationSpeed;
        }
    }
    
    private void UpdateFacingDirection()
    {
        if (player == null) return;
        
        float direction = player.position.x > transform.position.x ? 1 : -1;
        if ((direction > 0 && !isFacingRight) || (direction < 0 && isFacingRight))
        {
            Flip();
        }
    }
    
    private IEnumerator PerformRangedAttack()
    {
        isAttacking = true;
        
        // Trigger attack animation
        if (animator != null)
        {
            animator.SetBool(IdleHash, false);
            animator.SetTrigger(AttackHash);
            animator.speed = attackAnimationSpeed;
        }
        
        // Wait for attack animation to reach the firing point
        yield return new WaitForSeconds(0.3f); // Adjust timing based on your animation
        
        // Fire projectile
        FireProjectile();
        
        // Wait for attack animation to complete
        yield return new WaitForSeconds(0.7f); // Adjust timing based on your animation
        
        // Return to idle
        if (animator != null)
        {
            animator.SetBool(IdleHash, true);
            animator.speed = idleAnimationSpeed;
        }
        
        lastAttackTime = Time.time;
        isAttacking = false;
    }
    
    private void FireProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Projectile prefab is not assigned!");
            return;
        }

        // Phát âm thanh tấn công skeleton
        if (AudioManager.Instance != null && AudioManager.Instance.skeletonAttack != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.skeletonAttack);
            Debug.Log("💀 Skeleton attacking - playing attack sound!");
        }

        // Calculate direction to player
        Vector2 direction = (player.position - firePoint.position).normalized;
        
        // Create projectile from prefab (preserves all prefab properties)
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        
        // Get projectile components
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        
        if (projectileScript != null)
        {
            // Only override essential properties, preserve visual properties from prefab
            projectileScript.damage = damage;
            projectileScript.isEnemyProjectile = true;
            
            // Use prefab's speed if available, otherwise use enemy's speed
            float finalSpeed = projectileScript.speed > 0 ? projectileScript.speed : projectileSpeed;
            
            // Set direction and velocity directly to ensure movement
            if (projectileRb != null)
            {
                projectileRb.linearVelocity = direction * finalSpeed;
                
                // Ensure Rigidbody2D is enabled and configured correctly
                projectileRb.simulated = true;
                projectileRb.bodyType = RigidbodyType2D.Kinematic;
                projectileRb.gravityScale = 0f;
            }
            
           
            
        }
        else
        {
            // Fallback if no Projectile script
            if (projectileRb != null)
            {
                projectileRb.linearVelocity = direction * projectileSpeed;
                projectileRb.simulated = true;
                projectileRb.bodyType = RigidbodyType2D.Kinematic;
                projectileRb.gravityScale = 0f;
            }
            
        }
    }
    
    public override void TakeDamage(float damage, bool isMagicDamage = false)
    {
        if (isDead) return;
        
        base.TakeDamage(damage, isMagicDamage);
        
        // Flash effect
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashEffect());
        }
    }
    
    private IEnumerator FlashEffect()
    {
        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
    }
    
    protected override void Die()
    {
        isDead = true;
        
        // Stop all coroutines
        StopAllCoroutines();
        
        // Trigger death animation
        if (animator != null)
        {
            animator.SetTrigger(DieHash);
        }
        
        // Drop coins if CoinDrop component exists
        CoinDrop coinDrop = GetComponent<CoinDrop>();
        if (coinDrop != null)
        {
            coinDrop.DropCoin();
        }
        
        // Disable components
        if (rb != null) rb.simulated = false;
        if (GetComponent<Collider2D>() != null) GetComponent<Collider2D>().enabled = false;
        
        // Disable this script after a delay
        StartCoroutine(DisableAfterDeath());
    }
    
    private IEnumerator DisableAfterDeath()
    {
        yield return new WaitForSeconds(2f); // Wait for death animation
        gameObject.SetActive(false);
    }
    
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangedDetectionRange);
        
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangedAttackRange);
        
        // Draw fire point
        if (firePoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(firePoint.position, 0.1f);
        }
    }
} 