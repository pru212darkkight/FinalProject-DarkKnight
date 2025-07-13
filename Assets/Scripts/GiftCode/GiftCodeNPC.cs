using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.EventSystems;

public class GiftCodeNPC : MonoBehaviour
{
    public GiftCodeManager giftCodeManager; // Kéo vào Inspector
    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Nếu panel ĐANG MỞ và KHÔNG focus input field → tắt panel
            if (giftCodeManager.panelGiftCode.activeSelf)
            {
                if (!IsInputFieldFocused())
                {
                    giftCodeManager.ClosePanel();
                }
                // Nếu đang focus input field thì không làm gì, tránh bug khi nhập E trong input
            }
            else
            {
                // Panel đang tắt → mở panel
                giftCodeManager.ShowPanel();
            }
        }
    }

    private bool IsInputFieldFocused()
    {
        // Nếu đang có input field hoặc button được focus
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
        {
            var selected = EventSystem.current.currentSelectedGameObject;
            return selected.GetComponent<TMP_InputField>() != null
                || selected.GetComponent<UnityEngine.UI.InputField>() != null;
        }
        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
