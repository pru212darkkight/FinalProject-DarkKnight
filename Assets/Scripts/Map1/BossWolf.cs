using UnityEngine;
using System.Collections;

public class BossWolf : Enemy
{
    public float dashSpeed = 10f;
    public float dashDuration = 0.3f;
    public float jumpForce = 18f;
    public float slamDelay = 0.5f;
    public float attackInterval = 2f;

    [Header("Flash Effect")]
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;
    public int flashCount = 3;

    private int attackStep = 0;
    private Vector2 originalPosition;
    private bool isAttacking = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    // Animator parameter hash
    private readonly int WalkHash = Animator.StringToHash("Walk");
    private readonly int DashHash = Animator.StringToHash("Dash");
    private readonly int JumpHash = Animator.StringToHash("Jump");
    private readonly int AttackHash = Animator.StringToHash("Attack");
    private readonly int DieHash = Animator.StringToHash("Die");

    protected override void Start()
    {
        base.Start();
       
        currentHealth = maxHealth;
        originalPosition = transform.position;
        
        // Lấy SpriteRenderer và màu gốc
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    protected override void Update()
    {
        if (isDead || player == null) return;
        // Kiểm tra mặt đất
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (animator != null)
        {
            animator.SetBool(IsGroundedHash, isGrounded);
        }
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
        {
            if (!isAttacking)
            {
                StartCoroutine(AttackPattern());
            }
        }
        else
        {
            // Dừng di chuyển nếu player ngoài tầm
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (animator != null)
            {
                animator.SetFloat(SpeedHash, 0);
            }
        }
    }

    private IEnumerator AttackPattern()
    {
        isAttacking = true;
        switch (attackStep)
        {
            case 0:
                yield return StartCoroutine(MoveToPlayerAndAttack());
                break;
            case 1:
                yield return StartCoroutine(DashThroughPlayer());
                break;
            case 2:
                yield return StartCoroutine(JumpAndSlam());
                break;
        }
        attackStep = (attackStep + 1) % 3;
        rb.linearVelocity = Vector2.zero;
        if (animator != null) animator.SetFloat(SpeedHash, 0);
        yield return new WaitForSeconds(attackInterval);
        isAttacking = false;
    }

    private IEnumerator MoveToPlayerAndAttack()
    {
        // Di chuyển đến trước mặt player với animation Walk
        if (animator != null) animator.SetTrigger(WalkHash);
        float targetX = player.position.x + (player.position.x > transform.position.x ? -1.2f : 1.2f);
        bool hasHitPlayer = false;
        
        while (Mathf.Abs(transform.position.x - targetX) > 0.1f && !hasHitPlayer)
        {
            float dir = targetX > transform.position.x ? 1 : -1;
            rb.linearVelocity = new Vector2(dir * moveSpeed*1.5f, rb.linearVelocity.y);
            if ((dir > 0 && !isFacingRight) || (dir < 0 && isFacingRight)) Flip();
            if (animator != null) animator.SetFloat(SpeedHash, Mathf.Abs(rb.linearVelocity.x));
            
            // Kiểm tra va chạm với player trong quá trình di chuyển
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, attackRange);
            foreach (Collider2D hitPlayer in hitPlayers)
            {
                if (hitPlayer.CompareTag("Player"))
                {
                    // Attack ngay khi gặp player
                    if (animator != null) animator.SetTrigger(AttackHash);
                    Attack();
                    hasHitPlayer = true;
                    break;
                }
            }
            
            yield return null;
        }
        
        rb.linearVelocity = Vector2.zero;
        if (animator != null) 
        {
            animator.SetFloat(SpeedHash, 0);
            animator.ResetTrigger(WalkHash);
        }
        
        // Nếu chưa gặp player, attack ở vị trí cuối
        if (!hasHitPlayer)
        {
            yield return new WaitForSeconds(0.2f);
            if (animator != null) animator.SetTrigger(AttackHash);
            Attack();
        }
    }

    private IEnumerator DashThroughPlayer()
    {
        // Lưu vị trí trước khi dash
        Vector2 dashStartPosition = transform.position;
        
        // Lướt nhanh qua player với animation Dash
        if (animator != null) animator.SetTrigger(DashHash);
        Vector2 dashDir = (player.position.x > transform.position.x) ? Vector2.right : Vector2.left;
        if ((dashDir.x > 0 && !isFacingRight) || (dashDir.x < 0 && isFacingRight)) Flip();
        float dashTime = 0f;
        bool hasHitPlayer = false;
        
        while (dashTime < dashDuration)
        {
            rb.linearVelocity = new Vector2(dashDir.x * dashSpeed, rb.linearVelocity.y);
            dashTime += Time.deltaTime;
            if (animator != null) animator.SetFloat(SpeedHash, Mathf.Abs(rb.linearVelocity.x));
            
            // Kiểm tra va chạm với player trong quá trình dash
            if (!hasHitPlayer)
            {
                Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, attackRange);
                foreach (Collider2D hitPlayer in hitPlayers)
                {
                    if (hitPlayer.CompareTag("Player"))
                    {
                        PlayerController1 playerController = hitPlayer.GetComponent<PlayerController1>();
                        if (playerController != null)
                        {
                            playerController.TakeDamage(damage);
                            hasHitPlayer = true;
                        }
                        break;
                    }
                }
            }
            
            yield return null;
        }
        rb.linearVelocity = Vector2.zero;
        if (animator != null) 
        {
            animator.SetFloat(SpeedHash, 0);
            animator.ResetTrigger(DashHash);
        }
        yield return new WaitForSeconds(0.2f);
        
        // Quay đầu lại hướng player sau khi dash lần đầu
        float direction = player.position.x > transform.position.x ? 1 : -1;
        if ((direction > 0 && !isFacingRight) || (direction < 0 && isFacingRight)) Flip();
        
        // Quay lại vị trí trước khi dash
        dashDir = (dashStartPosition.x > transform.position.x) ? Vector2.right : Vector2.left;
        dashTime = 0f;
        bool hasHitPlayer2 = false;
        while (Mathf.Abs(transform.position.x - dashStartPosition.x) > 0.1f && dashTime < 1.5f)
        {
            rb.linearVelocity = new Vector2(dashDir.x * dashSpeed, rb.linearVelocity.y);
            dashTime += Time.deltaTime;
            if (animator != null) animator.SetFloat(SpeedHash, Mathf.Abs(rb.linearVelocity.x));
            
            // Kiểm tra va chạm với player trong quá trình dash lần 2
            if (!hasHitPlayer2)
            {
                Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, attackRange);
                foreach (Collider2D hitPlayer in hitPlayers)
                {
                    if (hitPlayer.CompareTag("Player"))
                    {
                        PlayerController1 playerController = hitPlayer.GetComponent<PlayerController1>();
                        if (playerController != null)
                        {
                            playerController.TakeDamage(damage);
                            hasHitPlayer2 = true;
                        }
                        break;
                    }
                }
            }
            
            yield return null;
        }
        rb.linearVelocity = Vector2.zero;
        if (animator != null) animator.SetFloat(SpeedHash, 0);
        yield return new WaitForSeconds(0.1f);
         direction = player.position.x > transform.position.x ? 1 : -1;
        if ((direction > 0 && !isFacingRight) || (direction < 0 && isFacingRight)) Flip();
      
        
    }

    private IEnumerator JumpAndSlam()
    {
        // Nhảy lên cao với animation Jump
        if (animator != null) animator.SetTrigger(JumpHash);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce*3);
        if (animator != null) animator.SetBool(IsGroundedHash, false);
        yield return new WaitForSeconds(slamDelay);
        
        // Dậm xuống vị trí player
        float targetX = player.position.x;
        while (Mathf.Abs(transform.position.y - player.position.y) > 0.5f)
        {
            float dir = targetX > transform.position.x ? 1 : -1;
            rb.linearVelocity = new Vector2(dir * moveSpeed*4.3f, rb.linearVelocity.y);
            yield return null;
        }
        rb.AddForce(new Vector2(0, -jumpForce * 8f), ForceMode2D.Impulse);
        bool hasHitPlayer = false;
        
        if (!hasHitPlayer)
        {
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, attackRange);
            foreach (Collider2D hitPlayer in hitPlayers)
            {
                if (hitPlayer.CompareTag("Player"))
                {
                    PlayerController1 playerController = hitPlayer.GetComponent<PlayerController1>();
                    if (playerController != null)
                    {
                        playerController.TakeDamage(damage);
                        hasHitPlayer = true;
                    }
                    break;
                }
            }
        }
        
        yield return new WaitUntil(() => isGrounded);
        if (animator != null) 
        {
            animator.SetBool(IsGroundedHash, true);
            animator.ResetTrigger(JumpHash);
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public override void TakeDamage(float damage, bool isMagicDamage = false)
    {
        if (isDead) return;

        // Gọi hàm TakeDamage của lớp cha (Enemy)
        base.TakeDamage(damage, isMagicDamage);

        // Flash effect khi bị đánh
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashEffect());
        }

        // Nếu máu thấp, có thể thay đổi hành vi
        if (currentHealth < maxHealth * 0.3f) // Dưới 30% máu
        {
            // Tăng tốc độ tấn công hoặc thay đổi pattern
            attackInterval = Mathf.Max(attackInterval * 0.8f, 1f); // Giảm thời gian chờ tối thiểu 1s
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

    protected override void Die()
    {
        if (isDead) return;

        isDead = true;
        
        // Dừng tất cả hành động
        StopAllCoroutines();
        isAttacking = false;
        
        // Dừng di chuyển
        rb.linearVelocity = Vector2.zero;
        
        // Trigger death animation
        if (animator != null)
        {
            animator.SetTrigger(DieHash);
        }

        // Có thể thêm:
        // - Drop items
        // - Unlock doors
        // - Play victory sound
        // - Show victory UI
        // - Spawn particles
        // - v.v.

        // Gọi hàm Die của lớp cha
        base.Die();
    }
}