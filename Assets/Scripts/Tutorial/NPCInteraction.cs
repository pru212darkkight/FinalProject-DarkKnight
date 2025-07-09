using UnityEngine;
using TMPro;

public class NPCInteraction : MonoBehaviour
{
    public GameObject shopUI;

    public GameObject interactionUI;
    public TextMeshProUGUI interactionText;

    private bool isShopOpen = false;

    public void ShowInteractionHint()
    {
        interactionUI.SetActive(true);
        interactionText.text = "F";
    }

    public void HideInteractionHint()
    {
        interactionUI.SetActive(false);
    }

    void Start()
    {
        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    public void OpenShop()
    {
        Debug.Log("Đã gọi OpenShop()");
        shopUI.SetActive(true);
        isShopOpen = true;
    }


    public void CloseAll()
    {
        shopUI.SetActive(false);
        HideInteractionHint();
        isShopOpen = false;
    }
}
