using UnityEngine;

public class PlatformToggleMover : MonoBehaviour
{
    public Transform pointA; // Vị trí gốc (A)
    public Transform pointB; // Vị trí đích (B)
    public float moveSpeed = 2f;

    private bool movingToB = false;
    private bool isMoving = false;
    private Transform playerOnPlatform = null;

    void Update()
    {
        if (isMoving)
        {
            Vector2 target = movingToB ? pointB.position : pointA.position;
            transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, target) < 0.01f)
            {
                isMoving = false; // Dừng khi đến nơi
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
                movingToB = !movingToB; // Đảo chiều di chuyển
                isMoving = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Bỏ player ra khỏi platform khi rời đi
            other.transform.SetParent(null);
        }
    }
}
