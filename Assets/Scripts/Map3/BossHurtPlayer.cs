using UnityEngine;

public class BossHurtPlayer : MonoBehaviour 
{
    [Header("Damage Settings")]
    public float damageAmount = 15f;
    public bool isMagicDamage = false;
    public bool onlyDamageWhenAttacking = false; // TEST: Always damage
    
    [Header("References")]
    private PlayerController1 thePlayer;
    private Map3BossController bossController;
    
    [Header("Debug")]
    public bool showDebug = true;
    
    void Start() 
    {
        // Find player
        thePlayer = FindAnyObjectByType<PlayerController1>();
        
        // Find boss controller
        bossController = GetComponent<Map3BossController>();
        if (bossController == null)
        {
            bossController = GetComponentInParent<Map3BossController>();
        }
        
        if (showDebug)
        {
            Debug.Log($"BossHurtPlayer: Setup complete. Player: {(thePlayer != null ? "Found" : "Not Found")}, Boss: {(bossController != null ? "Found" : "Not Found")}");
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (showDebug)
            {
                Debug.Log("🔥 BossHurtPlayer: Player collision detected!");
            }
            
            // Check if should damage player
            if (ShouldDamagePlayer())
            {
                if (thePlayer != null) 
                {
                    thePlayer.TakeDamage(damageAmount, isMagicDamage);
                    Debug.Log($"🩸 Boss hit player! Damage: {damageAmount}");
                }
                else
                {
                    Debug.LogWarning("BossHurtPlayer: Player not found!");
                }
            }
            else
            {
                if (showDebug)
                {
                    Debug.Log("🛡️ BossHurtPlayer: Boss not attacking - no damage");
                }
            }
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (showDebug)
            {
                Debug.Log("🔥 BossHurtPlayer: Player collision detected!");
            }
            
            // Check if should damage player
            if (ShouldDamagePlayer())
            {
                if (thePlayer != null) 
                {
                    thePlayer.TakeDamage(damageAmount, isMagicDamage);
                    Debug.Log($"🩸 Boss hit player! Damage: {damageAmount}");
                }
                else
                {
                    Debug.LogWarning("BossHurtPlayer: Player not found!");
                }
            }
            else
            {
                if (showDebug)
                {
                    Debug.Log("🛡️ BossHurtPlayer: Boss not attacking - no damage");
                }
            }
        }
    }
    
    /// <summary>
    /// Check if boss should damage player
    /// </summary>
    private bool ShouldDamagePlayer()
    {
        // If onlyDamageWhenAttacking is disabled, always damage
        if (!onlyDamageWhenAttacking)
        {
            return true;
        }
        
        // If no boss controller, fall back to always damage
        if (bossController == null)
        {
            Debug.LogWarning("BossHurtPlayer: No boss controller found - defaulting to damage");
            return true;
        }
        
        // Check if boss is currently attacking
        bool isAttacking = bossController.IsCurrentlyAttacking;
        
        if (showDebug)
        {
            Debug.Log($"BossHurtPlayer: Boss attacking state: {isAttacking}");
        }
        
        return isAttacking;
    }
}
