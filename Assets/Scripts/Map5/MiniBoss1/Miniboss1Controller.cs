using UnityEngine;

public class MiniBoss1Controller : MonoBehaviour
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
    public bool returnToOrigin = true; // Tùy chỉnh miniboss có quay về vị trí gốc không

    [HideInInspector] public Vector3 startPoint;
    private Rigidbody2D rb;
    private Animator animator;
    private EnemyHealth enemyHealth;
    public MiniBoss1Attack miniBossAttack;

    private enum State { Idle, MovingToPlayer, Attacking, Hurt, Returning }
    private State state = State.Idle;
    private State stateBeforeHurt = State.Idle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        miniBossAttack = GetComponent<MiniBoss1Attack>();
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
                    if (returnToOrigin)
                        state = State.Returning;
                    else
                        state = State.Idle; // Đứng yên tại chỗ nếu không về gốc
                    break;
                }
                MoveToPlayer();
                break;


            case State.Attacking:
                if (!playerDetectedNow)
                {
                    state = State.Returning;
                }
                // Đợi animation gọi EndAttack() để chuyển state
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
        // Lấy center của vùng attackRange box
        Vector2 attackCenter = (Vector2)transform.position + attackRangeBoxOffset;
        bool inAttackRangeBox = Physics2D.OverlapBox(attackCenter, attackRangeBoxSize, 0, LayerMask.GetMask("Player"));
        // (Hoặc bạn có thể dùng code check thủ công khoảng cách player với center + attackRangeBoxSize)

        // Kiểm tra player nằm trong attackBoxSize chưa (chính là vùng cận chiến)
        int dir = transform.localScale.x > 0 ? -1 : 1;
        Vector2 meleeOffset = new Vector2(Mathf.Abs(miniBossAttack.attackBoxOffset.x) * dir, miniBossAttack.attackBoxOffset.y);
        Vector2 meleeCenter = (Vector2)transform.position + meleeOffset;
        bool inMeleeBox = Physics2D.OverlapBox(meleeCenter, miniBossAttack.attackBoxSize, 0, LayerMask.GetMask("Player"));

        LookAtTarget(player.position.x);

        // Nếu đã trong vùng box cast thì chỉ cast
        if (inAttackRangeBox)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsRunning", false);
            state = State.Attacking;
            // Chỉ gọi cast thôi nếu chưa đến cận chiến
            if (inMeleeBox)
                miniBossAttack.RandomAttack(true); // Cho phép đánh gần/cast
            else
                miniBossAttack.RandomAttack(false); // Chỉ cast
        }
        else
        {
            // Chưa vào vùng tấn công thì di chuyển về phía player
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

        stateBeforeHurt = state;
        state = State.Hurt;
        animator.SetTrigger("Hurt");
        rb.linearVelocity = Vector2.zero;
    }

    public void OnHurtEnd()
    {
        if (enemyHealth != null && enemyHealth.isDead) return;
        if (stateBeforeHurt == State.Attacking)
        {
            state = State.Attacking;

            // Kiểm tra player còn trong vùng melee không, truyền vào hàm attack
            int dir = transform.localScale.x > 0 ? -1 : 1;
            Vector2 meleeOffset = new Vector2(Mathf.Abs(miniBossAttack.attackBoxOffset.x) * dir, miniBossAttack.attackBoxOffset.y);
            Vector2 meleeCenter = (Vector2)transform.position + meleeOffset;
            bool inMeleeBox = Physics2D.OverlapBox(meleeCenter, miniBossAttack.attackBoxSize, 0, LayerMask.GetMask("Player"));

            miniBossAttack.RandomAttack(inMeleeBox); // Chỉ cho phép melee nếu player vẫn trong vùng đỏ
        }
        else if (stateBeforeHurt == State.MovingToPlayer)
        {
            state = State.MovingToPlayer;
        }
        else if (stateBeforeHurt == State.Returning)
        {
            state = State.Returning;
        }
        else
        {
            state = State.Idle;
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

    // SỬA ĐÚNG FLIP HƯỚNG NHƯ NÀY!
    void LookAtTarget(float targetX)
    {
        Vector3 scale = transform.localScale;
        // Nếu player bên phải mà miniboss đang nhìn trái (scale.x > 0) thì flip
        if (targetX > transform.position.x && scale.x > 0)
            scale.x = -scale.x;
        // Nếu player bên trái mà miniboss đang nhìn phải (scale.x < 0) thì flip
        else if (targetX < transform.position.x && scale.x < 0)
            scale.x = -scale.x;
        transform.localScale = scale;
    }



    // Method để gọi khi mini boss 1 chết (từ EnemyHealth script)
    public void OnDeath()
    {
        // 🎵 Play death sound
        if (AudioManager.Instance != null && AudioManager.Instance.miniBoss1Death != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.miniBoss1Death);
            Debug.Log("💀 MiniBoss1 died - playing death sound!");
        }
    }

    void OnDrawGizmosSelected()
    {
        if (Camera.main == null) return;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        if (screenPos.z > 0 &&
            screenPos.x >= 0 && screenPos.x <= Screen.width &&
            screenPos.y >= 0 && screenPos.y <= Screen.height)
        {
            // Vẽ vùng detect range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, new Vector3(detectRangeX * 2, detectRangeY * 2, 0.1f));

            // Vẽ vùng cast (attackRangeBox)
            Gizmos.color = Color.blue;
            Vector2 attackCenter = (Vector2)transform.position + attackRangeBoxOffset;
            Gizmos.DrawWireCube(attackCenter, attackRangeBoxSize);
        }
    }

}
