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

        if (allItems.Length == 0)
        {
            Debug.LogError("❌ Không có item nào trong thư mục Resources/Items!");
            return;
        }

        // Tạo icon cho tất cả item
        for (int i = 0; i < allItems.Length; i++)
        {
            CreateShopIcon(allItems[i], defaultPrice);
        }

        // ✅ Hiển thị chi tiết món đầu tiên sau khi đã tạo UI
        ShowItemDetail(allItems[0], defaultPrice);
    }

    void CreateShopIcon(ItemData itemData, int price)
    {
        GameObject ui = Instantiate(itemUIPrefab, itemListContainer);
        Transform iconObj = ui.transform.Find("Icon");

        if (iconObj == null || iconObj.GetComponent<Button>() == null || iconObj.GetComponent<Image>() == null)
        {
            Debug.LogError($"❌ Prefab thiếu thành phần Icon/Image/Button: {itemUIPrefab.name}");
            return;
        }

        iconObj.GetComponent<Image>().sprite = itemData.icon;
        iconObj.GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log($"🖱️ Clicked: {itemData.itemName}");
            ShowItemDetail(itemData, price);
        });
    }

    void ShowItemDetail(ItemData item, int price)
    {
        if (!detailPanel.activeSelf)
            detailPanel.SetActive(true);

        detailPanel.transform.SetAsLastSibling();  // Đảm bảo không bị che

        selectedItem = item;
        selectedItemPrice = price;

        if (item.icon != null)
        {
            detailIcon.sprite = item.icon;
        }
        else
        {
            Debug.LogWarning("❗ Item không có icon: " + item.itemName);
        }

        detailName.text = item.itemName;

        string stats = "";
        if (item.healthBonus != 0) stats += $"Health: +{item.healthBonus}\n";
        if (item.armorBonus != 0) stats += $"Armor: +{item.armorBonus}\n";
        if (item.strengthBonus != 0) stats += $"Strength: +{item.strengthBonus}\n";
        if (item.manaBonus != 0) stats += $"Mana: +{item.manaBonus}\n";
        if (item.moveSpeedBonus != 0) stats += $"Speed: +{item.moveSpeedBonus}\n";

        detailStats.text = string.IsNullOrEmpty(stats) ? "Không có chỉ số" : stats;

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
