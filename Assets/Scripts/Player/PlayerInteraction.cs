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
                
                if (currentNPC.IsShopOpen())
                {
                    currentNPC.CloseAll();
                }

                currentNPC = null;
            }
        }
    }

    private void Update()
    {
        if (currentNPC != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (currentNPC.IsShopOpen())
            {
                currentNPC.CloseAll();
            }
            else
            {
                currentNPC.OpenShop();
            }
        }
    }
}
