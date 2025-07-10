using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemId;         // ID duy nhất cho item
    public string itemName;       // Tên hiển thị
    public Sprite icon;           // Ảnh đại diện
    public ItemType itemType;     // Loại item (vũ khí, giáp, nhẫn, ...)
    public float healthBonus;
    public float staminaBonus;
    public float manaBonus;
    public float strengthBonus;
    public float armorBonus;
    public float magicResistBonus;
    public float healthRegenBonus;
    public float staminaRegenBonus;
    public float manaRegenBonus;
    public float moveSpeedBonus;
    public float jumpBonus;
    public int price;
    // Thêm các chỉ số khác nếu cần
}

public enum ItemType
{
    Weapon,
    Armor,
    Pants,
    Helmet,
    Boots,
    Ring,
    

    // ...
} 