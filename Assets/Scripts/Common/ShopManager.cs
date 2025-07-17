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
    public Image coinIcon; // 🪙 icon đồng xu

    [Header("UI Coins")]
    public TextMeshProUGUI moneyText;

    private ItemData selectedItem;
    private int selectedItemPrice;

    void Start()
    {
        // Delay để đảm bảo PlayerMoney đã load xong (đặc biệt quan trọng cho WebGL)
        StartCoroutine(InitializeShopWithDelay());
    }

    System.Collections.IEnumerator InitializeShopWithDelay()
    {
        // Chờ 1 frame để các script khác khởi tạo
        yield return null;

        // Retry logic để đảm bảo PlayerMoney đã sẵn sàng
        int retryCount = 0;
        while (playerMoney == null && retryCount < 10)
        {
            playerMoney = FindObjectOfType<PlayerMoney>();
            retryCount++;
            yield return new WaitForSeconds(0.1f);
        }

        // Đảm bảo PlayerMoney đã load data
        if (playerMoney != null)
        {
            // Force load money trước khi update UI
            playerMoney.LoadMoney();
            yield return new WaitForSeconds(0.1f); // Thêm delay nhỏ cho WebGL
        }

        LoadShopItems();
        UpdateMoneyUI();

        Debug.Log($"🛒 ShopManager initialized. PlayerMoney coins: {(playerMoney != null ? playerMoney.coins : -1)}");
    }

    void LoadShopItems()
    {
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

            if (coinIcon != null)
                coinIcon.gameObject.SetActive(false); // ẩn đồng xu
        }
        else
        {
            buyButtonText.text = $"{price}";
            buyButton.interactable = true;

            if (coinIcon != null)
                coinIcon.gameObject.SetActive(true); // hiện đồng xu

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
            if (AudioManager.Instance != null && AudioManager.Instance.buyItem != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.buyItem);
            }
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

    public void UpdateMoneyUI()
    {
        // Tìm PlayerMoney nếu chưa có reference
        if (playerMoney == null)
        {
            playerMoney = FindObjectOfType<PlayerMoney>();
        }

        if (playerMoney != null)
        {
            // Force reload money từ PlayerPrefs (quan trọng cho WebGL)
            playerMoney.LoadMoney();

            Debug.Log($"💰 UpdateMoneyUI called. Coins: {playerMoney.coins}");

            if (moneyText != null)
            {
                moneyText.text = $"{playerMoney.coins} ";
            }
        }
        else
        {
            Debug.LogError("🚨 ShopManager: PlayerMoney not found!");
            if (moneyText != null)
            {
                moneyText.text = "0 ";
            }
        }
    }

    public void OpenShopUI()
    {
        UpdateMoneyUI();
    }

    // Method để force refresh toàn bộ shop (gọi khi cần thiết)
    [ContextMenu("Force Refresh Shop")]
    public void ForceRefreshShop()
    {
        StartCoroutine(ForceRefreshShopCoroutine());
    }

    System.Collections.IEnumerator ForceRefreshShopCoroutine()
    {
        // Tìm lại PlayerMoney
        playerMoney = FindObjectOfType<PlayerMoney>();

        if (playerMoney != null)
        {
            // Force reload money
            playerMoney.LoadMoney();
            yield return new WaitForSeconds(0.1f);
        }

        // Reload shop items và update UI
        LoadShopItems();
        UpdateMoneyUI();

        Debug.Log("🔄 Shop force refreshed!");
    }

    // Method để gọi khi player nhận tiền (từ external scripts)
    public void OnPlayerMoneyChanged()
    {
        UpdateMoneyUI();
    }
}
