using UnityEngine;

public class LavaZone : MonoBehaviour
{
    public float lavaDamage = 9999f; 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var player = collision.GetComponent<PlayerController1>(); 
            if (player != null)
            {
                player.TakeDamage(lavaDamage, false, "Dung nham");
            }
        }
    }
}
