using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class ItemInfoPanel : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text statsText;
    public Button equipButton;
    public Button removeButton;

    private ItemData currentItem;
    private InventoryUI inventoryUI;

    public void ShowInfo(ItemData item, bool isEquipped, InventoryUI ui)
    {
        currentItem = item;
        inventoryUI = ui;

        icon.sprite = item.icon;
        nameText.text = item.itemName;

        var sb = new StringBuilder();
        sb.AppendLine($"Type: {item.itemType}");

        if (item.healthBonus != 0) sb.AppendLine($"Health: {item.healthBonus}");
        if (item.staminaBonus != 0) sb.AppendLine($"Stamina: {item.staminaBonus}");
        if (item.manaBonus != 0) sb.AppendLine($"Mana: {item.manaBonus}");
        if (item.strengthBonus != 0) sb.AppendLine($"Strength: {item.strengthBonus}");
        if (item.armorBonus != 0) sb.AppendLine($"Armor: {item.armorBonus}");
        if (item.magicResistBonus != 0) sb.AppendLine($"Magic Resist: {item.magicResistBonus}");
        if (item.healthRegenBonus != 0) sb.AppendLine($"Health Regen: {item.healthRegenBonus}");
        if (item.staminaRegenBonus != 0) sb.AppendLine($"Stamina Regen: {item.staminaRegenBonus}");
        if (item.manaRegenBonus != 0) sb.AppendLine($"Mana Regen: {item.manaRegenBonus}");
        if (item.moveSpeedBonus != 0) sb.AppendLine($"Move Speed: {item.moveSpeedBonus}");
        if (item.jumpBonus != 0) sb.AppendLine($"Jump: {item.jumpBonus}");

        statsText.text = sb.ToString();

        equipButton.gameObject.SetActive(!isEquipped);
        removeButton.gameObject.SetActive(isEquipped);

        equipButton.onClick.RemoveAllListeners();
        equipButton.onClick.AddListener(() => {
            if (inventoryUI != null && currentItem != null)
                inventoryUI.OnEquipItem(currentItem);
        });

        removeButton.onClick.RemoveAllListeners();
        removeButton.onClick.AddListener(() => {
            if (inventoryUI != null && currentItem != null)
                inventoryUI.OnRemoveItem(currentItem);
        });

        gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);
}
