using UnityEngine;

public class BringToFront : MonoBehaviour
{
    [SerializeField] private RectTransform itemScrollView;

    void Start()
    {
        // Đảm bảo đối tượng được đặt làm sibling cuối cùng (hiển thị trên cùng)
        if (itemScrollView != null)
        {
            itemScrollView.SetAsLastSibling();
        }
    }
}
