using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float height = 2f;                  // Chiều cao bay lên tối đa
    public float speed = 2f;                   // Tốc độ di chuyển
    public float damage = 20f;                 // Sát thương
    public bool flipWhenGoingDown = true;      // Lật sprite khi rơi xuống

    private float startY;                      // Vị trí Y ban đầu
    private float lastY;                       // Lưu vị trí Y để kiểm tra hướng
    private SpriteRenderer spriteRenderer;     // Sprite để flip
    private bool isGoingDown = false;          // Cờ trạng thái đang rơi

    void Start()
    {
        startY = transform.position.y;
        lastY = startY;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Tính vị trí mới theo PingPong
        float time = Time.time * speed;
        float offset = Mathf.PingPong(time, height);
        float newY = startY + offset;

        // Kiểm tra hướng rơi/thăng
        bool currentlyGoingDown = newY < lastY;

        if (flipWhenGoingDown && spriteRenderer != null)
        {
            if (currentlyGoingDown != isGoingDown)
            {
                isGoingDown = currentlyGoingDown;
                spriteRenderer.flipY = isGoingDown;
            }
        }

        // Cập nhật vị trí
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        lastY = newY;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var player = collision.GetComponent<PlayerController1>();
            if (player != null)
            {
                player.TakeDamage(damage, false, "Fireball");
            }
        }
    }
}
