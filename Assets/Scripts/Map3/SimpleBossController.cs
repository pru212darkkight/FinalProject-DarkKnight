using UnityEngine;

public class SimpleBossController : MonoBehaviour
{
    [Header("Settings")]
    public float detectionRange = 8f;
    public float attackRange = 2f;
    public float moveSpeed = 3f;
    
    [Header("References")]
    public Transform player;
    public Animator animator;
    
    private bool playerDetected = false;
    
    void Start()
    {
        // Auto find player
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
        
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }
    
    void Update()
    {
        if (player == null) return;
        
        float distance = Vector2.Distance(transform.position, player.position);
        
        if (distance <= detectionRange && !playerDetected)
        {
            // Player detected - start walking
            playerDetected = true;
            animator.SetBool("IsWalking", true);
            Debug.Log("Boss detected player!");
        }
        else if (distance > detectionRange && playerDetected)
        {
            // Player lost - stop walking
            playerDetected = false;
            animator.SetBool("IsWalking", false);
            Debug.Log("Boss lost player!");
        }
        
        if (playerDetected)
        {
            if (distance <= attackRange)
            {
                // Attack
                animator.SetBool("IsWalking", false);
                animator.SetTrigger("attack");
                Debug.Log("Boss attacking!");
            }
            else
            {
                // Move towards player
                Vector2 direction = (player.position - transform.position).normalized;
                transform.Translate(direction * moveSpeed * Time.deltaTime);
                
                // Face player
                if (direction.x > 0)
                    transform.localScale = new Vector3(1, 1, 1);
                else
                    transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }
    
    public void OnAttackEnd()
    {
        // Called by animation event
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance <= detectionRange)
            {
                animator.SetBool("IsWalking", true);
            }
        }
    }
    
    public void TakeDamage(float damage)
    {
        Debug.Log($"Boss took {damage} damage!");
        // Add health logic here later
    }
    
    void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
