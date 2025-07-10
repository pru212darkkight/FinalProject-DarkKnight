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
    public TextMeshProUGUI buyButtonText;

    // Item đang chọn
    private ItemData selectedItem;
    private int selectedItemPrice;

    void Start()
    {
        ItemData[] allItems = Resources.LoadAll<ItemData>("Items");
        Debug.Log("🟡 Tổng số item load được: " + allItems.Length);

        if (allItems.Length == 0)
        {
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
            return;
        }

        iconObj.GetComponent<Image>().sprite = itemData.icon;
        iconObj.GetComponent<Button>().onClick.AddListener(() =>
        {
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

        void AddStat(string label, float value)
        {
            if (value != 0)
                stats += $"{label}: +{value}\n";
        }

        AddStat("Health", item.healthBonus);
        AddStat("Stamina", item.staminaBonus);
        AddStat("Mana", item.manaBonus);
        AddStat("Strength", item.strengthBonus);
        AddStat("Armor", item.armorBonus);
        AddStat("Magic Resist", item.magicResistBonus);
        AddStat("Health Regen", item.healthRegenBonus);
        AddStat("Stamina Regen", item.staminaRegenBonus);
        AddStat("Mana Regen", item.manaRegenBonus);
        AddStat("Speed", item.moveSpeedBonus);
        AddStat("Jump", item.jumpBonus);


        detailStats.text = string.IsNullOrEmpty(stats) ? "Không có chỉ số" : stats;

        // 👉 Cập nhật text cho nút mua
        if (buyButtonText != null)
        {
            buyButtonText.text = $"{item.price} coins";
        }

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
