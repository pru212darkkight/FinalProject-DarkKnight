using UnityEngine;

public class SkullBlastProjectile : MonoBehaviour
{
    private float speed;
    private float damage;
    public float lifeTime = 4f;

    private Vector2 direction = Vector2.right;
    private float timer = 0f;

    // Gọi khi tạo (truyền hướng, tốc độ, damage, layer player)
    public void Init(Vector2 dir, float spd, float dmg, LayerMask playerLayerMask)
    {
        direction = dir.normalized;
        speed = spd;
        damage = dmg;

        // Xoay viên đạn theo hướng bay (giống FireBullet)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        // Lật sprite nếu bắn qua trái
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.flipY = direction.x < 0; // hoặc flipX tùy sprite của bạn
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
        // Player
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
