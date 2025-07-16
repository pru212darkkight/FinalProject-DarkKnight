using UnityEngine;

public class RockTriggerZone : MonoBehaviour
{
    private FallingRock parentRock;

    void Awake()
    {
        parentRock = GetComponentInParent<FallingRock>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parentRock.TriggerFall();
        }
    }
}
