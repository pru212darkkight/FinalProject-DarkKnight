using UnityEngine;

public class SimpleDeathTrap : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu là player
        if (other.CompareTag("Player"))
        {
            PlayerController1 player = other.GetComponent<PlayerController1>();
            if (player != null)
            {
                // Gọi method Die của player
                player.TakeDamage(100000000,false,"Death River");
            }
        }
        
        // Kiểm tra nếu là enemy
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Gọi method Die của enemy
                enemy.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
            }
        }
        
      
    }
} 