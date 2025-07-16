using UnityEngine;

public class WallUnlocker : MonoBehaviour
{
    [Header("Enemy cần theo dõi")]
    public GameObject enemy;

    [Header("Tường chắn sẽ biến mất")]
    public GameObject wall;

    private EnemyHealth enemyHealth;

    void Start()
    {
        if (enemy != null)
        {
            enemyHealth = enemy.GetComponent<EnemyHealth>();
        }
    }

    void Update()
    {
        // Nếu enemy bị tiêu diệt (isDead) hoặc enemy đã bị Destroy (null)
        if ((enemy == null || (enemyHealth != null && enemyHealth.isDead)) && wall != null)
        {
            // Gỡ bỏ collider và renderer (tường biến mất hoàn toàn)
            Collider2D wallCollider = wall.GetComponent<Collider2D>();
            if (wallCollider != null)
                Destroy(wallCollider);

            SpriteRenderer sr = wall.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.enabled = false;

            // Nếu muốn gỡ hoàn toàn object tường thì dùng dòng sau thay vì hai dòng trên:
            // Destroy(wall);

            this.enabled = false; // Tắt script sau khi hoàn tất
        }
    }
}
