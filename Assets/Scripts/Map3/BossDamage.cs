using UnityEngine;

public class BossDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 10f;
    public bool isMagicDamage = false;

    [Header("Auto Find Player")]
    public bool autoFindPlayer = true;
    public PlayerController1 playerController;

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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController1 player = collision.gameObject.GetComponent<PlayerController1>();
            if (player != null)
            {
                player.TakeDamage(damage, isMagicDamage);
                Debug.Log($"Player hit by boss, damage taken: {damage}");
            }
            else
            {
                Debug.LogWarning("BossDamage: Player doesn't have PlayerController1 component!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController1 player = other.GetComponent<PlayerController1>();
            if (player != null)
            {
                player.TakeDamage(damage, isMagicDamage);
                Debug.Log($"Player hit by boss (trigger), damage taken: {damage}");
            }
            else
            {
                Debug.LogWarning("BossDamage: Player doesn't have PlayerController1 component!");
            }
        }
    }
}

