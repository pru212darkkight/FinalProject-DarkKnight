using UnityEngine;

public class DeathTrap : MonoBehaviour
{
    [Header("Trap Settings")]
    [SerializeField] private bool affectPlayer = true;
    [SerializeField] private bool affectEnemies = true;
    [SerializeField] private bool affectBosses = true;
    [SerializeField] private bool destroyOnTrigger = false; // Có hủy bẫy sau khi kích hoạt không
    
    [Header("Visual Effects")]
    [SerializeField] private bool showDeathEffect = true;
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private Color trapColor = Color.red;
    [SerializeField] private bool pulseEffect = true;
    [SerializeField] private float pulseSpeed = 2f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip trapSound;
    [SerializeField] private AudioClip deathSound;
    
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isPulsing = false;
    
    void Start()
    {
        // Setup components
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            spriteRenderer.color = trapColor;
        }
        
        // Setup collider nếu chưa có
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
            boxCollider.isTrigger = true;
        }
        else
        {
            collider.isTrigger = true;
        }
        
        // Bắt đầu hiệu ứng pulse nếu được bật
        if (pulseEffect && spriteRenderer != null)
        {
            StartPulseEffect();
        }
        
        Debug.Log($"DeathTrap initialized - Player: {affectPlayer}, Enemies: {affectEnemies}, Bosses: {affectBosses}");
    }
    
    void Update()
    {
        // Hiệu ứng pulse
        if (pulseEffect && spriteRenderer != null && isPulsing)
        {
            float alpha = 0.5f + 0.5f * Mathf.Sin(Time.time * pulseSpeed);
            Color pulseColor = trapColor;
            pulseColor.a = alpha;
            spriteRenderer.color = pulseColor;
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem có phải player không
        if (affectPlayer && other.CompareTag("Player"))
        {
            HandlePlayerDeath(other.gameObject);
            return;
        }
        
        // Kiểm tra xem có phải enemy không
        if (affectEnemies && other.CompareTag("Enemy"))
        {
            HandleEnemyDeath(other.gameObject);
            return;
        }
        
        // Kiểm tra xem có phải boss không
        if (affectBosses && other.CompareTag("Boss"))
        {
            HandleBossDeath(other.gameObject);
            return;
        }
        
        // Kiểm tra các component cụ thể
        if (other.GetComponent<PlayerController1>() != null && affectPlayer)
        {
            HandlePlayerDeath(other.gameObject);
        }
        else if (other.GetComponent<Enemy>() != null && affectEnemies)
        {
            HandleEnemyDeath(other.gameObject);
        }
        else if (other.GetComponent<BossWolf>() != null && affectBosses)
        {
            HandleBossDeath(other.gameObject);
        }
    }
    
    void HandlePlayerDeath(GameObject player)
    {
        Debug.Log("Player fell into death trap!");
        
        // Play sound
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        
        // Show death effect
        if (showDeathEffect)
        {
            ShowDeathEffect(player.transform.position);
        }
        
        // Kill player
        PlayerController1 playerController = player.GetComponent<PlayerController1>();
        if (playerController != null)
        {
            // Gọi method chết của player (nếu có)
            if (playerController.GetType().GetMethod("Die") != null)
            {
                playerController.SendMessage("Die");
            }
            else
            {
                // Fallback: Destroy player
                Destroy(player);
            }
        }
        else
        {
            // Fallback: Destroy player
            Destroy(player);
        }
        
        // Destroy trap nếu được set
        if (destroyOnTrigger)
        {
            Destroy(gameObject);
        }
    }
    
    void HandleEnemyDeath(GameObject enemy)
    {
        Debug.Log($"Enemy {enemy.name} fell into death trap!");
        
        // Play sound
        if (audioSource != null && trapSound != null)
        {
            audioSource.PlayOneShot(trapSound);
        }
        
        // Show death effect
        if (showDeathEffect)
        {
            ShowDeathEffect(enemy.transform.position);
        }
        
        // Kill enemy
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            // Gọi method chết của enemy (nếu có)
            if (enemyComponent.GetType().GetMethod("Die") != null)
            {
                enemyComponent.SendMessage("Die");
            }
            else
            {
                // Fallback: Destroy enemy
                Destroy(enemy);
            }
        }
        else
        {
            // Fallback: Destroy enemy
            Destroy(enemy);
        }
        
        // Destroy trap nếu được set
        if (destroyOnTrigger)
        {
            Destroy(gameObject);
        }
    }
    
    void HandleBossDeath(GameObject boss)
    {
        Debug.Log($"Boss {boss.name} fell into death trap!");
        
        // Play sound
        if (audioSource != null && trapSound != null)
        {
            audioSource.PlayOneShot(trapSound);
        }
        
        // Show death effect
        if (showDeathEffect)
        {
            ShowDeathEffect(boss.transform.position);
        }
        
        // Kill boss
        BossWolf bossComponent = boss.GetComponent<BossWolf>();
        if (bossComponent != null)
        {
            // Gọi method chết của boss (nếu có)
            if (bossComponent.GetType().GetMethod("Die") != null)
            {
                bossComponent.SendMessage("Die");
            }
            else
            {
                // Fallback: Destroy boss
                Destroy(boss);
            }
        }
        else
        {
            // Fallback: Destroy boss
            Destroy(boss);
        }
        
        // Destroy trap nếu được set
        if (destroyOnTrigger)
        {
            Destroy(gameObject);
        }
    }
    
    void ShowDeathEffect(Vector3 position)
    {
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, position, Quaternion.identity);
        }
        else
        {
            // Tạo hiệu ứng đơn giản
            CreateSimpleDeathEffect(position);
        }
    }
    
    void CreateSimpleDeathEffect(Vector3 position)
    {
        // Tạo particle effect đơn giản
        GameObject effect = new GameObject("DeathEffect");
        effect.transform.position = position;
        
        // Thêm ParticleSystem
        ParticleSystem particles = effect.AddComponent<ParticleSystem>();
        var main = particles.main;
        main.startLifetime = 1f;
        main.startSpeed = 3f;
        main.startSize = 0.5f;
        main.startColor = Color.red;
        main.maxParticles = 20;
        
        var emission = particles.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0f, 20)
        });
        
        // Tự hủy sau 2 giây
        Destroy(effect, 2f);
    }
    
    void StartPulseEffect()
    {
        isPulsing = true;
    }
    
    void StopPulseEffect()
    {
        isPulsing = false;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = trapColor;
        }
    }
    
    // Public methods để điều khiển từ bên ngoài
    public void SetTrapActive(bool active)
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = active;
        }
        
        if (active)
        {
            StartPulseEffect();
        }
        else
        {
            StopPulseEffect();
        }
    }
    
    public void SetAffectPlayer(bool affect)
    {
        affectPlayer = affect;
    }
    
    public void SetAffectEnemies(bool affect)
    {
        affectEnemies = affect;
    }
    
    public void SetAffectBosses(bool affect)
    {
        affectBosses = affect;
    }
    
    // Gizmos để hiển thị trong Scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
} 