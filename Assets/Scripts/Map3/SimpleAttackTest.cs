using UnityEngine;

public class SimpleAttackTest : MonoBehaviour
{
    [Header("Test Settings")]
    public KeyCode attackKey = KeyCode.Space;
    public float attackRange = 2f;
    public float attackCooldown = 0.5f;
    
    private bool canAttack = true;
    private float lastAttackTime;
    
    private void Update()
    {
        // Simple input test - bypass Input System
        if (Input.GetKeyDown(attackKey) && canAttack)
        {
            PerformAttack();
        }
        
        // Update cooldown
        if (!canAttack && Time.time >= lastAttackTime + attackCooldown)
        {
            canAttack = true;
        }
    }
    
    void PerformAttack()
    {
        canAttack = false;
        lastAttackTime = Time.time;
        
        Debug.Log("ATTACK! Looking for enemies...");
        
        // Tìm tất cả colliders trong tầm attack
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, attackRange);
        
        foreach (Collider2D hit in hitObjects)
        {
            if (hit.gameObject == gameObject) continue; // Skip self
            
            Debug.Log($"Hit object: {hit.name}");
            
            // Test UnderwaterEnemyHealth
            UnderwaterEnemyHealth enemyHealth = hit.GetComponent<UnderwaterEnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.OnPlayerAttack();
                Debug.Log($"Attacked fish: {hit.name}");
                continue;
            }
            
            // Test UnderwaterMine
            UnderwaterMine mine = hit.GetComponent<UnderwaterMine>();
            if (mine != null)
            {
                mine.OnPlayerAttack();
                Debug.Log($"Attacked mine: {hit.name}");
                continue;
            }
            
            // Test any object with "Fish" or "Mine" in name
            if (hit.name.Contains("Fish") || hit.name.Contains("Mine"))
            {
                Debug.Log($"Found enemy without health component: {hit.name}");
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = canAttack ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
