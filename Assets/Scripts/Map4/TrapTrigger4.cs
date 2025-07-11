using UnityEngine;

public class TrapTrigger4 : MonoBehaviour
{
    [Header("Gán bẫy cần điều khiển")]
    public Trap4 trapToActivate;

    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            trapToActivate?.ActivateTrap();
        }
    }
}
