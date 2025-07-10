using UnityEngine;

public class PlatformToggleMover : MonoBehaviour
{
    public Transform pointA; // Vị trí gốc (A)
    public Transform pointB; // Vị trí đích (B)
    public float moveSpeed = 2f;
    public AudioClip moveSound;

    private bool movingToB = false;
    private bool isMoving = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = moveSound;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (isMoving)
        {
            Vector2 target = movingToB ? pointB.position : pointA.position;
            transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            if (!audioSource.isPlaying)
            {
                audioSource.Play(); // Phát âm thanh khi bắt đầu di chuyển
            }

            if (Vector2.Distance(transform.position, target) < 0.01f)
            {
                isMoving = false;
                audioSource.Stop(); // Dừng âm thanh khi đến nơi
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Gắn player vào platform
            other.transform.SetParent(transform);

            if (!isMoving)
            {
                movingToB = !movingToB;
                isMoving = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Bỏ player ra khỏi platform
            other.transform.SetParent(null);
        }
    }
}
