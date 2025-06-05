using UnityEngine;

public class FireSpellEffect : MonoBehaviour
{
    public float lifetime = 2f;     // Thời gian tồn tại của hiệu ứng
    public float moveSpeed = 15f;   // Tốc độ di chuyển của chưởng lửa
    public float damageMultiplier = 2.5f;  // Hệ số sát thương của spell 1
    public LayerMask enemyLayer;    // Layer của kẻ địch

    private SpriteRenderer spriteRenderer;
    private Vector2 moveDirection;
    private bool isInitialized = false;
    private float baseStrength;     // Sức mạnh cơ bản từ player

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log("FireSpellEffect Awake called");
    }

    void Start()
    {
        Debug.Log("FireSpellEffect Start called");
        // Tự động hủy sau thời gian lifetime
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector2 direction, float strength)
    {
        
        // Đảm bảo hướng di chuyển chỉ theo chiều ngang
        moveDirection = new Vector2(direction.x, 0).normalized;
        baseStrength = strength;
        
        // Lật sprite nếu di chuyển sang trái
        if (moveDirection.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized)
        {

            return;
        }

        // Di chuyển chưởng lửa theo chiều ngang
        Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
        transform.position += movement;

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu va chạm với kẻ địch
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            // Gây sát thương cho kẻ địch
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                float damage = baseStrength * damageMultiplier;
                Debug.Log($"Dealing {damage} damage to enemy");
                enemy.TakeDamage(damage, true); // true để đánh dấu là sát thương phép thuật
            }

            // TODO: Thêm animation nổ ở đây nếu có
            Destroy(gameObject); // Hủy sau khi nổ
        }
    }
} 