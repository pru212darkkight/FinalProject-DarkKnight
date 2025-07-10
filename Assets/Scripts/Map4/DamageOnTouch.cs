using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    public float damageAmount = 10f;
    public bool isMagicDamage = false;
    public float damageCooldown = 1f; // thời gian giữa 2 lần gây sát thương

    private float lastDamageTime = -999f;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryDealDamage(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TryDealDamage(collision);
    }

    void TryDealDamage(Collider2D collision)
    {
        if (Time.time < lastDamageTime + damageCooldown) return;

        PlayerController1 player = collision.GetComponent<PlayerController1>();
        if (player != null)
        {
            player.TakeDamage(damageAmount, isMagicDamage);
            lastDamageTime = Time.time;
        }
    }
}
