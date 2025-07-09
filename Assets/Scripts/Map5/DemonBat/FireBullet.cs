using UnityEngine;

public class FireBullet : MonoBehaviour
{
    public float speed = 5f;
    public float damage = 20f;
    public float lifeTime = 3f;

    private Vector2 direction = Vector2.right;
    private float timer = 0f;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        // Xoay viên đạn theo hướng bay
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 180f;
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
                player.TakeDamage(damage, true);
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
