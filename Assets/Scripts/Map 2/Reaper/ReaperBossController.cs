using System.Collections;
using UnityEngine;

public class ReaperBossController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public float detectRangeX = 5f;
    public float detectRangeY = 2f;
    public Vector2 attackRangeBoxSize = new Vector2(4f, 2f);
    public Vector2 attackRangeBoxOffset = new Vector2(0f, 0f);
    public float moveSpeed = 2f;
    public float healthRegenRate = 3f;

    [Header("AI Options")]
    public bool returnToOrigin = true;

    [HideInInspector] public Vector3 startPoint;
    private Rigidbody2D rb;
    private Animator animator;
    private EnemyHealth enemyHealth;
    public ReaperBossAttack reaperAttack;

    [Header("Flash Effect")]
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;
    public int flashCount = 3;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Audio")]
    private bool wasPlayerDetected = false; // Để track khi player mới vào vùng detect

    private enum State { Idle, MovingToPlayer, Attacking, Hurt, Returning }
    private State state = State.Idle;
    private State stateBeforeHurt = State.Idle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        reaperAttack = GetComponent<ReaperBossAttack>();
        startPoint = transform.position;
    }

    void Update()
    {
        if (state == State.Hurt)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsRunning", false);
            return;
        }

        if (enemyHealth != null && enemyHealth.isDead)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsRunning", false);
            return;
        }

        bool playerDetectedNow = Mathf.Abs(player.position.x - transform.position.x) <= detectRangeX &&
                                 Mathf.Abs(player.position.y - transform.position.y) <= detectRangeY;

        // 🎵 Phát âm thanh khi player mới vào vùng detect
        if (playerDetectedNow && !wasPlayerDetected)
        {
            if (AudioManager.Instance != null && AudioManager.Instance.reaperDetect != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.reaperDetect);
                Debug.Log("💀 Reaper Boss detected player - playing detect sound!");
            }
        }
        wasPlayerDetected = playerDetectedNow;

        switch (state)
        {
            case State.Idle:
                animator.SetBool("IsRunning", false);
                if (playerDetectedNow)
                    state = State.MovingToPlayer;
                else
                    RegenerateHealth();
                break;

            case State.MovingToPlayer:
                if (!playerDetectedNow)
                {
                    state = returnToOrigin ? State.Returning : State.Idle;
                    break;
                }
                MoveToPlayer();
                break;

            case State.Attacking:
                if (!playerDetectedNow)
                    state = State.Returning;
                break;

            case State.Returning:
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
                    animator.SetBool("IsRunning", false);
                    state = State.Idle;
                }
                RegenerateHealth();
                break;
        }
    }

    void MoveToPlayer()
    {
        Vector2 attackCenter = (Vector2)transform.position + attackRangeBoxOffset;
        bool inAttackRangeBox = Physics2D.OverlapBox(attackCenter, attackRangeBoxSize, 0, LayerMask.GetMask("Player"));

        int dir = transform.localScale.x > 0 ? -1 : 1;
        Vector2 meleeOffset = new Vector2(Mathf.Abs(reaperAttack.attackBoxOffset.x) * dir, reaperAttack.attackBoxOffset.y);
        Vector2 meleeCenter = (Vector2)transform.position + meleeOffset;
        bool inMeleeBox = Physics2D.OverlapBox(meleeCenter, reaperAttack.attackBoxSize, 0, LayerMask.GetMask("Player"));

        LookAtTarget(player.position.x);

        if (inAttackRangeBox)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsRunning", false);
            state = State.Attacking;
            reaperAttack.RandomAttack(inMeleeBox);
        }
        else
        {
            Vector2 target = new Vector2(player.position.x, rb.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, moveSpeed * Time.deltaTime);
            rb.MovePosition(newPos);
            animator.SetBool("IsRunning", true);
        }
    }

    public void EndAttack()
    {
        if (state != State.Attacking) return;

        if (Mathf.Abs(player.position.x - transform.position.x) <= detectRangeX &&
            Mathf.Abs(player.position.y - transform.position.y) <= detectRangeY)
        {
            state = State.MovingToPlayer;
        }
        else
        {
            state = State.Returning;
        }
    }

    public void OnTakeDamage()
    {
        if (state == State.Hurt) return;
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashEffect());
        }
        stateBeforeHurt = state;
        state = State.Hurt;
        animator.SetTrigger("Hurt");
        rb.linearVelocity = Vector2.zero;

        // 🎵 Phát âm thanh khi boss chết
        if (enemyHealth != null && enemyHealth.currentHealth <= 0 && AudioManager.Instance != null && AudioManager.Instance.reaperDeath != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.reaperDeath);
            Debug.Log("💀 Reaper Boss died - playing death sound!");
        }
    }
    private IEnumerator FlashEffect()
    {
        for (int i = 0; i < flashCount; i++)
        {
            // Chuyển sang màu flash
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);

            // Trở về màu gốc
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
    }
    public void OnHurtEnd()
    {
        if (enemyHealth != null && enemyHealth.isDead) return;

        if (stateBeforeHurt == State.Attacking)
        {
            state = State.Attacking;

            int dir = transform.localScale.x > 0 ? -1 : 1;
            Vector2 meleeOffset = new Vector2(Mathf.Abs(reaperAttack.attackBoxOffset.x) * dir, reaperAttack.attackBoxOffset.y);
            Vector2 meleeCenter = (Vector2)transform.position + meleeOffset;
            bool inMeleeBox = Physics2D.OverlapBox(meleeCenter, reaperAttack.attackBoxSize, 0, LayerMask.GetMask("Player"));

            reaperAttack.RandomAttack(inMeleeBox);
        }
        else
        {
            state = stateBeforeHurt;
        }
    }

    void RegenerateHealth()
    {
        if (enemyHealth.currentHealth < enemyHealth.maxHealth)
        {
            enemyHealth.currentHealth += healthRegenRate * Time.deltaTime;
            enemyHealth.currentHealth = Mathf.Min(enemyHealth.currentHealth, enemyHealth.maxHealth);
        }
    }

    void LookAtTarget(float targetX)
    {
        Vector3 scale = transform.localScale;

        // Nếu player bên trái mà boss đang quay phải => lật trái
        if (targetX < transform.position.x && scale.x > 0)
            scale.x = -scale.x;

        // Nếu player bên phải mà boss đang quay trái => lật phải
        else if (targetX > transform.position.x && scale.x < 0)
            scale.x = -scale.x;

        transform.localScale = scale;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(detectRangeX * 2, detectRangeY * 2, 0.1f));

        Gizmos.color = Color.blue;
        Vector2 attackCenter = (Vector2)transform.position + attackRangeBoxOffset;
        Gizmos.DrawWireCube(attackCenter, attackRangeBoxSize);

        if (reaperAttack != null)
        {
            Gizmos.color = Color.red;
            int dir = transform.localScale.x > 0 ? 1 : -1;
            Vector2 offset = new Vector2(Mathf.Abs(reaperAttack.attackBoxOffset.x) * dir, reaperAttack.attackBoxOffset.y);
            Vector2 center = (Vector2)transform.position + offset;
            Gizmos.DrawWireCube(center, reaperAttack.attackBoxSize);

            Gizmos.color = Color.cyan;
            float px = player != null ? player.position.x : transform.position.x;
            float baseY = transform.position.y + reaperAttack.thunderHeight;

            Vector3[] thunderPositions = new Vector3[]
            {
                new Vector3(px, baseY, 0),
                new Vector3(px - reaperAttack.thunderDistance, baseY, 0),
                new Vector3(px + reaperAttack.thunderDistance, baseY, 0)
            };

            foreach (var pos in thunderPositions)
                Gizmos.DrawWireSphere(pos, 0.15f);
        }
    }
}