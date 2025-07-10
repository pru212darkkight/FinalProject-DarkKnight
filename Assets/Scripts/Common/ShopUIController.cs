using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUIController : MonoBehaviour
{
    public Transform itemListContainer;         // Grid Content của ScrollView
    public GameObject itemUIPrefab;             // Prefab nhỏ
    public Inventory inventory;
    public PlayerMoney playerMoney;

    [Header("Chi tiết bên trái")]
    public Image detailIcon;
    public TextMeshProUGUI detailName;
    public TextMeshProUGUI detailStats;
    public Button buyButton;

    private ItemData selectedItem;
    public int defaultPrice = 100;

    void Start()
    {
        var allItems = Resources.LoadAll<ItemData>("Items");
        foreach (var item in allItems)
        {
            GameObject ui = Instantiate(itemUIPrefab, itemListContainer);
            var itemUI = ui.GetComponent<ShopItemUI>();
            itemUI.Setup(item, this);
        }

        buyButton.onClick.AddListener(BuySelectedItem);
    }

    public void ShowItemDetail(ItemData item)
    {
        selectedItem = item;
        detailIcon.sprite = item.icon;
        detailName.text = item.itemName;
        detailStats.text = GetStatText(item);
    }

    string GetStatText(ItemData item)
    {
        string result = "";
        void AddLine(string name, float value)
        {
            if (value != 0)
                result += $"- {name}: +{value}\n";
        }

        AddLine("Health", item.healthBonus);
        AddLine("Stamina", item.staminaBonus);
        AddLine("Mana", item.manaBonus);
        AddLine("Strength", item.strengthBonus);
        AddLine("Armor", item.armorBonus);
        AddLine("Magic Resist", item.magicResistBonus);
        AddLine("Health Regen", item.healthRegenBonus);
        AddLine("Stamina Regen", item.staminaRegenBonus);
        AddLine("Mana Regen", item.manaRegenBonus);
        AddLine("Move Speed", item.moveSpeedBonus);
        AddLine("Jump", item.jumpBonus);
        return result;
    }

    void BuySelectedItem()
    {
        if (selectedItem == null) return;
        if (inventory.HasItem(selectedItem.itemId))
        {
            Debug.Log("Đã sở hữu item này.");
            return;
        }

        if (playerMoney.SpendCoins(defaultPrice))
        {
            inventory.AddItem(selectedItem);
            Debug.Log("Đã mua: " + selectedItem.itemName);
        }
    }
}
