using UnityEngine;

public class BossDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 10f;
    public bool isMagicDamage = false;
    public bool onlyDamageWhenAttacking = true; // Only damage when boss is attacking

    [Header("Auto Find Player")]
    public bool autoFindPlayer = true;
    public PlayerController1 playerController;

    [Header("Boss Reference")]
    public Map3BossController bossController;

    void Start()
    {
        // Auto find player if not assigned
        if (autoFindPlayer && playerController == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerController = playerObj.GetComponent<PlayerController1>();
                Debug.Log("BossDamage: Player found and assigned!");
            }
            else
            {
                Debug.LogWarning("BossDamage: Player not found!");
            }
        }

        // Auto find boss controller if not assigned
        if (bossController == null)
        {
            bossController = GetComponent<Map3BossController>();
            if (bossController == null)
            {
                bossController = GetComponentInParent<Map3BossController>();
            }

            if (bossController != null)
            {
                Debug.Log("BossDamage: Boss controller found and assigned!");
            }
            else
            {
                Debug.LogWarning("BossDamage: Boss controller not found!");
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"🔥 BossDamage: OnCollisionEnter2D with {collision.gameObject.name}");

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("🔥 BossDamage: Player collision detected!");

            // Check if should damage player
            if (ShouldDamagePlayer())
            {
                PlayerController1 player = collision.gameObject.GetComponent<PlayerController1>();
                if (player != null)
                {
                    player.TakeDamage(damage, isMagicDamage);
                    Debug.Log($"🩸 BossDamage: Player hit by boss during attack, damage taken: {damage}");
                }
                else
                {
                    Debug.LogWarning("BossDamage: Player doesn't have PlayerController1 component!");
                }
            }
            else
            {
                Debug.Log("🛡️ BossDamage: Player touched boss but boss is not attacking - no damage");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"🔥 BossDamage: OnTriggerEnter2D with {other.gameObject.name}");

        if (other.CompareTag("Player"))
        {
            Debug.Log("🔥 BossDamage: Player trigger detected!");

            // Check if should damage player
            if (ShouldDamagePlayer())
            {
                PlayerController1 player = other.GetComponent<PlayerController1>();
                if (player != null)
                {
                    player.TakeDamage(damage, isMagicDamage);
                    Debug.Log($"🩸 BossDamage: Player hit by boss during attack (trigger), damage taken: {damage}");
                }
                else
                {
                    Debug.LogWarning("BossDamage: Player doesn't have PlayerController1 component!");
                }
            }
            else
            {
                Debug.Log("🛡️ BossDamage: Player touched boss trigger but boss is not attacking - no damage");
            }
        }
    }

    /// <summary>
    /// Check if boss should damage player based on attack state
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
            Debug.LogWarning("BossDamage: No boss controller found - defaulting to damage");
            return true;
        }

        // Check if boss is currently attacking
        bool isAttacking = bossController.IsCurrentlyAttacking;

        if (isAttacking)
        {
            Debug.Log("Boss is attacking - damage allowed");
        }
        else
        {
            Debug.Log("Boss is not attacking - no damage");
        }

        return isAttacking;
    }
}

