using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public Transform raycastPoint;    // Điểm phía trước enemy (tầm đánh)
    public float moveSpeed = 3f;
    public float attackThreshold = 0.2f; // Khoảng lệch x để nhận biết vào tầm đánh
    public float attackCooldown = 1f;
    public int maxHealth = 100;
    public int currentHealth;
    public GameObject hitbox;

    private Rigidbody2D rb;
    private Animator anim;
    [HideInInspector] public bool isPlayerInTrigger = false;
    private float lastAttackTime;
    private bool isDead = false;

    private float startX;
    private float initialScaleX;

    public float minDistanceToPlayer = 0.15f; // <-- Khoảng cách tối thiểu giữa enemy và player

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        startX = transform.position.x;
        currentHealth = maxHealth;
        initialScaleX = transform.localScale.x; // lưu scale x ban đầu (-1 hoặc 1)
    }

    void Update()
    {
        if (isDead) return;

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (isPlayerInTrigger)
        {
            float deltaX = Mathf.Abs(player.position.x - raycastPoint.position.x);
            if (deltaX <= attackThreshold)
            {
                rb.linearVelocity = Vector2.zero;
                anim.SetBool("isRunning", false);

                // Chỉ attack khi KHÔNG ở state "Attack" và cooldown đã đủ
                if (!stateInfo.IsName("Attack") && Time.time - lastAttackTime > attackCooldown)
                {
                    Attack();
                    lastAttackTime = Time.time;
                }
            }
            else
            {
                // Giữ khoảng cách tối thiểu với player, không tiến sát quá
                float dir = player.position.x - transform.position.x;
                float distanceToPlayer = Mathf.Abs(dir);

                if (distanceToPlayer > minDistanceToPlayer)
                {
                    rb.linearVelocity = new Vector2(Mathf.Sign(dir) * moveSpeed, rb.linearVelocity.y);
                    anim.SetBool("isRunning", true);

                    // Flip sprite nếu cần
                    if ((dir > 0 && transform.localScale.x < 0) || (dir < 0 && transform.localScale.x > 0))
                    {
                        Vector3 scale = transform.localScale;
                        scale.x *= -1;
                        transform.localScale = scale;
                    }
                }
                else
                {
                    rb.linearVelocity = Vector2.zero;
                    anim.SetBool("isRunning", false);
                }
            }
        }
        else
        {
            float toStartX = startX - transform.position.x;
            if (Mathf.Abs(toStartX) > 0.1f)
            {
                rb.linearVelocity = new Vector2(Mathf.Sign(toStartX) * moveSpeed, rb.linearVelocity.y);
                anim.SetBool("isRunning", true);

                // Flip sprite theo hướng di chuyển về start
                if ((toStartX > 0 && transform.localScale.x < 0) || (toStartX < 0 && transform.localScale.x > 0))
                {
                    Vector3 scale = transform.localScale;
                    scale.x *= -1;
                    transform.localScale = scale;
                }
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                anim.SetBool("isRunning", false);

                // LUÔN set lại scale x về giá trị ban đầu
                Vector3 scale = transform.localScale;
                scale.x = initialScaleX;
                transform.localScale = scale;
            }
        }
    }

    void Attack()
    {
        anim.SetTrigger("attack");
    }

    // Animation event gọi để bật/tắt hitbox
    public void EnableHitbox()
    {
        if (hitbox != null)
            hitbox.SetActive(true);
    }
    public void DisableHitbox()
    {
        if (hitbox != null)
            hitbox.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        anim.SetTrigger("hurt");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        anim.SetTrigger("death");
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject, 2f);
    }
}
