using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Setup")]
    public Transform itemListContainer;
    public GameObject itemUIPrefab;

    [Header("Data")]
    public Inventory inventory;
    public PlayerMoney playerMoney;

    [Header("UI Detail Panel")]
    public GameObject detailPanel;
    public Image detailIcon;
    public TextMeshProUGUI detailName;
    public TextMeshProUGUI detailStats;
    public Button buyButton;
    public TextMeshProUGUI buyButtonText;

    [Header("UI Coins")]
    public TextMeshProUGUI moneyText;

    private ItemData selectedItem;
    private int selectedItemPrice;

    void Start()
    {
        LoadShopItems();
        UpdateMoneyUI();
    }

    void LoadShopItems()
    {
        // Xóa các icon cũ
        foreach (Transform child in itemListContainer)
            Destroy(child.gameObject);

        ItemData[] allItems = Resources.LoadAll<ItemData>("Items");

        Debug.Log($"🟡 Tổng số item load được: {allItems.Length}");

        if (allItems.Length == 0) return;

        foreach (var item in allItems)
        {
            CreateShopIcon(item, item.price);
        }

        ShowItemDetail(allItems[0], allItems[0].price);
    }

    void CreateShopIcon(ItemData itemData, int price)
    {
        GameObject ui = Instantiate(itemUIPrefab, itemListContainer);
        Transform iconObj = ui.transform.Find("Icon");

        if (iconObj == null)
        {
            Debug.LogWarning("❗ Prefab thiếu thành phần Icon.");
            return;
        }

        Image iconImage = iconObj.GetComponent<Image>();
        Button iconButton = iconObj.GetComponent<Button>();

        if (iconImage == null || iconButton == null)
        {
            Debug.LogWarning("❗ Prefab thiếu Image hoặc Button.");
            return;
        }

        iconImage.sprite = itemData.icon;
        iconImage.color = inventory.HasItem(itemData.itemId) ? Color.gray : Color.white;

        iconButton.onClick.AddListener(() =>
        {
            ShowItemDetail(itemData, price);
        });
    }

    void ShowItemDetail(ItemData item, int price)
    {
        if (!detailPanel.activeSelf)
            detailPanel.SetActive(true);

        detailPanel.transform.SetAsLastSibling();

        selectedItem = item;
        selectedItemPrice = price;

        detailIcon.sprite = item.icon != null ? item.icon : null;
        detailName.text = item.itemName;

        // Tạo chuỗi stats
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

        if (inventory.HasItem(item.itemId))
        {
            buyButtonText.text = "Owned";
            buyButton.interactable = false;
        }
        else
        {
            buyButtonText.text = $"{price} ";
            buyButton.interactable = true;

            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(BuySelectedItem);
        }
    }

    void BuySelectedItem()
    {
        if (selectedItem == null) return;

        if (inventory.HasItem(selectedItem.itemId))
        {
            Debug.Log("Đã sở hữu item này.");
            return;
        }

        if (playerMoney.SpendCoins(selectedItemPrice))
        {
            inventory.AddItem(selectedItem);
            Debug.Log($"✅ Đã mua: {selectedItem.itemName}");

            LoadShopItems();
            ShowItemDetail(selectedItem, selectedItem.price);
            UpdateMoneyUI();
        }
        else
        {
            Debug.LogWarning("❌ Không đủ tiền!");
        }
    }

    void UpdateMoneyUI()
    {
        if (moneyText != null && playerMoney != null)
            moneyText.text = $"{playerMoney.coins} ";
    }
}
