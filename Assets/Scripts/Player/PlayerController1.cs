using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class PlayerController1 : MonoBehaviour
{
    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction attackAction;
    public InputAction attack2Action;  // New attack 2 input
    public InputAction attack3Action;  // New attack 3 input
    public InputAction spell1Action;  // New spell 1 input
    public InputAction spell2Action;  // New spell 2 input
    public InputAction defendAction;  // New defend input
    public InputAction dashAction;  // New dash input

    private Animator animator;
    private Rigidbody2D rb;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Attack Settings")]
    public float attackCooldown = 0.5f;
    public float attack2Cooldown = 0.8f;  // Longer cooldown for attack 2
    public float attack3Cooldown = 1.2f;  // Even longer cooldown for attack 3
    public float spell1Cooldown = 2f;    // Cooldown for spell 1
    public float spell2Cooldown = 3f;    // Cooldown for spell 2
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public float attack2Range = 0.7f;     // Longer range for attack 2
    public float attack3Range = 1f;       // Even longer range for attack 3
    public float spell1Range = 5f;       // Range for spell 1
    public float spell2Range = 8f;       // Range for spell 2
    public LayerMask enemyLayer;

    [Header("Animation Settings")]
    private readonly int SpeedHash = Animator.StringToHash("Speed");
    private readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private readonly int IsJumpingHash = Animator.StringToHash("IsJumping");
    private readonly int IsFallingHash = Animator.StringToHash("IsFalling");
    private readonly int IsAttack1Hash = Animator.StringToHash("IsAttack");
    private readonly int IsAttack2Hash = Animator.StringToHash("IsAttack2");
    private readonly int IsAttack3Hash = Animator.StringToHash("IsAttack3");
    private readonly int IsSpell1Hash = Animator.StringToHash("IsSpell1");
    private readonly int IsSpell2Hash = Animator.StringToHash("IsSpell2");
    private readonly int IsDefendingHash = Animator.StringToHash("IsDefend");
    private readonly int IsDashingHash = Animator.StringToHash("IsDash");
    private readonly int JumpHash = Animator.StringToHash("Jump");
    private readonly int AttackHash = Animator.StringToHash("Attack");
    private readonly int Attack2Hash = Animator.StringToHash("Attack2");
    private readonly int Attack3Hash = Animator.StringToHash("Attack3");
    private readonly int Spell1Hash = Animator.StringToHash("Spell1");
    private readonly int Spell2Hash = Animator.StringToHash("Spell2");
    private readonly int DefendHash = Animator.StringToHash("Defend");
    private readonly int DashHash = Animator.StringToHash("Dash");
    private readonly int LandedHash = Animator.StringToHash("Landed");
    private readonly int HurtHash = Animator.StringToHash("Hurt");
    private readonly int IsHurtHash = Animator.StringToHash("IsHurt");

    [Header("Character Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float strength = 10f;      // Sức mạnh
    public float stamina = 100f;      // Thể lực
    public float maxStamina = 100f;
    public float mana = 100f;         // Mana
    public float maxMana = 100f;      // Max mana
    public float speed = 5f;          // Tốc độ
    public float armor = 5f;          // Giáp
    public float magicResist = 5f;    // Kháng phép

    [Header("Health Recovery Settings")]
    public float healthRecoveryRate = 2f;    // Base health recovery per second
    public float healthRegenDelay = 5f;      // Delay before health starts regenerating
    private float lastDamageTime;            // Last time player took damage

    [Header("Stamina Settings")]
    public float staminaRegenRate = 5f;    // Stamina regeneration rate
    public float staminaRegenDelay = 0.5f; // Delay before stamina starts regenerating
    private float lastStaminaUseTime;

    [Header("Mana Settings")]
    public float manaRegenRate = 5f;    // Mana regeneration rate
    public float manaRegenDelay = 0.5f; // Delay before mana starts regenerating
    private float lastManaUseTime;

    [Header("UI Elements")]
    public Image healthBar;
    public Image staminaBar;
    public Image manaBar;

    [Header("Attack Settings")]
    public float staminaToHealthRatio = 0.5f; // How much stamina affects health recovery
    public float minStaminaForRecovery = 20f; // Minimum stamina needed for health recovery

    [Header("Defend Settings")]
    public float defendStaminaCost = 10f;  // Stamina cost per second while defending
    public float minStaminaToDefend = 20f; // Minimum stamina needed to start defending

    [Header("Dash Settings")]
    public float dashForce = 20f;        // Lực dash
    public float dashDuration = 0.2f;    // Thời gian dash
    public float dashCooldown = 1f;      // Thời gian hồi dash
    public float dashStaminaCost = 30f;  // Chi phí stamina cho mỗi lần dash
    public float minStaminaToDash = 30f; // Lượng stamina tối thiểu để dash

    [Header("Spell Settings")]
    public float spell1ManaCost = 20f;    // Mana cost for spell 1
    public float spell2ManaCost = 40f;    // Mana cost for spell 2
    public float minManaForSpell1 = 20f;  // Minimum mana needed for spell 1
    public float minManaForSpell2 = 40f;  // Minimum mana needed for spell 2
    public GameObject fireSpellPrefab;     // Prefab cho chưởng lửa
    public Transform spellSpawnPoint;      // Điểm sinh ra chưởng lửa

    [Header("hurt Effect Settings")]
    public float hurtStunDuration = 0.5f;
    public float hurtKnockbackForce = 5f;
    public Color hurtFlashColor = Color.red;
    public float hurtFlashDuration = 0.1f;
    public int hurtFlashCount = 3;

    private Vector2 moveInput;
    private bool isJumping;
    private bool isGrounded;
    private bool wasGroundedLastFrame;
    private bool isAttacking;
    private bool isAttacking2;
    private bool isAttacking3;
    private float lastAttackTime;
    private float lastAttack2Time;
    private float lastAttack3Time;
    private bool isSpell1;
    private bool isSpell2;
    private float lastSpell1Time;
    private float lastSpell2Time;
    private bool isDefending;
    private float lastDefendTime;
    private bool isDashing;
    private float lastDashTime;
    private float dashTimeLeft;
    private Vector2 dashDirection;
    private SpriteRenderer spriteRenderer;
    private bool ishurt = false;
    private float hurtStunTimeLeft = 0f;
    private Color originalColor;

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        attackAction.Enable();
        attack2Action.Enable();
        attack3Action.Enable();
        spell1Action.Enable();    // Enable spell 1 input
        spell2Action.Enable();    // Enable spell 2 input
        defendAction.Enable();    // Enable defend input
        dashAction.Enable();    // Enable dash input
        jumpAction.performed += OnJump;
        attackAction.performed += OnAttack;
        attack2Action.performed += OnAttack2;
        attack3Action.performed += OnAttack3;
        spell1Action.performed += OnSpell1;    // Add spell 1 handler
        spell2Action.performed += OnSpell2;    // Add spell 2 handler
        defendAction.started += OnDefendStart;    // Changed from performed to started
        defendAction.canceled += OnDefendEnd;       // Add defend end handler
        dashAction.performed += OnDash;    // Add dash handler
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        attackAction.Disable();
        attack2Action.Disable();
        attack3Action.Disable();
        spell1Action.Disable();    // Disable spell 1 input
        spell2Action.Disable();    // Disable spell 2 input
        defendAction.Disable();    // Disable defend input
        dashAction.Disable();    // Disable dash input
        jumpAction.performed -= OnJump;
        attackAction.performed -= OnAttack;
        attack2Action.performed -= OnAttack2;
        attack3Action.performed -= OnAttack3;
        spell1Action.performed -= OnSpell1;    // Remove spell 1 handler
        spell2Action.performed -= OnSpell2;    // Remove spell 2 handler
        defendAction.started -= OnDefendStart;    // Changed from performed to started
        defendAction.canceled -= OnDefendEnd;       // Remove defend end handler
        dashAction.performed -= OnDash;    // Remove dash handler
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        stamina = maxStamina;
        mana = maxMana;
        originalColor = spriteRenderer.color;
        UpdateUI();
    }

    void Update()
    {
        // Read movement input
        moveInput = moveAction.ReadValue<Vector2>();

        // Flip sprite based on direction
        if (moveInput.x > 0.01f)
            transform.localScale = new Vector3(1.7f, 1.7f, 1);
        else if (moveInput.x < -0.01f)
            transform.localScale = new Vector3(-1.7f, 1.7f, 1);

        // Ground check
        wasGroundedLastFrame = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Set animator parameters
        animator.SetFloat(SpeedHash, Mathf.Abs(moveInput.x));
        animator.SetBool(IsGroundedHash, isGrounded);
        animator.SetBool(IsJumpingHash, !isGrounded && rb.linearVelocity.y > 0.1f);
        animator.SetBool(IsFallingHash, !isGrounded && rb.linearVelocity.y < -0.1f);
        animator.SetBool(IsAttack1Hash, isAttacking);
        animator.SetBool(IsAttack2Hash, isAttacking2);
        animator.SetBool(IsAttack3Hash, isAttacking3);
        animator.SetBool(IsSpell1Hash, isSpell1);
        animator.SetBool(IsSpell2Hash, isSpell2);
        animator.SetBool(IsDashingHash, isDashing);    // Set dash animation parameter
        if (isDefending)
        {
            animator.SetBool(IsDefendingHash, true);
        }

        // Handle dropDown (landing) animation trigger
        if (!wasGroundedLastFrame && isGrounded)
        {
            animator.SetTrigger(LandedHash);
        }

        // Handle hurt stun (only affects movement speed)
        if (ishurt)
        {
            hurtStunTimeLeft -= Time.deltaTime;
            if (hurtStunTimeLeft <= 0)
            {
                Debug.Log("Hurt stun ended");
                ishurt = false;
                animator.SetBool(IsHurtHash, false);
            }
        }

        // Stamina regeneration
        if (Time.time > lastStaminaUseTime + staminaRegenDelay)
        {
            stamina = Mathf.Min(stamina + staminaRegenRate * Time.deltaTime, maxStamina);
            UpdateUI();
        }

        // Mana regeneration
        if (Time.time > lastManaUseTime + manaRegenDelay)
        {
            mana = Mathf.Min(mana + manaRegenRate * Time.deltaTime, maxMana);
            UpdateUI();
        }

        // Health regeneration - independent of stamina
        if (Time.time > lastDamageTime + healthRegenDelay && currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min(currentHealth + healthRecoveryRate * Time.deltaTime, maxHealth);
            UpdateUI();
        }

        // Handle defend stamina cost
        if (isDefending)
        {
            stamina = Mathf.Max(0, stamina - defendStaminaCost * Time.deltaTime);
            if (stamina <= 0)
            {
                OnDefendEnd(new InputAction.CallbackContext());
            }
            UpdateUI();
        }

        // Handle dash duration
        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0)
            {
                EndDash();
            }
        }
    }

    private void FixedUpdate()
    {
        Vector2 velocity = rb.linearVelocity;

        if (isDashing)
        {
            // Apply dash force
            velocity = dashDirection * dashForce;
        }
        else if (!isDefending)
        {
            if (!isAttacking && !isAttacking2 && !isAttacking3)
            {
                // Reduce movement speed while hurt
                float moveSpeedMultiplier = ishurt ? 0.5f : 1f;
                velocity.x = moveInput.x * moveSpeed * moveSpeedMultiplier;
            }
            else
            {
                velocity.x = moveInput.x * moveSpeed * 0.3f;
            }
        }
        else
        {
            // Stop movement while defending
            velocity.x = 0;
        }

        if (isJumping && isGrounded)
        {
            velocity.y = jumpForce;
            isJumping = false;
        }

        rb.linearVelocity = velocity;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded && !isAttacking && !isAttacking2 && !isAttacking3)
        {
            isJumping = true;
            animator.SetTrigger(JumpHash);
        }
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        // Only trigger on button press, not hold
        if (context.performed && !context.canceled)
        {
            if (Time.time >= lastAttackTime + attackCooldown && 
                !isAttacking && !isAttacking2 && !isAttacking3 && 
                !isSpell1 && !isSpell2)
            {
                isAttacking = true;
                lastAttackTime = Time.time;
                animator.SetTrigger(AttackHash);
                animator.SetBool(IsAttack1Hash, true);
                // Reset hurt state when attacking
                ishurt = false;
                animator.SetBool(IsHurtHash, false);
                Debug.Log("Attack started");

                // Perform attack hurt detection
                Collider2D[] hurtEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
                foreach (Collider2D enemy in hurtEnemies)
                {
                    Enemy enemyComponent = enemy.GetComponent<Enemy>();
                    if (enemyComponent != null)
                    {
                        float damage = strength; // Basic attack damage
                        Debug.Log($"hurt enemy with Attack 1: {enemy.name} for {damage} damage");
                        enemyComponent.TakeDamage(damage, false); // false for physical damage
                    }
                }
            }
        }
    }

    public void OnAttackEnd()
    {
        isAttacking = false;
        animator.SetBool(IsAttack1Hash, false);
        Debug.Log("Attack 1 ended");
    }

    private void OnAttack2(InputAction.CallbackContext context)
    {
        // Only trigger on button press, not hold
        if (context.performed && !context.canceled)
        {
            if (Time.time >= lastAttack2Time + attack2Cooldown && 
                !isAttacking && !isAttacking2 && !isAttacking3 && 
                !isSpell1 && !isSpell2)
            {
                isAttacking2 = true;
                lastAttack2Time = Time.time;
                animator.SetTrigger(Attack2Hash);
                animator.SetBool(IsAttack2Hash, true);
                // Reset hurt state when attacking
                ishurt = false;
                animator.SetBool(IsHurtHash, false);
                Debug.Log("Attack 2 started");

                // Perform attack 2 hurt detection
                Collider2D[] hurtEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attack2Range, enemyLayer);
                foreach (Collider2D enemy in hurtEnemies)
                {
                    Enemy enemyComponent = enemy.GetComponent<Enemy>();
                    if (enemyComponent != null)
                    {
                        float damage = strength * 1.5f; // Attack 2 deals 1.5x damage
                        Debug.Log($"hurt enemy with Attack 2: {enemy.name} for {damage} damage");
                        enemyComponent.TakeDamage(damage, false); // false for physical damage
                    }
                }
            }
        }
    }

    public void OnAttack2End()
    {
        isAttacking2 = false;
        animator.SetBool(IsAttack2Hash, false);
        Debug.Log("Attack 2 ended");
    }

    private void OnAttack3(InputAction.CallbackContext context)
    {
        // Only trigger on button press, not hold
        if (context.performed && !context.canceled)
        {
            if (Time.time >= lastAttack3Time + attack3Cooldown && 
                !isAttacking && !isAttacking2 && !isAttacking3 && 
                !isSpell1 && !isSpell2)
            {
                isAttacking3 = true;
                lastAttack3Time = Time.time;
                animator.SetTrigger(Attack3Hash);
                animator.SetBool(IsAttack3Hash, true);
                // Reset hurt state when attacking
                ishurt = false;
                animator.SetBool(IsHurtHash, false);
                Debug.Log("Attack 3 started");

                // Perform attack 3 hurt detection
                Collider2D[] hurtEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attack3Range, enemyLayer);
                foreach (Collider2D enemy in hurtEnemies)
                {
                    Enemy enemyComponent = enemy.GetComponent<Enemy>();
                    if (enemyComponent != null)
                    {
                        float damage = strength * 3f; // Attack 3 deals 3x damage
                        Debug.Log($"hurt enemy with Attack 3: {enemy.name} for {damage} damage");
                        enemyComponent.TakeDamage(damage, false); // false for physical damage
                    }
                }
            }
        }
    }

    public void OnAttack3End()
    {
        isAttacking3 = false;
        animator.SetBool(IsAttack3Hash, false);
        Debug.Log("Attack 3 ended");
    }

    private void OnSpell1(InputAction.CallbackContext context)
    {
        // Only trigger on button press, not hold
        if (context.performed && !context.canceled)
        {
            if (Time.time >= lastSpell1Time + spell1Cooldown && 
                !isAttacking && !isAttacking2 && !isAttacking3 && 
                !isSpell1 && !isSpell2 &&
                mana >= minManaForSpell1)  // Check if we have enough mana
            {
                isSpell1 = true;
                lastSpell1Time = Time.time;
                mana -= spell1ManaCost;    // Consume mana
                lastManaUseTime = Time.time;  // Update last mana use time
                animator.SetTrigger(Spell1Hash);
                animator.SetBool(IsSpell1Hash, true);
                // Reset hurt state when casting spell
                ishurt = false;
                animator.SetBool(IsHurtHash, false);
                Debug.Log("Spell 1 started");

                // Tạo chưởng lửa với delay nhỏ
                StartCoroutine(SpawnFireballWithDelay(0.3f)); // Delay 0.3 giây

                UpdateUI();  // Update UI to show mana consumption
            }
        }
    }

    private System.Collections.IEnumerator SpawnFireballWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Tạo chưởng lửa
        if (fireSpellPrefab != null && spellSpawnPoint != null)
        {
            Debug.Log("Creating fire spell");
            // Xác định hướng bắn dựa trên hướng nhân vật
            Vector2 direction = new Vector2(transform.localScale.x > 0 ? 1 : -1, 0);
            Debug.Log($"Fire spell direction: {direction}");
            
            // Tạo cầu lửa và khởi tạo ngay lập tức
            GameObject fireSpell = Instantiate(fireSpellPrefab, spellSpawnPoint.position, Quaternion.identity);
            FireSpellEffect fireEffect = fireSpell.GetComponent<FireSpellEffect>();
            if (fireEffect != null)
            {
                Debug.Log("Initializing fire spell effect");
                fireEffect.Initialize(direction, strength);
            }
            else
            {
                Debug.LogError("FireSpellEffect component not found on prefab!");
            }
        }
        else
        {
            Debug.LogError("Fire spell prefab or spawn point is null!");
        }
    }

    public void OnSpell1End()
    {
        isSpell1 = false;
        animator.SetBool(IsSpell1Hash, false);
        Debug.Log("Spell 1 ended");
    }

    private void OnSpell2(InputAction.CallbackContext context)
    {
        // Only trigger on button press, not hold
        if (context.performed && !context.canceled)
        {
            if (isGrounded && Time.time >= lastSpell2Time + spell2Cooldown && 
                !isAttacking && !isAttacking2 && !isAttacking3 && 
                !isSpell1 && !isSpell2 &&
                mana >= minManaForSpell2)  // Check if we have enough mana
            {
                isSpell2 = true;
                lastSpell2Time = Time.time;
                mana -= spell2ManaCost;    // Consume mana
                lastManaUseTime = Time.time;  // Update last mana use time
                animator.SetTrigger(Spell2Hash);
                animator.SetBool(IsSpell2Hash, true);
                // Reset hurt state when casting spell
                ishurt = false;
                animator.SetBool(IsHurtHash, false);
                Debug.Log("Spell 2 started");

                // Perform spell 2 hurt detection
                Collider2D[] hurtEnemies = Physics2D.OverlapCircleAll(attackPoint.position, spell2Range, enemyLayer);
                foreach (Collider2D enemy in hurtEnemies)
                {
                    Enemy enemyComponent = enemy.GetComponent<Enemy>();
                    if (enemyComponent != null)
                    {
                        float damage = strength * 4f;  // Spell 2 does the most damage
                        Debug.Log($"hurt enemy with Spell 2: {enemy.name} for {damage} damage");
                        enemyComponent.TakeDamage(damage, true); // true for magic damage
                    }
                }

                UpdateUI();  // Update UI to show mana consumption
            }
        }
    }

    public void OnSpell2End()
    {
        isSpell2 = false;
        animator.SetBool(IsSpell2Hash, false);
        Debug.Log("Spell 2 ended");
    }

    private void OnDefendStart(InputAction.CallbackContext context)
    {
        if (isGrounded && stamina >= minStaminaToDefend && !isDefending && !isAttacking && !isAttacking2 && !isAttacking3 && !isSpell1 && !isSpell2)
        {
            isDefending = true;
            animator.SetTrigger(DefendHash);
            animator.SetBool(IsDefendingHash, true);
            Debug.Log("Defend started");
        }
    }

    private void OnDefendEnd(InputAction.CallbackContext context)
    {
        if (isDefending)
        {
            isDefending = false;
            animator.SetBool(IsDefendingHash, false);
            Debug.Log("Defend ended");
        }
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (Time.time >= lastDashTime + dashCooldown && 
            stamina >= minStaminaToDash && 
            !isDashing && !isDefending && 
            !isAttacking && !isAttacking2 && !isAttacking3 && 
            !isSpell1 && !isSpell2)
        {
            // Determine dash direction based on input, but only horizontal
            dashDirection = new Vector2(moveInput.x, 0).normalized;
            if (dashDirection.magnitude < 0.1f)
            {
                // If no input, dash in facing direction
                dashDirection = new Vector2(transform.localScale.x > 0 ? 1 : -1, 0);
            }

            // Start dash
            isDashing = true;
            dashTimeLeft = dashDuration;
            lastDashTime = Time.time;
            stamina -= dashStaminaCost;
            lastStaminaUseTime = Time.time;

            // Trigger dash animation
            animator.SetTrigger(DashHash);
            animator.SetBool(IsDashingHash, true);
            Debug.Log("Dash started");

            UpdateUI();
        }
    }

    private void EndDash()
    {
        isDashing = false;
        animator.SetBool(IsDashingHash, false);
        Debug.Log("Dash ended");
    }

    // Update OnDrawGizmosSelected to show spell ranges
    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            // Attack ranges
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, attack2Range);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackPoint.position, attack3Range);

            // Spell ranges
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(attackPoint.position, spell1Range);

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(attackPoint.position, spell2Range);
        }
    }

    public void TakeDamage(float damage, bool isMagicDamage = false)
    {
        // If defending, prevent all damage
        if (isDefending)
        {
            Debug.Log("Damage blocked!");
            return;
        }

        float finalDamage = damage;
        if (isMagicDamage)
        {
            finalDamage *= (1 - (magicResist / 100f)); // Reduce magic damage based on magic resistance
        }
        else
        {
            finalDamage *= (1 - (armor / 100f)); // Reduce physical damage based on armor
        }

        currentHealth = Mathf.Max(0, currentHealth - finalDamage);
        lastDamageTime = Time.time; // Update last damage time
        UpdateUI();

        // Apply hurt effects
        ApplyhurtEffects();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void ApplyhurtEffects()
    {
        // Trigger hurt animation
        animator.SetTrigger(HurtHash);
        animator.SetBool(IsHurtHash, true);

        // Apply hurt stun (shorter duration)
        ishurt = true;
        hurtStunTimeLeft = hurtStunDuration * 0.3f; // Reduce stun duration to 30%

        // Apply knockback (reduced force)
        Vector2 knockbackDirection = new Vector2(-transform.localScale.x, 0.5f).normalized;
        rb.linearVelocity = Vector2.zero; // Reset velocity
        rb.AddForce(knockbackDirection * (hurtKnockbackForce * 0.5f), ForceMode2D.Impulse); // Reduce knockback force to 50%

        // Start hurt flash effect
        StartCoroutine(hurtFlashEffect());
    }

    private System.Collections.IEnumerator hurtFlashEffect()
    {
        for (int i = 0; i < hurtFlashCount; i++)
        {
            spriteRenderer.color = hurtFlashColor;
            yield return new WaitForSeconds(hurtFlashDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(hurtFlashDuration);
        }
    }

    private void Die()
    {
        // Handle player death
        Debug.Log("Player died!");
        // Add death animation, game over logic, etc.
    }

    private void UpdateUI()
    {
        if (healthBar != null)
            healthBar.fillAmount = currentHealth / maxHealth;
        if (staminaBar != null)
            staminaBar.fillAmount = stamina / maxStamina;
        if (manaBar != null)
            manaBar.fillAmount = mana / maxMana;
    }
}
