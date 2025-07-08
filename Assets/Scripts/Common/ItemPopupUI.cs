using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// HƯỚNG DẪN:
// Để GameObject ItemPopupUI luôn bật (SetActive = true) trong scene.
// Script sẽ tự động bật/tắt phần hiển thị khi cần, không cần tắt GameObject cha.

public class ItemPopupUI : MonoBehaviour
{
    public Image iconImage;
    public Text nameText;
    public RectTransform rectTransform; // Kéo RectTransform của popup vào đây
    public GameObject UIImage;
    public Vector2 startPos = new Vector2(0, -200); // Vị trí bắt đầu (dưới)
    public Vector2 endPos = new Vector2(0, 0);      // Vị trí kết thúc (giữa)
    public float moveDuration = 0.5f;               // Thời gian di chuyển

    void Awake()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = startPos;
        // Để GameObject luôn bật, không tắt ở đây!
    }

    public void Show(ItemData item)
    {
        if (iconImage != null)
            iconImage.sprite = item.icon;
        if (nameText != null)
            nameText.text = item.itemName;
        UIImage.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(MoveUpAndHide());
    }

    IEnumerator MoveUpAndHide()
    {
        float t = 0;
        rectTransform.anchoredPosition = startPos;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t / moveDuration);
            yield return null;
        }
        rectTransform.anchoredPosition = endPos;
        yield return new WaitForSeconds(1.5f); // Hiện ở giữa 1.5s
        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
} 