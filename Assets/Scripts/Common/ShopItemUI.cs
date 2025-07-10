using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public Image icon;
    private ItemData currentData;
    private ShopUIController shopController;

    public void Setup(ItemData data, ShopUIController controller)
    {
        currentData = data;
        shopController = controller;
        icon.sprite = data.icon;

        GetComponent<Button>().onClick.AddListener(() =>
        {
            shopController.ShowItemDetail(currentData);
        });
    }
}
