using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Image icon;
    public ItemData item;
    private System.Action onClick;
    private System.Action<ItemData> onDrop;

    public void SetItem(ItemData item, System.Action onClick)
    {
        this.item = item;
        icon.sprite = item.icon;
        icon.enabled = true;
        this.onClick = onClick;
    }

    public void SetDragAndDrop(System.Action<ItemData> onDrop)
    {
        this.onDrop = onDrop;
    }

    // Click để mở info
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            onClick?.Invoke();
    }

    // Drag & drop (bạn hoàn thiện logic kéo icon, thả đúng slot)
    public void OnBeginDrag(PointerEventData eventData) { /* ... */ }
    public void OnDrag(PointerEventData eventData) { /* ... */ }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (onDrop != null)
            onDrop(item);
    }
}
