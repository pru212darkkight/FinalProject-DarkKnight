using UnityEngine;

public class BeholderAttack : MonoBehaviour
{
    public Transform player;               // Kéo Player vào Inspector
    public GameObject firePrefab;
    public Transform fireSpawnPoint;
    public float attackCooldown = 2f;

    private float lastAttackTime = -100f;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        // Nếu chưa kéo player, tìm theo tag "Player" (ưu tiên kéo tay trong Inspector)
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
    }

    public void Attack()
    {
        if (Time.time > lastAttackTime + attackCooldown && player != null)
        {
            lastAttackTime = Time.time;
            if (animator) animator.SetTrigger("Attack");

            if (firePrefab && fireSpawnPoint)
            {
                Vector2 dir = (player.position - fireSpawnPoint.position).normalized;
                GameObject fire = Instantiate(firePrefab, fireSpawnPoint.position, Quaternion.identity);
                FireBullet bullet = fire.GetComponent<FireBullet>();
                if (bullet != null)
                {
                    bullet.SetDirection(dir);
                }
            }
        }
    }
}
