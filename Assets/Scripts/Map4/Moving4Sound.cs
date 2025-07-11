using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class Moving4Sound : MonoBehaviour
{
    public float speedThreshold = 0.1f; // Ngưỡng nhỏ để tránh phát khi rung nhẹ
    private Rigidbody2D rb;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true; // Bật lặp lại âm thanh
    }

    void Update()
    {
        float speed = rb.linearVelocity.magnitude;

        if (speed > speedThreshold)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Pause(); // hoặc Stop() nếu muốn cắt hẳn
        }
    }
}
