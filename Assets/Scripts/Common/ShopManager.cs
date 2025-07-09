using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Setup")]
    public Transform itemListContainer;       // Container chứa các icon item
    public GameObject itemUIPrefab;           // Prefab chỉ có Icon (Image + Button)

    [Header("Data")]
    public Inventory inventory;
    public PlayerMoney playerMoney;
    public int defaultPrice = 100;

    [Header("UI Detail (bên trái)")]
    public GameObject detailPanel;
    public Image detailIcon;
    public TextMeshProUGUI detailName;
    public TextMeshProUGUI detailStats;
    public Button buyButton;

    // Item đang chọn
    private ItemData selectedItem;
    private int selectedItemPrice;

    void Start()
    {
        ItemData[] allItems = Resources.LoadAll<ItemData>("Items");

        Debug.Log("🟡 Tổng số item load được: " + allItems.Length);

        foreach (var item in allItems)
        {
            Debug.Log("🟢 Loaded: " + item.itemName);
            CreateShopIcon(item, defaultPrice);
        }

        detailPanel.SetActive(false);
    }

    void CreateShopIcon(ItemData itemData, int price)
    {
        GameObject ui = Instantiate(itemUIPrefab, itemListContainer);

        Transform iconObj = ui.transform.Find("Icon");

        if (iconObj == null || iconObj.GetComponent<Button>() == null || iconObj.GetComponent<Image>() == null)
        {
            Debug.LogError($"❌ Prefab bị thiếu Icon hoặc Button/Image: {itemUIPrefab.name}");
            return;
        }

        iconObj.GetComponent<Image>().sprite = itemData.icon;

        iconObj.GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log($"🖱️ Clicked: {itemData.itemName}"); // ✅ Log test click
            ShowItemDetail(itemData, price);
        });
    }

    void ShowItemDetail(ItemData item, int price)
    {
        detailPanel.SetActive(true);
        detailPanel.transform.SetAsLastSibling();

        // Gán selected
        selectedItem = item;
        selectedItemPrice = price;

        // Gán thông tin chi tiết
        detailIcon.sprite = item.icon;
        detailName.text = item.itemName;

        string stats = "";
        if (item.healthBonus != 0) stats += $"❤️ Health: +{item.healthBonus}\n";
        if (item.armorBonus != 0) stats += $"🛡️ Armor: +{item.armorBonus}\n";
        if (item.strengthBonus != 0) stats += $"💪 Strength: +{item.strengthBonus}\n";
        if (item.manaBonus != 0) stats += $"🔮 Mana: +{item.manaBonus}\n";
        if (item.moveSpeedBonus != 0) stats += $"🏃 Speed: +{item.moveSpeedBonus}\n";

        detailStats.text = stats;

        // Gán sự kiện cho nút mua
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(BuySelectedItem);
    }
    void BuySelectedItem()
    {
        if (selectedItem == null) return;

        if (inventory.HasItem(selectedItem.itemId))
        {
            Debug.Log("⚠ Đã sở hữu item này.");
            return;
        }

        if (playerMoney.SpendCoins(selectedItemPrice))
        {
            inventory.AddItem(selectedItem);
            Debug.Log($"✅ Đã mua: {selectedItem.itemName}");
        }
        else
        {
            Debug.LogWarning("❌ Không đủ tiền.");
        }
    }


}
