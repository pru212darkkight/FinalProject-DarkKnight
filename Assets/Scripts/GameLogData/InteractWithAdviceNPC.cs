using UnityEngine;
using UnityEngine.InputSystem;

public class InteractWithAdviceNPC : MonoBehaviour
{
    public AdvicePanelController advicePanelController; // Kéo script quản lý panel vào đây
    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            advicePanelController.ToggleAdvicePanel();
        }
    }


    // Detect player vào vùng trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // (Tuỳ chọn) Hiện hint "Bấm E để nhận lời khuyên"
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            advicePanelController.HideAdvice();
            // (Tuỳ chọn) Ẩn hint
        }
    }
}
