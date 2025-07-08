using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public GameObject interactionUI; // UI hiện chữ "Nhấn E để nói chuyện"
    private bool isNearNPC = false;
    private NPCDialogue currentNPC;

    void Update()
    {
        if (isNearNPC && Input.GetKeyDown(KeyCode.E))
        {
            currentNPC.StartDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            isNearNPC = true;
            currentNPC = other.GetComponent<NPCDialogue>();
            interactionUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            isNearNPC = false;
            currentNPC = null;
            interactionUI.SetActive(false);
        }
    }
}
