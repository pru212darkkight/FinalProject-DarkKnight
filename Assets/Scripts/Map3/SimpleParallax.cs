using UnityEngine;

public class SimpleParallax : MonoBehaviour
{
    [Header("References")]
    public Transform player; // Player transform

    [Header("Parallax Settings")]
    [Range(0f, 1f)]
    public float parallaxSpeed = 0.2f;

    [Header("Debug")]
    public bool enableDebug = true;

    private Material mat;
    private PlayerController1 playerController;
    private Vector2 currentOffset;

    void Start()
    {
        mat = GetComponent<Renderer>().material;

        // Tự động tìm player nếu chưa assign
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (player != null)
        {
            playerController = player.GetComponent<PlayerController1>();
        }

        // Lấy offset hiện tại
        currentOffset = mat.GetTextureOffset("_MainTex");

        if (enableDebug)
        {
            Debug.Log($"SimpleParallax initialized. Current offset: {currentOffset}");
        }
    }

    void Update()
    {
        if (player == null || playerController == null) return;

        // Lấy input từ player
        Vector2 playerInput = playerController.MoveInput;

        // Chỉ update khi player đang di chuyển
        if (Mathf.Abs(playerInput.x) > 0.01f)
        {
            // Background di chuyển NGƯỢC hướng với player
            // Player sang phải (+) -> Background sang trái (offset.x tăng)
            // Player sang trái (-) -> Background sang phải (offset.x giảm)
            currentOffset.x += playerInput.x * parallaxSpeed * Time.deltaTime;

            // Apply offset
            mat.SetTextureOffset("_MainTex", currentOffset);

            if (enableDebug)
            {
                Debug.Log($"Player Input: {playerInput.x:F2}, New Offset: {currentOffset.x:F2}");
            }
        }
    }

    // Reset offset về 0
    [ContextMenu("Reset Offset")]
    public void ResetOffset()
    {
        currentOffset = Vector2.zero;
        mat.SetTextureOffset("_MainTex", currentOffset);
        Debug.Log("Parallax offset reset to zero");
    }
}