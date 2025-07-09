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
    public InputAction spell3Action;  // New spell 3 input
    public InputAction interactAction; // Interact (mở rương)

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
    public float attackTimeout = 1f;      // Timeout để tự động reset attack state

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
    private readonly int IsSpell3Hash = Animator.StringToHash("IsSpell3");
    private readonly int IsDefendingHash = Animator.StringToHash("IsDefend");
    private readonly int IsDashingHash = Animator.StringToHash("IsDash");
    private readonly int JumpHash = Animator.StringToHash("Jump");
    private readonly int AttackHash = Animator.StringToHash("Attack");
    private readonly int Attack2Hash = Animator.StringToHash("Attack2");
    private readonly int Attack3Hash = Animator.StringToHash("Attack3");
    private readonly int Spell1Hash = Animator.StringToHash("Spell1");
    private readonly int Spell2Hash = Animator.StringToHash("Spell2");
    private readonly int Spell3Hash = Animator.StringToHash("Spell3");
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
    // Spell 3 (Transform) settings
    public float spell3ManaCost = 50f;    // Mana cost for spell 3
    public float minManaForSpell3 = 50f;  // Minimum mana needed for spell 3
    public float spell3Cooldown = 10f;    // Cooldown for spell 3
    public float spell3Duration = 5f;     // Duration of transformation
    private float lastSpell3Time = -10f;  // Initialize to negative value so spell can be used immediately
    private bool isSpell3;
    private float spell3TimeLeft = 0;
    private Sprite originalSprite;

    [Header("hurt Effect Settings")]
    public float hurtStunDuration = 0.5f;
    public float hurtKnockbackForce = 5f;
    public Color hurtFlashColor = Color.red;
    public float hurtFlashDuration = 0.1f;
    public int hurtFlashCount = 3;

    [Header("Death & Respawn Settings")]
    public float respawnDelay = 2f;           // Thời gian chờ trước khi respawn
    public Vector3 respawnPosition;           // Vị trí respawn
    public bool useCheckpoint = true;         // Có sử dụng checkpoint không
    public float deathAnimationDuration = 1f; // Thời gian animation chết
    public GameObject gameOverUI;             // UI hiển thị khi game over
    public AudioClip deathSound;              // Âm thanh khi chết
    public AudioClip respawnSound;            // Âm thanh khi respawn

    private Vector2 moveInput;

    // Public properties để các script khác có thể access
    public Vector2 MoveInput => moveInput;
    public bool IsAttacking => isAttacking;
    public bool IsAttacking2 => isAttacking2;
    public bool IsAttacking3 => isAttacking3;
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
    private float lastSpell1Time;
    private float lastSpell2Time;
    private bool isDead = false;
    private bool isRespawning = false;
    private Vector3 lastCheckpoint;
    private AudioSource audioSource;

    ///trung doc 

    private float defaultSpeed;
    private bool isPoisoned = false;

    public Inventory inventory; // Gán qua Inspector hoặc tìm bằng code

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
        spell3Action.Enable();    // Enable spell 3 input
        interactAction.Enable();    // Enable interact input
        jumpAction.performed += OnJump;
        attackAction.performed += OnAttack;
        attack2Action.performed += OnAttack2;
        attack3Action.performed += OnAttack3;
        spell1Action.performed += OnSpell1;    // Add spell 1 handler
        spell2Action.performed += OnSpell2;    // Add spell 2 handler
        defendAction.started += OnDefendStart;    // Changed from performed to started
        defendAction.canceled += OnDefendEnd;       // Add defend end handler
        dashAction.performed += OnDash;    // Add dash handler
        spell3Action.performed += OnSpell3;    // Add spell 3 handler
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
        spell3Action.Disable();    // Disable spell 3 input
        interactAction.Disable();    // Disable interact input
        jumpAction.performed -= OnJump;
        attackAction.performed -= OnAttack;
        attack2Action.performed -= OnAttack2;
        attack3Action.performed -= OnAttack3;
        spell1Action.performed -= OnSpell1;    // Remove spell 1 handler
        spell2Action.performed -= OnSpell2;    // Remove spell 2 handler
        defendAction.started -= OnDefendStart;    // Changed from performed to started
        defendAction.canceled -= OnDefendEnd;       // Remove defend end handler
        dashAction.performed -= OnDash;    // Remove dash handler
        spell3Action.performed -= OnSpell3;    // Remove spell 3 handler
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
        stamina = maxStamina;
        mana = maxMana;
        originalColor = spriteRenderer.color;

        defaultSpeed = moveSpeed; // Lưu tốc độ gốc

        // Set initial respawn position
        if (respawnPosition == Vector3.zero)
        {
            respawnPosition = transform.position;
        }
        lastCheckpoint = respawnPosition;

        UpdateUI();
        ApplyEquipmentStats(true); // Hồi đầy máu/mana/stamina khi vào scene mới
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
        animator.SetBool(IsSpell3Hash, isSpell3);
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

        // Handle spell 3 (transform) duration
        if (isSpell3)
        {
            spell3TimeLeft -= Time.deltaTime;
            Debug.Log(spell3TimeLeft);
            if (spell3TimeLeft <= 0 && !isAttacking && !isAttacking2 && !isAttacking3 && !isSpell1 && !isSpell2)
            {
                EndSpell3();
            }
        }

        // Auto-reset attack states if they get stuck
        if (isAttacking && Time.time > lastAttackTime + attackTimeout)
        {
            Debug.LogWarning("Attack 1 state stuck, auto-resetting");
            OnAttackEnd();
        }
        if (isAttacking2 && Time.time > lastAttack2Time + attackTimeout)
        {
            Debug.LogWarning("Attack 2 state stuck, auto-resetting");
            OnAttack2End();
        }
        if (isAttacking3 && Time.time > lastAttack3Time + attackTimeout)
        {
            Debug.LogWarning("Attack 3 state stuck, auto-resetting");
            OnAttack3End();
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
                    EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        float damage = strength; // Basic attack damage
                        Debug.Log($"hurt enemy with Attack 1: {enemy.name} for {damage} damage");
                        enemyHealth.TakeDamage(damage, false); // false for physical damage
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
                    EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        float damage = strength * 1.5f; // Attack 2 deals 1.5x damage
                        Debug.Log($"hurt enemy with Attack 2: {enemy.name} for {damage} damage");
                        enemyHealth.TakeDamage(damage, false); // false for physical damage
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
                    EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        float damage = strength * 3f; // Attack 3 deals 3x damage
                        Debug.Log($"hurt enemy with Attack 3: {enemy.name} for {damage} damage");
                        enemyHealth.TakeDamage(damage, false); // false for physical damage
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
                    EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        float damage = strength * 4f;  // Spell 2 does the most damage
                        Debug.Log($"hurt enemy with Spell 2: {enemy.name} for {damage} damage");
                        enemyHealth.TakeDamage(damage, true); // false for physical damage
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
    ///Hieu ung trung doc
    private IEnumerator PoisonEffect()
    {
        isPoisoned = true;

        // Làm chậm 50% và đổi màu
        moveSpeed *= 0.5f;
        if (spriteRenderer != null)
            spriteRenderer.color = Color.purple;

        yield return new WaitForSeconds(2f); // thời gian bị độc

        // Khôi phục trạng thái
        moveSpeed = defaultSpeed;
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        isPoisoned = false;
    }

    private Coroutine poisonCoroutine;

    public void ApplyPoisonEffect(float duration, float slowFactor, Color effectColor)
    {
        // Nếu đang có hiệu ứng cũ thì huỷ và chạy lại
        if (poisonCoroutine != null)
            StopCoroutine(poisonCoroutine);

        poisonCoroutine = StartCoroutine(PoisonRoutine(duration, slowFactor, effectColor));
    }

    private IEnumerator PoisonRoutine(float duration, float slowFactor, Color effectColor)
    {
        float originalSpeed = moveSpeed;
        moveSpeed *= slowFactor;

        if (spriteRenderer != null)
            spriteRenderer.color = effectColor;

        yield return new WaitForSeconds(duration);

        moveSpeed = originalSpeed;

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        poisonCoroutine = null;
    }


    public void ApplyhurtEffects()
    {
        // Reset attack states when hurt to prevent stuck states
        if (isAttacking)
        {
            OnAttackEnd();
        }
        if (isAttacking2)
        {
            OnAttack2End();
        }
        if (isAttacking3)
        {
            OnAttack3End();
        }

        // Reset spell states when hurt (except spell3)
        if (isSpell1)
        {
            OnSpell1End();
        }
        if (isSpell2)
        {
            OnSpell2End();
        }

        // Trigger hurt animation
        animator.SetTrigger(HurtHash);
        animator.SetBool(IsHurtHash, true);

        // Apply hurt stun (shorter duration)
        ishurt = true;
        hurtStunTimeLeft = hurtStunDuration * 0.3f; // Reduce stun duration to 30%
    }



    private void Die()
    {
        if (isDead) return; // Prevent multiple death calls

        isDead = true;
        Debug.Log("Player died!");

        // Disable all input and movement
        DisablePlayerInput();

        // Stop all ongoing actions
        StopAllCoroutines();
        ResetAllStates();

        // Play death sound
        // if (audioSource != null && deathSound != null)
        // {
        //     audioSource.PlayOneShot(deathSound);
        // }

        // Trigger death animation
        animator.SetTrigger("Die");
        animator.SetBool("IsDead", true);

        // Disable collider to prevent further interactions
        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        // Stop movement
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;

        // Start death sequence
        StartCoroutine(DeathSequence());
    }

    private void DisablePlayerInput()
    {
        // Disable all input actions
        moveAction.Disable();
        jumpAction.Disable();
        attackAction.Disable();
        attack2Action.Disable();
        attack3Action.Disable();
        spell1Action.Disable();
        spell2Action.Disable();
        defendAction.Disable();
        dashAction.Disable();
        spell3Action.Disable();
    }

    private void EnablePlayerInput()
    {
        // Re-enable all input actions
        moveAction.Enable();
        jumpAction.Enable();
        attackAction.Enable();
        attack2Action.Enable();
        attack3Action.Enable();
        spell1Action.Enable();
        spell2Action.Enable();
        defendAction.Enable();
        dashAction.Enable();
        spell3Action.Disable();
    }

    private void ResetAllStates()
    {
        // Reset all combat states
        isAttacking = false;
        isAttacking2 = false;
        isAttacking3 = false;
        isSpell1 = false;
        isSpell2 = false;
        isDefending = false;
        isDashing = false;
        isSpell3 = false;
        ishurt = false;
        isJumping = false;

        // Reset animator states
        animator.SetBool(IsAttack1Hash, false);
        animator.SetBool(IsAttack2Hash, false);
        animator.SetBool(IsAttack3Hash, false);
        animator.SetBool(IsSpell1Hash, false);
        animator.SetBool(IsSpell2Hash, false);
        animator.SetBool(IsSpell3Hash, false);
        animator.SetBool(IsDefendingHash, false);
        animator.SetBool(IsDashingHash, false);
        animator.SetBool(IsHurtHash, false);
    }

    private System.Collections.IEnumerator DeathSequence()
    {
        // Wait for death animation
        yield return new WaitForSeconds(deathAnimationDuration);

        // Check if player has lives or should respawn
        if (ShouldRespawn())
        {
            // Start respawn process
            StartCoroutine(RespawnPlayer());
        }
        else
        {
            // Game over
            // GameOver();
        }
    }

    private bool ShouldRespawn()
    {
        // Không respawn tự động - chỉ game over
        return false;
    }

    private System.Collections.IEnumerator RespawnPlayer()
    {
        isRespawning = true;

        // Wait before respawn
        yield return new WaitForSeconds(respawnDelay);

        // Respawn player
        RespawnPlayerAtCheckpoint();

        // Play respawn sound
        if (audioSource != null && respawnSound != null)
        {
            audioSource.PlayOneShot(respawnSound);
        }

        // Reset death state
        isDead = false;
        isRespawning = false;

        // Re-enable player functionality
        EnablePlayerInput();
        ResetAllStates();

        // Re-enable collider
        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            playerCollider.enabled = true;
        }

        // Re-enable physics
        rb.isKinematic = false;

        // Reset animator
        animator.SetBool("IsDead", false);

        Debug.Log("Player respawned!");
    }

    private void RespawnPlayerAtCheckpoint()
    {
        // Determine respawn position
        Vector3 respawnPos = useCheckpoint ? lastCheckpoint : respawnPosition;

        // Move player to respawn position
        transform.position = respawnPos;

        // Reset health and stats
        currentHealth = maxHealth;
        stamina = maxStamina;
        mana = maxMana;

        // Reset spell3 transformation if active
        if (isSpell3)
        {
            EndSpell3();
        }

        // Update UI
        UpdateUI();

        // Reset sprite color
        spriteRenderer.color = originalColor;
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");

        // // Show game over UI
        // if (gameOverUI != null)
        // {
        //     gameOverUI.SetActive(true);
        // }

        // You can add more game over logic here:
        // - Save game state
        // - Show restart/quit options
        // - Play game over music
        // - etc.
    }

    // Public method to set checkpoint
    public void SetCheckpoint(Vector3 checkpointPosition)
    {
        if (useCheckpoint)
        {
            lastCheckpoint = checkpointPosition;
            Debug.Log($"Checkpoint set at: {checkpointPosition}");
        }
    }

    // Public method to force respawn (useful for debugging or special events)
    public void ForceRespawn()
    {
        if (isDead && !isRespawning)
        {
            StartCoroutine(RespawnPlayer());
        }
    }

    private void UpdateUI()
    {
        if (healthBar != null)
        {
            float healthPercent = currentHealth / maxHealth;
            healthBar.fillAmount = healthPercent;
        }
        else
        {
            Debug.LogWarning("PlayerController1: healthBar is null!");
        }

        if (staminaBar != null)
        {
            float staminaPercent = stamina / maxStamina;
            staminaBar.fillAmount = staminaPercent;
        }
        else
        {
            Debug.LogWarning("PlayerController1: staminaBar is null!");
        }

        if (manaBar != null)
        {
            float manaPercent = mana / maxMana;
            manaBar.fillAmount = manaPercent;
        }
        else
        {
            Debug.LogWarning("PlayerController1: manaBar is null!");
        }
    }

    private void OnSpell3(InputAction.CallbackContext context)
    {
        if (context.performed && !context.canceled)
        {
            if (Time.time >= lastSpell3Time + spell3Cooldown &&
                !isAttacking && !isAttacking2 && !isAttacking3 &&
                !isSpell1 && !isSpell2 &&
                mana >= minManaForSpell3)
            {
                // Trigger animation trước
                animator.SetTrigger(Spell3Hash);
                isSpell3 = true;

                // Set cooldown sau khi spell đã được kích hoạt
                lastSpell3Time = Time.time;

                // Tiêu thụ mana và cập nhật thời gian sử dụng mana
                mana -= spell3ManaCost;
                lastManaUseTime = Time.time;

                // Set thời gian biến hình
                spell3TimeLeft = spell3Duration;

                // Tăng chỉ số khi biến hình
                strength *= 2f;
                speed *= 1.5f;

                Debug.Log("Spell 3 (Transform) started");
                UpdateUI();
            }
        }
    }

    private void EndSpell3()
    {
        isSpell3 = false;
        // Trả lại chỉ số gốc
        strength /= 2f;
        speed /= 1.5f;
        Debug.Log("Spell 3 (Transform) ended");
    }

    public void ApplyEquipmentStats(bool resetVitals = false)
    {
        ResetBaseStats();
        if (inventory != null)
        {
            foreach (var item in inventory.equippedItems)
            {
                maxHealth += item.healthBonus;
                stamina += item.staminaBonus;
                maxStamina += item.staminaBonus; // hoặc item.maxStaminaBonus nếu có
                maxMana += item.manaBonus;    // hoặc item.maxManaBonus nếu có
                strength += item.strengthBonus;
                moveSpeed += item.moveSpeedBonus;
                armor += item.armorBonus;
                magicResist += item.magicResistBonus;
                healthRecoveryRate += item.healthRegenBonus;
                staminaRegenRate += item.staminaRegenBonus;
                manaRegenRate += item.manaRegenBonus;
                jumpForce += item.jumpBonus;
            }

            // Kiểm tra set bonus đơn giản
            CheckSetBonus();

            Debug.Log("Chỉ số đã được cập nhật!");
        }
        if (resetVitals)
        {
            currentHealth = maxHealth;
            mana = maxMana;
            stamina = maxStamina;
        }
        else
        {
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            mana = Mathf.Min(mana, maxMana);
            stamina = Mathf.Min(stamina, maxStamina);
        }
    }

    // Hàm kiểm tra set bonus đơn giản
    private void CheckSetBonus()
    {
        Debug.Log("Checking set bonus");
        if (inventory == null) return;
        Debug.Log("Inventory not null");
        // Set 1: INFERNO
        if (inventory.HasItem("S4")
        && inventory.HasItem("A1")
        && inventory.HasItem("H2")
        && inventory.HasItem("P1")
        && inventory.HasItem("B1")
        && inventory.HasItem("R4"))
        {
            strength += 20f;
            Debug.Log("Inferno set bonus");
        }

        // Set 2: WATER
        if (inventory.HasItem("S2")
        && inventory.HasItem("A3")
        && inventory.HasItem("H5")
        && inventory.HasItem("P5")
        && inventory.HasItem("B4")
        && inventory.HasItem("R2"))
        {
            maxMana += 100f;
            Debug.Log("Water set bonus");
        }

        // Set 3: NOBLE
        if (inventory.HasItem("ring_01") && inventory.HasItem("ring_02"))
        {
            staminaRegenRate += 5f;
            stamina += 100f;
            Debug.Log("Noble set bonus");
        }

        // Set 4: WITCH
        if (inventory.HasItem("S6")
        && inventory.HasItem("A2")
        && inventory.HasItem("H1")
        && inventory.HasItem("P2")
        && inventory.HasItem("B3")
        && inventory.HasItem("R1"))
        {
            Debug.Log("Witch set bonus");
            manaRegenRate += 5f;
        }
    }

    // Thêm hàm này để reset chỉ số gốc trước khi cộng thêm từ trang bị
    public void ResetBaseStats()
    {
        // Gán lại các chỉ số về giá trị gốc (có thể cần lưu các giá trị gốc này ở biến riêng nếu chỉ số có thể thay đổi trong runtime)
        maxHealth = 100f;
        maxStamina = 100f;
        maxMana = 100f;
        strength = 10f;
        moveSpeed = 5f;
        armor = 5f;
        magicResist = 5f;
        healthRecoveryRate = 2f;
        staminaRegenRate = 10f;
        manaRegenRate = 5f;
        jumpForce = 5f;
    }
}