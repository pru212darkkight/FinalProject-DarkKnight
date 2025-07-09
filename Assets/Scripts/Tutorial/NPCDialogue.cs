using UnityEngine;
using UnityEngine.UI;

public class NPCDialogue : MonoBehaviour
{
    public string[] dialogueLines; // Các câu thoại
    private int currentLine = 0;

    public GameObject dialogueUI; // UI panel chứa thoại
    public Text dialogueText;     // Text hiển thị nội dung

    public void StartDialogue()
    {
        currentLine = 0;
        dialogueUI.SetActive(true);
        ShowNextLine();
    }

    void Update()
    {
        if (dialogueUI.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextLine();
        }
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
        }
    }
}
