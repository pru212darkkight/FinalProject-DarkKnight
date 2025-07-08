using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<ItemData> ownedItems = new List<ItemData>();
    public List<ItemData> equippedItems = new List<ItemData>();

    // Thêm item vào inventory
    public void AddItem(ItemData item)
    {
        if (!ownedItems.Contains(item))
            ownedItems.Add(item);
    }

    // Trang bị item
    public void EquipItem(ItemData item)
    {
        if (ownedItems.Contains(item) && !equippedItems.Contains(item))
            equippedItems.Add(item);
    }

    // Bỏ trang bị
    public void UnequipItem(ItemData item)
    {
        if (equippedItems.Contains(item))
            equippedItems.Remove(item);
    }

    // Kiểm tra sở hữu
    public bool HasItem(string itemId)
    {
        return ownedItems.Exists(i => i.itemId == itemId);
    }

    // Lưu inventory (PlayerPrefs, chỉ lưu id)
    public void SaveInventory()
    {
        var ids = ownedItems.Select(i => i.itemId).ToArray();
        PlayerPrefs.SetString("OwnedItems", string.Join(",", ids));
        var eqIds = equippedItems.Select(i => i.itemId).ToArray();
        PlayerPrefs.SetString("EquippedItems", string.Join(",", eqIds));
        PlayerPrefs.Save();
    }

    // Load inventory (cần truyền vào danh sách tất cả item có trong game)
    public void LoadInventory(List<ItemData> allItems)
    {
        string data = PlayerPrefs.GetString("OwnedItems", "");
        ownedItems.Clear();
        if (!string.IsNullOrEmpty(data))
        {
            var ids = data.Split(',');
            ownedItems = allItems.Where(i => ids.Contains(i.itemId)).ToList();
        }
        string eqData = PlayerPrefs.GetString("EquippedItems", "");
        equippedItems.Clear();
        if (!string.IsNullOrEmpty(eqData))
        {
            var eqIds = eqData.Split(',');
            equippedItems = allItems.Where(i => eqIds.Contains(i.itemId)).ToList();
        }
    }
} 