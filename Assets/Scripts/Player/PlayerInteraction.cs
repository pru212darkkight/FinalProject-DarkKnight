using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private NPCInteraction currentNPC;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            currentNPC = other.GetComponent<NPCInteraction>();
            if (currentNPC != null)
            {
                currentNPC.ShowInteractionHint();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            if (currentNPC != null)
            {
                currentNPC.CloseAll(); // Tắt shop + interaction UI
                currentNPC = null;
            }
        }
    }

    private void Update()
    {
        if (currentNPC != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            currentNPC.OpenShop();
        }

        if (currentNPC != null && Keyboard.current.xKey.wasPressedThisFrame)
        {
            currentNPC.CloseAll();
        }
    }
}
