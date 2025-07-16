using UnityEngine;

public class FallingRock : MonoBehaviour
{
    [Header("Tùy chỉnh thời gian rơi")]
    [Tooltip("Thời gian chờ trước khi viên đá bắt đầu rơi")]
    public float delayBeforeFall = 0.5f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.bodyType = RigidbodyType2D.Static; // Ban đầu đứng yên
    }

    public void TriggerFall()
    {
        Invoke(nameof(StartFalling), delayBeforeFall);
    }

    private void StartFalling()
    {
        if (rb != null)
            rb.bodyType = RigidbodyType2D.Dynamic;
    }
}
