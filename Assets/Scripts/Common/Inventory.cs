using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<ItemData> ownedItems = new List<ItemData>();
    public List<ItemData> equippedItems = new List<ItemData>();
    public PlayerController1 player; // Gán qua Inspector hoặc tìm bằng code

    void Start()
    {
        // Tự động load tất cả item từ Resources/Items
        ItemData[] allItems = Resources.LoadAll<ItemData>("Items");
        LoadInventory(allItems.ToList(), true); // true: đầu scene
    }

    // Thêm item vào inventory
    public void AddItem(ItemData item)
    {
        if (!ownedItems.Contains(item))
        {
            ownedItems.Add(item);
            SaveInventory(); // Tự động lưu mỗi khi thêm item
        }
        else
        {
            Debug.LogWarning("Item đã tồn tại trong inventory!");
        }
    }

    // Trang bị item
    public void EquipItem(ItemData item)
    {
        if (ownedItems.Contains(item) && !equippedItems.Contains(item))
        {
            // Gỡ item cùng type trước khi trang bị mới
            List<ItemData> itemsToUnequip = equippedItems.Where(i => i.itemType == item.itemType).ToList();
            foreach (ItemData oldItem in itemsToUnequip)
            {
                equippedItems.Remove(oldItem);
                Debug.Log($"Đã gỡ item cũ: {oldItem.itemName}");
            }
            
            // Trang bị item mới
            equippedItems.Add(item);
            SaveInventory();
            if (player != null)
            {
                player.ApplyEquipmentStats(false); // Không hồi đầy máu khi equip giữa trận
                Debug.Log($"Item đã được trang bị: {item.itemName}");
            }
        }
        else
        {
            Debug.LogWarning("Item không được sở hữu hoặc đã trang bị");
        }
    }


    // Bỏ trang bị
    public void UnequipItem(ItemData item)
    {
        if (equippedItems.Contains(item))
        {
            equippedItems.Remove(item);
            SaveInventory();
            if (player != null) player.ApplyEquipmentStats(false); // Không hồi đầy máu khi unequip giữa trận
        }
        else
        {
            Debug.LogWarning("Item không được trang bị!");
        }
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
    public void LoadInventory(List<ItemData> allItems, bool resetVitals = false)
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
        if (player != null) player.ApplyEquipmentStats(resetVitals);
    }
}