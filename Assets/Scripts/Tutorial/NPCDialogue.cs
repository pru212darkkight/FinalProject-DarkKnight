using UnityEngine;
using UnityEngine.UI;

public class NPCInteraction : MonoBehaviour
{
    public string[] dialogueLines;
    public GameObject dialogueUI;
    public GameObject shopUI;
    public Text dialogueText;

    private int currentLine = 0;
    private bool playerInRange = false;
    private bool isTalking = false;
    private bool isShopOpen = false;

    void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.E) && !isTalking)
        {
            StartDialogue();
        }

        else if (Input.GetKeyDown(KeyCode.F) && !isShopOpen)
        {
            OpenShop();
        }

        else if (Input.GetKeyDown(KeyCode.X))
        {
            CloseAll();
        }

        else if (isTalking && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextLine();
        }
    }

    void StartDialogue()
    {
        isTalking = true;
        currentLine = 0;
        dialogueUI.SetActive(true);
        ShowNextLine();
    }

    void ShowNextLine()
    {
        if (currentLine < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentLine];
            currentLine++;
        }
        else
        {
            dialogueUI.SetActive(false);
            isTalking = false;
        }
    }

    void OpenShop()
    {
        shopUI.SetActive(true);
        isShopOpen = true;
    }

    void CloseAll()
    {
        dialogueUI.SetActive(false);
        shopUI.SetActive(false);
        isTalking = false;
        isShopOpen = false;
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
            CloseAll();
        }
    }
}
