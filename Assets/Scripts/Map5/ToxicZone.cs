using UnityEngine;

public class ToxicZone : MonoBehaviour
{
    public float damagePerSecond = 10f;
    private PlayerController1 player;
    private bool playerInside = false;
    private float timer = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerController1>();
            playerInside = true;
            timer = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            player = null;
        }
    }

    private void Update()
    {
        if (playerInside && player != null)
        {
            timer += Time.deltaTime;
            if (timer >= 0.5f) // Gây sát thương mỗi 0.5 giây
            {
                player.TakeDamage(damagePerSecond, true); // true = magic damage
                timer = 0f;
            }
        }
    }
}
