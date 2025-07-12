using UnityEngine;
using TMPro;

public class NPCInteraction : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject shopUI;
    public GameObject interactionUI;
    public TextMeshProUGUI interactionText;

    [Header("Audio - SFX")]
    public AudioClip shopOpenClip;
    public AudioClip shopCloseClip;

    [Header("Audio - Voice Lines")]
    public AudioClip greetingClip; // Giọng nói khi hiện E

    private bool isShopOpen = false;
    private AudioSource audioSource;

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

    public void ShowInteractionHint()
    {
        interactionUI.SetActive(true);
        interactionText.text = "E";

        if (greetingClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(greetingClip);
        }
    }

    public void HideInteractionHint()
    {
        interactionUI.SetActive(false);
    }

    public void OpenShop()
    {
        if (isShopOpen) return;

        Debug.Log("OpenShop() called");
        shopUI.SetActive(true);
        isShopOpen = true;

        if (shopOpenClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(shopOpenClip);
        }
    }

    public void CloseAll()
    {
        if (!isShopOpen) return;

        Debug.Log("CloseAll() called");
        shopUI.SetActive(false);
        HideInteractionHint();
        isShopOpen = false;

        if (shopCloseClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(shopCloseClip);
        }
    }

    public bool IsShopOpen()
    {
        return isShopOpen;
    }
}
