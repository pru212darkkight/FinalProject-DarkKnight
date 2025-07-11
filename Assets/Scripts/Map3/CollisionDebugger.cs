using UnityEngine;

public class CollisionDebugger : MonoBehaviour 
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"🔥 COLLISION DEBUG: {gameObject.name} TRIGGER with {other.name} (Tag: {other.tag})");
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"🔥 COLLISION DEBUG: {gameObject.name} COLLISION with {collision.gameObject.name} (Tag: {collision.gameObject.tag})");
    }
    
    void OnTriggerStay2D(Collider2D other)
    {
        if (Time.frameCount % 60 == 0) // Log every second
        {
            Debug.Log($"🔄 COLLISION DEBUG: {gameObject.name} STAYING with {other.name}");
        }
    }
}
