using UnityEngine;

public class BossAttackDebug : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool enableDebug = true;
    public bool showAttackBox = true;
    public bool forceAttack = false;
    
    private Map3BossController bossController;
    private Transform player;
    
    void Start()
    {
        bossController = GetComponent<Map3BossController>();
        
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }
    
    void Update()
    {
        if (!enableDebug || bossController == null || player == null) return;
        
        // Debug attack detection every second
        if (Time.frameCount % 60 == 0)
        {
            DebugAttackDetection();
        }
        
        // Force attack test
        if (forceAttack && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Force attacking!");
            bossController.DoRandomAttack();
        }
    }
    
    void DebugAttackDetection()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        
        // Check attack box
        Vector2 attackCenter = (Vector2)transform.position + bossController.attackRangeBoxOffset;
        bool playerInAttackBox = Physics2D.OverlapBox(
            attackCenter,
            bossController.attackRangeBoxSize,
            0,
            bossController.playerLayer
        );
        
        Debug.Log("=== ATTACK DEBUG ===");
        Debug.Log($"Distance to Player: {distance:F2}");
        Debug.Log($"Attack Box Center: {attackCenter}");
        Debug.Log($"Attack Box Size: {bossController.attackRangeBoxSize}");
        Debug.Log($"Player Layer Mask: {bossController.playerLayer.value}");
        Debug.Log($"Player Actual Layer: {player.gameObject.layer}");
        Debug.Log($"Player in Attack Box: {playerInAttackBox}");
        
        // Check what's in the attack box
        Collider2D[] objectsInBox = Physics2D.OverlapBoxAll(
            attackCenter,
            bossController.attackRangeBoxSize,
            0
        );
        
        Debug.Log($"Objects in Attack Box: {objectsInBox.Length}");
        foreach (var obj in objectsInBox)
        {
            Debug.Log($"- {obj.name} (Layer: {obj.gameObject.layer})");
        }
        
        // Check attack cooldown
        float timeSinceLastAttack = Time.time - GetLastAttackTime();
        Debug.Log($"Time since last attack: {timeSinceLastAttack:F2}");
        Debug.Log($"Attack cooldown: {bossController.attackCooldown}");
        Debug.Log($"Can attack: {timeSinceLastAttack >= bossController.attackCooldown}");
        
        // Check boss state
        bool isAttacking = GetPrivateField<bool>("isAttacking");
        bool isDead = GetPrivateField<bool>("isDead");
        Debug.Log($"Boss isAttacking: {isAttacking}");
        Debug.Log($"Boss isDead: {isDead}");
    }
    
    float GetLastAttackTime()
    {
        var field = typeof(Map3BossController).GetField("lastAttackTime", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            return (float)field.GetValue(bossController);
        }
        return 0f;
    }
    
    T GetPrivateField<T>(string fieldName)
    {
        if (bossController == null) return default(T);
        
        var field = typeof(Map3BossController).GetField(fieldName, 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            return (T)field.GetValue(bossController);
        }
        return default(T);
    }
    
    void OnDrawGizmos()
    {
        if (!enableDebug || !showAttackBox || bossController == null) return;
        
        // Draw attack box
        Gizmos.color = Color.red;
        Vector3 boxCenter = transform.position + (Vector3)bossController.attackRangeBoxOffset;
        Gizmos.DrawWireCube(boxCenter, bossController.attackRangeBoxSize);
        
        // Draw attack box filled
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawCube(boxCenter, bossController.attackRangeBoxSize);
        
        // Draw line to player
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
            
            // Draw distance text
            float distance = Vector2.Distance(transform.position, player.position);
            UnityEditor.Handles.Label(
                Vector3.Lerp(transform.position, player.position, 0.5f),
                $"Distance: {distance:F2}"
            );
        }
    }
    
    [ContextMenu("Force Attack Now")]
    public void ForceAttackNow()
    {
        if (bossController != null)
        {
            bossController.DoRandomAttack();
            Debug.Log("Forced attack!");
        }
    }
    
    [ContextMenu("Reset Attack Cooldown")]
    public void ResetAttackCooldown()
    {
        var field = typeof(Map3BossController).GetField("lastAttackTime", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            field.SetValue(bossController, 0f);
            Debug.Log("Attack cooldown reset!");
        }
    }
}
