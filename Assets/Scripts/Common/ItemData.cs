using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemId;         // ID duy nhất cho item
    public string itemName;       // Tên hiển thị
    public Sprite icon;           // Ảnh đại diện
    public ItemType itemType;     // Loại item (vũ khí, giáp, nhẫn, ...)
    public int attackBonus;
    public int defenseBonus;
    public int healthBonus;
    public int manaBonus;
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