using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InteractWithAdviceNPC : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject interactionUI;           // UI chữ E
    public TextMeshProUGUI interactionText;    // Text bên trong
    public AdvicePanelController advicePanelController;

    [Header("Audio")]
    public AudioClip greetingClip;
    private AudioSource audioSource;

    private bool playerInRange = false;

    void Start()
    {
        if (interactionUI != null)
            interactionUI.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            advicePanelController.ToggleAdvicePanel();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactionUI != null)
            {
                interactionUI.SetActive(true);
                interactionText.text = "E";
            }

            if (greetingClip != null)
                audioSource.PlayOneShot(greetingClip);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactionUI != null)
                interactionUI.SetActive(false);

            advicePanelController.HideAdvice();
        }
    }
}
