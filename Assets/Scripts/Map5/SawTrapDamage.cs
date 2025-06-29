using UnityEngine;

public class SawTrapDamage : MonoBehaviour
{
    public float damage = 10f;
    public bool continuousDamage = false;
    public float damageInterval = 0.5f; // nếu continuous = true

    private float nextDamageTime;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!continuousDamage && other.CompareTag("Player"))
        {
            PlayerController1 player = other.GetComponent<PlayerController1>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (continuousDamage && other.CompareTag("Player") && Time.time >= nextDamageTime)
        {
            PlayerController1 player = other.GetComponent<PlayerController1>();
            if (player != null)
            {
                player.TakeDamage(damage);
                nextDamageTime = Time.time + damageInterval;
            }
        }
    }
}
