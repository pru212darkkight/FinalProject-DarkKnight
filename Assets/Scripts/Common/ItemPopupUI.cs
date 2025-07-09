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
    public GameObject animatedCoinObject; // Kéo GameObject con (icon xu động) vào đây

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
        // if (nameText != null)
        //     nameText.text = item.itemName;
        UIImage.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(MoveUpAndHide());
    }

    public void ShowCoin(int amount)
    {
        if (animatedCoinObject != null)
            animatedCoinObject.SetActive(true);

        if (nameText != null)
            nameText.text = $"+{amount} ";
        // UIImage.SetActive(true); // Nếu cần panel cha hiện lên
        StopAllCoroutines();
        StartCoroutine(MoveUpAndHide());
    }

    public void ShowCoinAtWorldPosition(int amount, Vector3 worldPosition)
    {
        Debug.Log($"ShowCoinAtWorldPosition called: amount={amount}, worldPosition={worldPosition}");
        
        // Đảm bảo popup được kích hoạt
        gameObject.SetActive(true);
        if (UIImage != null)
            UIImage.SetActive(true);
        if (animatedCoinObject != null)
            animatedCoinObject.SetActive(true);

        if (nameText != null)
            nameText.text = $"+{amount}";

        // Đặt vị trí popup theo worldPosition
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        Vector2 canvasPos;
        bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            Camera.main.WorldToScreenPoint(worldPosition),
            Camera.main,
            out canvasPos
        );
        
        if (!success)
        {
            Debug.LogWarning("Failed to convert world position to canvas position! Using center position.");
            // Fallback: đặt ở giữa màn hình
            canvasPos = Vector2.zero;
        }
        
        Debug.Log($"World position: {worldPosition}, Canvas position: {canvasPos}");
        
        // Lưu vị trí bắt đầu và kết thúc cho animation
        Vector2 startPos = canvasPos;
        Vector2 endPos = canvasPos + new Vector2(0, 100); // Di chuyển lên 100 pixel
        
        rectTransform.anchoredPosition = startPos;
        Debug.Log($"Set popup position: startPos={startPos}, endPos={endPos}");

        StopAllCoroutines();
        StartCoroutine(MoveUpAndHideAtPosition(startPos, endPos));
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

    IEnumerator MoveUpAndHideAtPosition(Vector2 startPosition, Vector2 endPosition)
    {
        float t = 0;
        rectTransform.anchoredPosition = startPosition;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t / moveDuration);
            yield return null;
        }
        rectTransform.anchoredPosition = endPosition;
        yield return new WaitForSeconds(1.5f); // Hiện ở vị trí cuối 1.5s
        Hide();
    }

    public void Hide()
    {
        if (animatedCoinObject != null)
            animatedCoinObject.SetActive(false);
        if (UIImage != null)
            UIImage.SetActive(false);
        if (nameText != null)
            nameText.text = "";
        // KHÔNG gọi gameObject.SetActive(false);
    }
} 