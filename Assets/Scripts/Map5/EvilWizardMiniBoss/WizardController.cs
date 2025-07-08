using UnityEngine;

public class WizardController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public WizardAttack wizardAttack;

    [Header("Detection/Attack Area")]
    public float detectRangeX = 10f;
    public float detectRangeY = 2f;
    public Vector2 summonZoneSize = new Vector2(8f, 2f);    // Vùng xanh
    public Vector2 summonZoneOffset = new Vector2(0f, 0f);
    public Vector2 aoeZoneSize = new Vector2(3f, 1f);       // Vùng đỏ
    public Vector2 aoeZoneOffset = new Vector2(1f, 0f);

    public float moveSpeed = 2.5f;
    public float healthRegenRate = 5f;

    [HideInInspector] public Vector3 startPoint;
    private Rigidbody2D rb;
    private Animator animator;
    private EnemyHealth enemyHealth;

    private bool isReturning = false;
    private bool previousPlayerDetected = false;

    public float attackCooldown = 2f;  // cooldown mỗi đòn AOE
    private float lastAttackTime = -999f;
    private bool isAttacking = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        startPoint = transform.position;
        wizardAttack.playerRef = player;
    }

    void Update()
    {
        if (enemyHealth != null && enemyHealth.isDead)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsRunning", false);
            return;
        }

        Vector3 center = transform.position;
        bool isPlayerDetectedNow =
            Mathf.Abs(player.position.x - center.x) <= detectRangeX &&
            Mathf.Abs(player.position.y - center.y) <= detectRangeY;

        // Vùng xanh (summon zone)
        Vector2 summonZoneCenter = (Vector2)transform.position + summonZoneOffset;
        bool inSummonZone = Physics2D.OverlapBox(summonZoneCenter, summonZoneSize, 0, LayerMask.GetMask("Player"));

        // Vùng đỏ (AOE zone)
        float facing = Mathf.Sign(transform.localScale.x);
        Vector2 aoeZoneCenter = (Vector2)transform.position + new Vector2(aoeZoneOffset.x * facing, aoeZoneOffset.y);
        bool inAOEZone = Physics2D.OverlapBox(aoeZoneCenter, aoeZoneSize, 0, LayerMask.GetMask("Player"));

        // Xử lý return
        if (!isPlayerDetectedNow && previousPlayerDetected)
            isReturning = true;
        previousPlayerDetected = isPlayerDetectedNow;

        if (isPlayerDetectedNow)
        {
            isReturning = false;
            LookAtTarget(player.position.x);

            if (inAOEZone)
            {
                rb.linearVelocity = Vector2.zero;
                animator.SetBool("IsRunning", false);

                wizardAttack.AllowSummon(true);

                // Tấn công liên tục khi player còn ở vùng đỏ và đã hết cooldown
                if (Time.time - lastAttackTime >= attackCooldown && !isAttacking)
                {
                    animator.SetTrigger("Attack");
                    isAttacking = true;
                    lastAttackTime = Time.time;
                }
            }
            else if (inSummonZone)
            {
                rb.linearVelocity = Vector2.zero;
                animator.SetBool("IsRunning", false);

                wizardAttack.AllowSummon(true);
                isAttacking = false;
            }
            else
            {
                wizardAttack.AllowSummon(false);
                Vector2 target = new Vector2(player.position.x, rb.position.y);
                Vector2 newPos = Vector2.MoveTowards(rb.position, target, moveSpeed * Time.deltaTime);
                rb.MovePosition(newPos);
                animator.SetBool("IsRunning", true);
                isAttacking = false;
            }
        }
        // Quay về gốc khi mất dấu
        else if (isReturning)
        {
            float distToStart = Vector2.Distance(transform.position, startPoint);
            if (distToStart > 0.1f)
            {
                LookAtTarget(startPoint.x);
                Vector2 newPos = Vector2.MoveTowards(rb.position, startPoint, moveSpeed * Time.deltaTime);
                rb.MovePosition(newPos);
                animator.SetBool("IsRunning", true);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                animator.SetBool("IsRunning", false);
                isReturning = false;

                // Flip về phải mặc định
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x);
                transform.localScale = scale;
            }

            if (distToStart <= 0.15f && enemyHealth.currentHealth < enemyHealth.maxHealth)
            {
                enemyHealth.currentHealth += healthRegenRate * Time.deltaTime;
                enemyHealth.currentHealth = Mathf.Min(enemyHealth.currentHealth, enemyHealth.maxHealth);
            }
            wizardAttack.AllowSummon(false);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsRunning", false);
            if (enemyHealth.currentHealth < enemyHealth.maxHealth)
            {
                enemyHealth.currentHealth += healthRegenRate * Time.deltaTime;
                enemyHealth.currentHealth = Mathf.Min(enemyHealth.currentHealth, enemyHealth.maxHealth);
            }
            wizardAttack.AllowSummon(false);
        }
    }
    public void OnAttackEnd()
    {
        isAttacking = false;
    }
    void LookAtTarget(float targetX)
    {
        var scale = transform.localScale;
        // scale.x > 0 là phải, < 0 là trái
        if (targetX > transform.position.x && scale.x < 0)
            scale.x *= -1;
        else if (targetX < transform.position.x && scale.x > 0)
            scale.x *= -1;
        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        // Vùng xanh: summon zone
        Gizmos.color = Color.green;
        Vector2 summonZoneCenter = (Vector2)transform.position + summonZoneOffset;
        Gizmos.DrawWireCube(summonZoneCenter, summonZoneSize);

        // Vùng đỏ: aoe zone
        Gizmos.color = Color.red;
        float facing = Application.isPlaying ? Mathf.Sign(transform.localScale.x) : 1f;
        Vector2 aoeZoneCenter = (Vector2)transform.position + new Vector2(aoeZoneOffset.x * facing, aoeZoneOffset.y);
        Gizmos.DrawWireCube(aoeZoneCenter, aoeZoneSize);

        // Vùng detect
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(detectRangeX * 2, detectRangeY * 2, 0.1f));
    }
}
