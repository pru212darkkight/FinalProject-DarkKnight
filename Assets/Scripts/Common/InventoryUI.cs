using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    [Header("Panel & Prefab")]
    public GameObject inventoryPanel;
    public GameObject itemSlotPrefab;
    public GameObject emptySlotPrefab;
    public Transform inventoryGrid;

    [Header("Trang bị")]
    public EquipmentSlotUI[] equipmentSlots; // 6 slot, gán trong Inspector (Sword, Ring, Pants, Armor, Boots, Helmet)

    [Header("Logic")]
    public Inventory inventory; // Tham chiếu Inventory script
    public ItemInfoPanel itemInfoPanel; // Script hiển thị thông tin item

    [Header("Cài đặt")]
    public int inventorySlotCount = 36;

    public PlayerStatsPanel playerStatsPanel;
    private bool isOpen = false;

    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;

        // Nếu đóng Inventory thì phải trả lại Time.timeScale trước khi tắt panel!
        if (!isOpen)
        {
            Time.timeScale = 1; // Resume game
            inventoryPanel.SetActive(false);
            itemInfoPanel.Hide();
        }
        else
        {
            inventoryPanel.SetActive(true);
            Time.timeScale = 0; // Pause game
            RefreshUI();
        }
    }


    public void RefreshUI()
    {
        // Xóa slot cũ
        foreach (Transform child in inventoryGrid) Destroy(child.gameObject);

        // Lấy item chưa trang bị
        var bagItems = inventory.ownedItems.Where(i => !inventory.equippedItems.Contains(i)).ToList();
        int emptySlots = Mathf.Max(0, inventorySlotCount - bagItems.Count);

        // Hiện item trong kho
        foreach (var item in bagItems)
        {
            var slot = Instantiate(itemSlotPrefab, inventoryGrid);
            var ui = slot.GetComponent<ItemSlotUI>();
            ui.SetItem(item, () => ShowItemInfo(item));
            ui.SetDragAndDrop(
                // Kéo từ kho vào slot trang bị
                (draggedItem) =>
                {
                    foreach (var equipSlot in equipmentSlots)
                    {
                        if (equipSlot.CanEquip(draggedItem))
                        {
                            equipSlot.Equip(draggedItem);
                            RefreshUI();
                            break;
                        }
                    }
                }
            );
        }
        // Hiện slot trống (nếu còn chỗ)
        for (int i = 0; i < emptySlots; i++)
        {
            Instantiate(emptySlotPrefab, inventoryGrid);
        }

        // Cập nhật slot trang bị
        foreach (var slot in equipmentSlots)
        {
            slot.Refresh(inventory.equippedItems, (item) => ShowItemInfo(item), inventory);
        }
        playerStatsPanel.UpdateStats(inventory.player);

    }

    public void ShowItemInfo(ItemData item)
    {
        bool isEquipped = inventory.equippedItems.Contains(item);
        itemInfoPanel.ShowInfo(item, isEquipped, this); // Truyền this
    }


    public void OnEquipItem(ItemData item)
    {
        inventory.EquipItem(item);
        RefreshUI();
    }

    public void OnRemoveItem(ItemData item)
    {
        inventory.UnequipItem(item);
        RefreshUI();
    }
}
