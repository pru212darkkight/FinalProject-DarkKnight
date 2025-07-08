using UnityEngine;
using UnityEngine.InputSystem;

public class TreasureChest : MonoBehaviour
{
    public Animator animator; // Gán animator của rương
    public string openAnimationName = "crazy"; // Tên animation mở rương
    [Header("Interaction")]
    public KeyCode openKey = KeyCode.E; // Phím mở rương
    public bool canOpenMultipleTimes = false;

    private bool playerInRange = false;
    private bool isOpened = false;

    // Input System
    private InputAction interactAction;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        // Lấy action từ PlayerController1
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            var playerController = playerObj.GetComponent<PlayerController1>();
            if (playerController != null)
            {
                interactAction = playerController.interactAction;
            }
        }
    }

    void Update()
    {
        if (playerInRange && (canOpenMultipleTimes || !isOpened) && interactAction != null && interactAction.WasPressedThisFrame())
        {
            Debug.Log("E pressed, opening chest");
            OpenChest();
        }
    }

    void OpenChest()
    {
        isOpened = true;
        animator.Play(openAnimationName);
        // TODO: Thêm hiệu ứng, phần thưởng, âm thanh ở đây nếu muốn
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // TODO: Hiện UI "Nhấn E để mở rương" nếu muốn
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            // TODO: Ẩn UI "Nhấn E để mở rương" nếu có
        }
    }
}