using UnityEngine;

public class SparkBullet : MonoBehaviour
{
    public float speed = 5f;
    public float damage = 20f;
    public float lifeTime = 3f;

    private Vector2 direction = Vector2.right;
    private float timer = 0f;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        // Xoay sprite cho đạn luôn chĩa về hướng bay (sprite mặc định nhìn phải)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var player = collision.GetComponent<PlayerController1>();
            if (player != null)
            {
                player.TakeDamage(damage, true, "Beholder");
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
