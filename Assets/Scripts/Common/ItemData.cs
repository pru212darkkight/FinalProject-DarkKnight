using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemId;         // ID duy nhất cho item
    public string itemName;       // Tên hiển thị
    public Sprite icon;           // Ảnh đại diện
    public ItemType itemType;     // Loại item (vũ khí, giáp, nhẫn, ...)
    public int healthBonus;
    public int staminaBonus;
    public int manaBonus;
    public int strengthBonus;
    public int speedBonus;
    public int armorBonus;
    public int magicResistBonus;
    public int healthRegenBonus;
    public int staminaRegenBonus;
    public int manaRegenBonus;
    public int moveSpeedBonus;
    public int jumpBonus;
    // Thêm các chỉ số khác nếu cần
}

public enum ItemType
{
    Weapon,
    Armor,
    Accessory,
    Consumable,
    // ...
} 