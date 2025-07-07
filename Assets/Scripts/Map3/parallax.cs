using UnityEngine;

public class parallax : MonoBehaviour
{
    [Header("References")]
    public Transform player; // Player transform

    [Header("Parallax Settings")]
    [Range(0f, 0.5f)]
    public float speed = 0.2f;

    [Range(0.01f, 0.1f)]
    public float movementThreshold = 0.01f;

    private Material mat;
    private float distance;
    private PlayerController1 playerController;
    private Vector3 playerLastPos;

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
                playerController = playerObj.GetComponent<PlayerController1>();
            }
        }
        else
        {
            playerController = player.GetComponent<PlayerController1>();
        }

        if (player != null)
        {
            playerLastPos = player.position;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Detect player movement using input
        bool isPlayerMoving = false;
        float playerInputX = 0f;

        if (playerController != null)
        {
            // Sử dụng input từ PlayerController
            playerInputX = playerController.MoveInput.x;
            isPlayerMoving = Mathf.Abs(playerInputX) > movementThreshold;
        }
        else
        {
            // Fallback: detect movement bằng position
            Vector3 playerMovement = player.position - playerLastPos;
            isPlayerMoving = Mathf.Abs(playerMovement.x) > movementThreshold * Time.deltaTime;
            playerInputX = Mathf.Sign(playerMovement.x);
        }

        // Chỉ update parallax khi player đang di chuyển
        if (isPlayerMoving)
        {
            // Background di chuyển NGƯỢC hướng với player
            // Nếu player di chuyển sang phải (+), background di chuyển sang trái (distance giảm)
            // Nếu player di chuyển sang trái (-), background di chuyển sang phải (distance tăng)
            float oldDistance = distance;
            distance -= playerInputX * Time.deltaTime * speed;
            mat.SetTextureOffset("_MainTex", Vector2.right * distance);

            // Debug log để kiểm tra
            Debug.Log($"Player Input: {playerInputX:F2}, Distance: {oldDistance:F2} -> {distance:F2}, Offset: {distance:F2}");
        }

        // Update last position
        playerLastPos = player.position;
    }
}
