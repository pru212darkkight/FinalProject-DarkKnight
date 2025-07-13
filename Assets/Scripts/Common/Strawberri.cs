using UnityEngine;
using TMPro;

public class Strawberri : MonoBehaviour
{
    [TextArea]
    public string dialogueLine = "Steal a look, but buy with gold!";
    public AudioClip voiceClip;
    public GameObject dialogueUI;             
    public TextMeshProUGUI dialogueText;     

    private AudioSource audioSource;
    private bool hasSpoken = false;

    void Start()
    {
        if (dialogueUI != null)
            dialogueUI.SetActive(false); 

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (dialogueUI != null && dialogueText != null)
            {
                dialogueText.text = dialogueLine;
                dialogueUI.SetActive(true);
            }

            if (!hasSpoken && voiceClip != null)
            {
                audioSource.PlayOneShot(voiceClip);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (dialogueUI != null)
                dialogueUI.SetActive(false);
        }
    }
}
