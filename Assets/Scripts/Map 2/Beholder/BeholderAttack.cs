using UnityEngine;

public class BeholderAttack : MonoBehaviour
{
    public Transform player;
    public GameObject firePrefab;
    public Transform fireSpawnPoint;
    public float attackCooldown = 2f;

    private float lastAttackTime = -100f;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
    }

    public void Attack()
    {
        if (Time.time > lastAttackTime + attackCooldown && player != null && firePrefab && fireSpawnPoint)
        {
            lastAttackTime = Time.time;

            if (animator)
                animator.SetTrigger("Attack");

            // 🎵 Phát âm thanh tấn công của Beholder
            if (AudioManager.Instance != null && AudioManager.Instance.beholderAttack != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.beholderAttack);
                Debug.Log("👁️ Beholder attacking - playing attack sound!");
            }

            // CHỈ lấy X,Y, bỏ Z (phòng trường hợp player/fireSpawnPoint lệch Z)
            Vector2 spawnPos = new Vector2(fireSpawnPoint.position.x, fireSpawnPoint.position.y);
            Vector2 targetPos = new Vector2(player.position.x, player.position.y);
            Vector2 dir = (targetPos - spawnPos).normalized;

            GameObject fire = Instantiate(firePrefab, spawnPos, Quaternion.identity);

            var bullet = fire.GetComponent<SparkBullet>(); // Đúng tên script bullet của bạn!
            if (bullet != null)
            {
                bullet.SetDirection(dir);
            }
        }
    }
}
