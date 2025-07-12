using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SummonedEnemy4Controller : MonoBehaviour
{
    [Header("Combat Settings")]
    public float maxHealth = 30f;
    public float detectRange = 4f;
    public float attackRange = 1.2f;
    public float stopRange = 1.2f;
    public float moveSpeed = 2f;
    public float attackCooldown = 1.5f;
    public float attackDamage = 10f;

    private float currentHealth;
    private float lastAttackTime;

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;

    private bool isRebornDone = false;
    private bool isDead = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        animator.SetTrigger("Reborn");
        Invoke(nameof(EnableIdleState), 1f);
    }

    private void EnableIdleState()
    {
        isRebornDone = true;
        animator.SetBool("isIdle", true);
    }

    private void Update()
    {
        if (!isRebornDone || isDead || player == null) return;

        float distanceX = Mathf.Abs(player.position.x - transform.position.x);

        if (distanceX <= attackRange)
        {
            HandleAttack();
        }
        else if (distanceX <= detectRange)
        {
            MoveToPlayer();
        }
        else
        {
            Idle();
        }
    }

    private void MoveToPlayer()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isWalking", true);

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * moveSpeed, 0);

        // Flip sprite
        Vector3 scale = transform.localScale;
        if ((direction > 0 && scale.x < 0) || (direction < 0 && scale.x > 0))
        {
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    private void Idle()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isIdle", true);
        rb.linearVelocity = Vector2.zero;
    }

    private void HandleAttack()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isIdle", false);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            animator.SetTrigger("Attack");
        }
    }

    public void DealDamage()
    {
        if (player == null || isDead) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= stopRange + 0.3f)
        {
            PlayerController1 pc = player.GetComponent<PlayerController1>();
            pc?.TakeDamage(attackDamage, false, "Death");
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hurt");
        }
    }

    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Death");
        animator.SetBool("isIdle", false);
        animator.SetBool("isWalking", false);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Invoke(nameof(DestroySelf), 1f);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    public bool IsDead()
    {
        return isDead;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}
