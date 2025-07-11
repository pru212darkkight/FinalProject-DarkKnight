using UnityEngine;

public class SimpleBossCollisionTest : MonoBehaviour 
{
    public float damageAmount = 15f;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"🔥 TRIGGER: Boss collided with {other.name}, Tag: {other.tag}");
        
        if (other.tag == "Player")
        {
            PlayerController1 player = other.GetComponent<PlayerController1>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);
                Debug.Log($"🩸 SIMPLE TEST: Boss dealt {damageAmount} damage to player!");
            }
            else
            {
                Debug.LogWarning("Player found but no PlayerController1 component!");
            }
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"🔥 COLLISION: Boss collided with {collision.gameObject.name}, Tag: {collision.gameObject.tag}");
        
        if (collision.gameObject.tag == "Player")
        {
            PlayerController1 player = collision.gameObject.GetComponent<PlayerController1>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);
                Debug.Log($"🩸 SIMPLE TEST: Boss dealt {damageAmount} damage to player!");
            }
            else
            {
                Debug.LogWarning("Player found but no PlayerController1 component!");
            }
        }
    }
}
