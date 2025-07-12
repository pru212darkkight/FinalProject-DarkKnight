using TMPro;
using UnityEngine;

public class AdvicePanelController : MonoBehaviour
{
    public DefeatLogger defeatLogger;
    public GeminiApiClient geminiApiClient;
    public TextMeshProUGUI adviceText;
    public GameObject advicePanel;

    private string adviceContent = null;
    private bool isAdviceReady = false;
    private bool isRequestingAdvice = false;
    private bool panelActive = false;

    public void ToggleAdvicePanel()
    {
        if (!panelActive)
        {
            ShowPanel();
        }
        else
        {
            HideAdvice();
        }
    }

    public void ShowPanel()
    {
        advicePanel.SetActive(true);
        panelActive = true;
        Time.timeScale = 0;

        // Nếu đã có content, hiện luôn
        if (isAdviceReady && !string.IsNullOrEmpty(adviceContent))
        {
            adviceText.text = adviceContent;
        }
        else
        {
            adviceText.text = "Đang đợi AI cho lời khuyên, vui lòng chờ...";
            if (!isRequestingAdvice)
                RequestAdvice();
        }
    }

    public void HideAdvice()
    {
        advicePanel.SetActive(false);
        panelActive = false;
        Time.timeScale = 1;
    }

    public void RequestAdvice()
    {
        if (isRequestingAdvice) return;
        isRequestingAdvice = true;
        isAdviceReady = false;
        adviceContent = null;

        if (GameManager.lastDefeatLogInstance == null)
        {
            adviceText.text = "Không có dữ liệu trận thua gần đây!";
            isRequestingAdvice = false;
            return;
        }

        var logData = defeatLogger.BuildGeminiRequest(GameManager.lastDefeatLogInstance);
        if (logData == null)
        {
            adviceText.text = "Không thể tạo log dữ liệu cho AI!";
            isRequestingAdvice = false;
            return;
        }

        string jsonLog = Newtonsoft.Json.JsonConvert.SerializeObject(logData, Newtonsoft.Json.Formatting.Indented);
        string prompt =
        @"Bạn là cố vấn AI cho game hành động sinh tồn.
- Phân tích nguyên nhân thua chi tiết, liệt kê rõ từng quái vật gây sát thương lớn nhất, loại damage (vật lý/phép) và lượng damage.
- Phân tích các điểm yếu về chỉ số nhân vật, kỹ năng, trang bị.
- Gợi ý nên nâng chỉ số gì, cải thiện chiến thuật ra sao.
- Đề xuất cụ thể tên trang bị nên mua, giá tiền, ưu điểm và tác dụng khi mang vào trận.
- Lý do chọn các trang bị, vì sao phù hợp.
- Trả lời thành các phần, **không được bỏ trống mục nào, không được kết thúc dòng bằng dấu ba chấm hoặc bị cắt cụt**. Nếu thiếu thông tin, ghi rõ là “Không đủ dữ liệu”.
- Đáp án dạng markdown hoặc bullet point, tối đa 10 dòng, không tự ý tóm tắt.";

        StartCoroutine(geminiApiClient.GetAdviceFromGemini(prompt, jsonLog, (advice) =>
        {
            adviceContent = advice;
            isAdviceReady = true;
            isRequestingAdvice = false;
            // Nếu panel vẫn đang mở, cập nhật text luôn
            if (panelActive)
            {
                adviceText.text = adviceContent;
            }
            GameDefeatData.Reset();
        }));
    }
}
