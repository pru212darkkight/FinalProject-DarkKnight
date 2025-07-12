using UnityEngine;

public class FixPlayerPhysics : MonoBehaviour
{
    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Fix physics settings for normal platformer gameplay
            rb.gravityScale = 1f;
            rb.mass = 1f;
            rb.linearDamping = 0f;
            rb.angularDamping = 0.05f;
            rb.freezeRotation = true;
            
            Debug.Log("Player physics fixed for Map 3!");
        }
        
        // Remove this script after fixing
        Destroy(this);
    }
}
