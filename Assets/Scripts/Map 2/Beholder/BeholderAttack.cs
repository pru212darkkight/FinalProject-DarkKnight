using UnityEngine;

public class BeholderAttack : MonoBehaviour
{
    public Transform player;                 // Kéo Player vào Inspector
    public GameObject firePrefab;            // Prefab đạn lửa
    public Transform fireSpawnPoint;         // Vị trí sinh đạn
    public float attackCooldown = 2f;        // Thời gian hồi mỗi lần bắn

    private float lastAttackTime = -100f;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();

        // Nếu chưa kéo Player vào, tự động tìm theo Tag
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

            // Gọi animation bắn
            if (animator)
                animator.SetTrigger("Attack");

            // Bắn đạn
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
