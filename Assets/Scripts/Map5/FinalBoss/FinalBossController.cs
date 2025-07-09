using UnityEngine;

public class FinalBossController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public FinalBossAttack finalBossAttack;

    [Header("Attack Area")]
    public Vector2 attackRangeBoxSize = new Vector2(6f, 3f);
    public Vector2 attackRangeBoxOffset = Vector2.zero;

    [Header("Skill/Timing")]
    public float attackCooldown = 2f;

    [Header("Damage/Teleport")]
    public float damageThreshold = 100f;   // Tổng damage nhận được để tăng dame & teleport
    public float damageIncrease = 10f;     // Mỗi lần đạt threshold sẽ cộng thêm damage này
    public float teleportRangeMinX = -8f;  // Khoảng dịch chuyển (tính từ vị trí bắt đầu)
    public float teleportRangeMaxX = 8f;

    private Vector3 startPoint;
    private float lastAttackTime;
    private float damageAccumulated;
    private Rigidbody2D rb;
    private Animator animator;
    private EnemyHealth enemyHealth;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        finalBossAttack = GetComponent<FinalBossAttack>();
        startPoint = transform.position;
    }

    void Update()
    {
        if (enemyHealth != null && enemyHealth.isDead)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsRunning", false);
            return;
        }

        // Kiểm tra player có trong vùng attackRangeBox không
        Vector2 attackCenter = (Vector2)transform.position + attackRangeBoxOffset;
        bool playerInAttackBox = Physics2D.OverlapBox(
            attackCenter,
            attackRangeBoxSize,
            0,
            LayerMask.GetMask("Player")
        );

        if (playerInAttackBox)
        {
            LookAtPlayer();
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsRunning", false);

            // Kiểm tra cooldown trước khi tấn công tiếp
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                int attackType = Random.Range(1, 4); // 1 = Thánh Giá, 2 = Mặt Trăng, 3 = Chưởng Đầu Lâu
                finalBossAttack.DoAttack(attackType);
                lastAttackTime = Time.time;
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsRunning", false);
        }
    }

    // Gọi từ EnemyHealth với sát thương đã được trừ armor/magicResist
    public void OnTakeDamage(float realDamage)
    {
        damageAccumulated += realDamage;
        if (damageAccumulated >= damageThreshold)
        {
            // Đổi tên đúng field của FinalBossAttack
            finalBossAttack.crossDamage += damageIncrease;        // Thánh Giá
            finalBossAttack.moonDamage += damageIncrease;         // Mặt Trăng
            finalBossAttack.skullBlastDamage += damageIncrease;   // Chưởng Đầu Lâu

            Teleport();
            damageAccumulated -= damageThreshold; // Giữ lại phần dư nếu bị dồn damage
        }
    }
    void LookAtPlayer()
    {
        if (player == null) return;
        Vector3 scale = transform.localScale;
        // Mặc định scale.x > 0 là nhìn phải, < 0 là nhìn trái (hoặc ngược lại tùy sprite)
        if (player.position.x > transform.position.x && scale.x < 0)
            scale.x = -scale.x;
        else if (player.position.x < transform.position.x && scale.x > 0)
            scale.x = -scale.x;
        transform.localScale = scale;
    }


    // Dịch chuyển boss đến vị trí ngẫu nhiên trong khoảng cho phép (theo trục X)
    void Teleport()
    {
        float minX = startPoint.x + teleportRangeMinX;
        float maxX = startPoint.x + teleportRangeMaxX;
        float newX = Random.Range(minX, maxX);
        transform.position = new Vector3(newX, startPoint.y, startPoint.z);
        // Nếu cần, gọi VFX teleport ở đây
    }

    // Vẽ vùng attack range box trên scene
    void OnDrawGizmosSelected()
    {
        if (Camera.main == null) return;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        if (screenPos.z > 0 &&
            screenPos.x >= 0 && screenPos.x <= Screen.width &&
            screenPos.y >= 0 && screenPos.y <= Screen.height)
        {
            Gizmos.color = Color.blue;
            Vector2 attackCenter = (Vector2)transform.position + attackRangeBoxOffset;
            Gizmos.DrawWireCube(attackCenter, attackRangeBoxSize);
        }
    }
}
