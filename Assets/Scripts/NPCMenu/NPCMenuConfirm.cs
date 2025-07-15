using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class NPCMenuConfirm : MonoBehaviour
{
    public GameObject confirmPanel;
    public Button yesButton;
    public Button noButton;

    private bool playerInRange = false;

    void Start()
    {
        confirmPanel.SetActive(false);
        yesButton.onClick.AddListener(OnYesClick);
        noButton.onClick.AddListener(OnNoClick);
    }

    void Update()
    {
        // Dùng Input System mới: Keyboard.current.eKey.wasPressedThisFrame
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Nếu panel đang hiện thì không làm gì (hoặc có thể cho ẩn panel ở đây tuỳ bạn)
            if (!confirmPanel.activeSelf)
            {
                confirmPanel.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            confirmPanel.SetActive(false);
        }
    }

    void OnYesClick()
    {
        SceneManager.LoadScene("Main Menu");
    }

    void OnNoClick()
    {
        confirmPanel.SetActive(false);
    }
}
