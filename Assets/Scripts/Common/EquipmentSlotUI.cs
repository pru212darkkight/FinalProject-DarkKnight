using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour, IDropHandler
{
    public ItemType slotType;
    public Image icon;
    public Inventory inventory;

    private ItemData currentItem;

    public void Refresh(List<ItemData> equippedItems, System.Action<ItemData> onClick, Inventory inventory)
    {
        this.inventory = inventory;
        currentItem = equippedItems.Find(i => i.itemType == slotType);
        if (currentItem != null)
        {
            icon.sprite = currentItem.icon;
            icon.enabled = true;
            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(() => onClick(currentItem));
        }
        else
        {
            icon.enabled = false;
            GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }


    public bool CanEquip(ItemData item) => item.itemType == slotType;

    public void Equip(ItemData item)
    {
        var oldItem = currentItem;
        if (oldItem != null) inventory.UnequipItem(oldItem);
        inventory.EquipItem(item);
    }

    public void OnDrop(PointerEventData eventData)
    {
        var draggedItem = eventData.pointerDrag.GetComponent<ItemSlotUI>()?.item;
        if (draggedItem != null && CanEquip(draggedItem))
        {
            Equip(draggedItem);
            FindAnyObjectByType<InventoryUI>().RefreshUI();
        }
    }
}
