using UnityEngine;

public class PortalController : MonoBehaviour
{
    [Header("References")]
    public Animator animator; // Animator của portal
    public GameObject playerObject; // Player đã có sẵn trong scene
    public Transform spawnPoint; // Vị trí spawn nhân vật (thường là vị trí cổng)

    public float delayBeforeAppear = 0.5f; // Thời gian chờ trước khi portal hiện ra

    private SpriteRenderer spriteRenderer;
    private Collider2D portalCollider;
    private bool hasSpawnedPlayer = false;

    void Awake()
    {
        // Lấy component
        spriteRenderer = GetComponent<SpriteRenderer>();
        portalCollider = GetComponent<Collider2D>();

        // Kiểm tra components
        if (spriteRenderer == null)
        {
            Debug.LogError("PortalController: SpriteRenderer not found!");
            return;
        }

        // Ẩn portal lúc đầu
        spriteRenderer.enabled = false;
        if (portalCollider != null) portalCollider.enabled = false;

        // Kiểm tra player object
        if (playerObject != null)
        {
            SpriteRenderer playerSprite = playerObject.GetComponent<SpriteRenderer>();
            if (playerSprite != null)
            {
                playerSprite.enabled = false;
            }
            else
            {
                Debug.LogWarning("PortalController: Player SpriteRenderer not found!");
            }
        }
        else
        {
            Debug.LogError("PortalController: Player Object is not assigned!");
        }
    }

    void Start()
    {
        // Sau delay, hiện portal và play animation xuất hiện
        Invoke(nameof(ShowPortal), delayBeforeAppear);
    }

    void ShowPortal()
    {
        // Phát âm thanh teleport - kiểm tra null trước
        if (AudioManager.Instance != null && AudioManager.Instance.teleportMusic != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.teleportMusic);
        }

        spriteRenderer.enabled = true;
        if (portalCollider != null) portalCollider.enabled = true;
        if (animator == null) animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.SetTrigger("Appear");
        }
        else
        {
            Debug.LogError("PortalController: Animator is null!");
        }
    }

    // Gọi từ Animation Event khi animation "xuất hiện" kết thúc
    public void OnAppearAnimationEnd()
    {
        if (!hasSpawnedPlayer)
        {
            // Di chuyển player sẵn trong scene đến vị trí portal và hiện lại
            playerObject.transform.position = spawnPoint.position;

            SpriteRenderer playerSprite = playerObject.GetComponent<SpriteRenderer>();
            playerSprite.enabled = true;
            
            hasSpawnedPlayer = true;
        }
        animator.SetTrigger("Disappear");
    }

    // Gọi từ Animation Event khi animation "biến mất" kết thúc
    public void OnDisappearAnimationEnd()
    {
        gameObject.SetActive(false); // Hoặc Destroy(gameObject);
    }
}