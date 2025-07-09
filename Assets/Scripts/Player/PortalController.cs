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

        // Ẩn portal lúc đầu
        spriteRenderer.enabled = false;
        if (portalCollider != null) portalCollider.enabled = false;

        

        SpriteRenderer playerSprite = playerObject.GetComponent<SpriteRenderer>();
        playerSprite.enabled = false;
       
    }

    void Start()
    {
        // Sau delay, hiện portal và play animation xuất hiện
        Invoke(nameof(ShowPortal), delayBeforeAppear);
    }

    void ShowPortal()
    {
        spriteRenderer.enabled = true;
        if (portalCollider != null) portalCollider.enabled = true;
        if (animator == null) animator = GetComponent<Animator>();
        animator.SetTrigger("Appear");
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