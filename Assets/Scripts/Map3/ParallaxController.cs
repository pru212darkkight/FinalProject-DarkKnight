using System.Linq;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    [Header("References")]
    public Transform player; // Player transform để theo dõi movement
    Transform cam; // Main camera

    [Header("Parallax Settings")]
    [Range(0.01f, 0.1f)]
    public float parallaxSpeed = 0.02f;

    [Header("Movement Detection")]
    [Range(0.01f, 0.1f)]
    public float movementThreshold = 0.01f; // Ngưỡng để detect player đang di chuyển

    // Private variables
    Vector3 playerLastPos;
    float totalDistance; // Tổng khoảng cách player đã di chuyển

    GameObject[] backgrounds;
    Material[] mat;
    float[] backSpeed;
    float farthestBack;

    // Player movement detection
    private PlayerController1 playerController;
    private Vector2 lastPlayerInput;

    void Start()
    {
        cam = Camera.main.transform;

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

        int backCount = transform.childCount;
        mat = new Material[backCount];
        backSpeed = new float[backCount];
        backgrounds = new GameObject[backCount];

        for (int i = 0; i < backCount; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            mat[i] = backgrounds[i].GetComponent<Renderer>().material;
        }
        BackSpeedCalculate(backCount);
    }

    void BackSpeedCalculate(int backCount)
    {
        for(int i = 0; i < backCount; i++) //find the farthest background
        {
            if ((backgrounds[i].transform.position.z - cam.position.z) > farthestBack)
            {
                farthestBack = backgrounds[i].transform.position.z - cam.position.z;
            }
        }

        for(int i = 0; i < backCount; i++) //set the speed of backgrounds
        {
            backSpeed[i] = 1 - (backgrounds[i].transform.position.z - cam.position.z) / farthestBack;
        }
    }

    private void LateUpdate()
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
            // Background di chuyển NGƯỢC hướng với player input
            // Nếu player di chuyển sang phải (+), background di chuyển sang trái (-)
            float parallaxMovement = -playerInputX * Time.deltaTime;
            totalDistance += parallaxMovement;

            // Update background positions
            for (int i = 0; i < backgrounds.Length; i++)
            {
                float speed = backSpeed[i] * parallaxSpeed;
                mat[i].SetTextureOffset("_MainTex", new Vector2(totalDistance, 0) * speed);
            }
        }

        // Update camera follow (keep existing behavior)
        transform.position = new Vector3(cam.position.x, transform.position.y, 0);

        // Update last position
        playerLastPos = player.position;
    }
}
