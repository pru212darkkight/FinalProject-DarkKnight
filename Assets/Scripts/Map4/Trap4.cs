using UnityEngine;

public class Trap4 : MonoBehaviour
{
    [Header("Trap Settings")]
    public Animator trapAnimator;
    public float fallSpeed = 5f;
    public bool useGravity = true;

    [Header("Detection Settings")]
    public float detectionRadius = 1.5f; // Bán kính vùng phát hiện player

    private bool triggered = false;
    private bool falling = false;
    private Rigidbody2D rb;

    void Start()
    {
        // Gán Rigidbody nếu dùng gravity
        if (useGravity)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Kinematic; // Không rơi ngay
            }
        }

        // Tự tìm Animator nếu chưa gán
        if (trapAnimator == null)
        {
            trapAnimator = GetComponent<Animator>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;

            if (trapAnimator != null)
            {
                trapAnimator.SetTrigger("Activate"); // Chạy anim cảnh báo
            }
        }
    }

    // Gọi từ Animation Event ở cuối clip cảnh báo
    public void StartFalling()
    {
        if (useGravity && rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; // Bắt đầu rơi tự do
        }
        else
        {
            falling = true; // Dùng code để rơi
        }

        // Tự hủy sau 3 giây kể từ khi bắt đầu rơi
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        if (falling)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Vẽ vùng phát hiện player (tham khảo bán kính detection)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
