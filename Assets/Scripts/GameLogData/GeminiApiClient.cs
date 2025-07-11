using UnityEngine;
using System.Text;
using System.Collections;
using Newtonsoft.Json;
using System;
using UnityEngine.Networking;

public class GeminiApiClient : MonoBehaviour
{
    public string geminiApiKey = "AIzaSyB0WJDcp-3yNNXJfZAOHxBTl5VjDR5blIg";
    public string model = "gemini-1.5-flash-latest";

    // Prompt mẫu: giới hạn 100 ký tự, ngắn gọn, 1 câu, không dài dòng
    private string GetAdvicePrompt()
    {
        return @"
        Bạn là cố vấn AI cho game hành động sinh tồn.
        - Hãy PHÂN TÍCH rõ nguyên nhân thua, nêu tên những quái vật gây sát thương cao nhất (ghi rõ loại sát thương: phép/vật lý, số damage).
        - Chỉ ra các chỉ số yếu của player, gợi ý nâng gì.
        - Đề xuất trang bị cần mua: GHI RÕ TÊN, GIÁ, các chỉ số/ưu điểm và tác dụng giúp vượt map.
        - Lý giải tại sao chọn trang bị đó và cần cải thiện điểm gì.
        - Viết ngắn gọn, rõ ràng, liệt kê từng ý theo gạch đầu dòng. Không giải thích lại dữ liệu.
        - Trả về đầy đủ các phần: nguyên nhân thua, phân tích điểm yếu, đề xuất trang bị, lời khuyên chiến thuật.";
    }


    public IEnumerator GetAdviceFromGemini(string prompt, string jsonLog, Action<string> onComplete, int retryCount = 3)
    {
        string url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={geminiApiKey}";

        // Nếu prompt truyền vào rỗng thì lấy mặc định
        if (string.IsNullOrWhiteSpace(prompt))
            prompt = GetAdvicePrompt();

        // Gói prompt + log vào message cho AI
        var payload = new
        {
            contents = new[] {
                new {
                    role = "user",
                    parts = new[] {
                        new {
                            text = prompt + "\nDữ liệu trận thua vừa rồi:\n" + jsonLog
                        }
                    }
                }
            }
        };

        string payloadJson = JsonConvert.SerializeObject(payload);

        for (int attempt = 0; attempt < retryCount; attempt++)
        {
            using (UnityWebRequest req = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(payloadJson);
                req.uploadHandler = new UploadHandlerRaw(bodyRaw);
                req.downloadHandler = new DownloadHandlerBuffer();
                req.SetRequestHeader("Content-Type", "application/json");

                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                {
                    string responseText = req.downloadHandler.text;
                    string advice = ParseAdviceFromGeminiResponse(responseText);

                    // Đảm bảo lời khuyên không quá 100 ký tự (phòng trường hợp AI trả về dài)
                    if (advice.Length > 100)
                        advice = advice.Substring(0, 100) + "...";

                    onComplete?.Invoke(advice);
                    yield break;
                }
                else if (req.responseCode == 503 && attempt < retryCount - 1)
                {
                    Debug.LogWarning("Gemini API overloaded, thử lại sau 3s...");
                    yield return new WaitForSecondsRealtime(3f);
                    continue;
                }
                else
                {
                    Debug.LogError("Gemini API error: " + req.error + "\n" + req.downloadHandler.text);
                    onComplete?.Invoke("Không lấy được lời khuyên, kiểm tra kết nối hoặc API Key!");
                    yield break;
                }
            }
        }
    }

    // Parse text trả về từ Gemini API
    string ParseAdviceFromGeminiResponse(string responseText)
    {
        try
        {
            var root = JsonConvert.DeserializeObject<GeminiResponseRoot>(responseText);
            if (root != null && root.candidates != null && root.candidates.Length > 0)
            {
                var candidate = root.candidates[0];
                if (candidate.content != null && candidate.content.parts != null && candidate.content.parts.Length > 0)
                {
                    return candidate.content.parts[0].text.Trim();
                }
            }
            return "Không có lời khuyên hợp lệ từ Gemini.";
        }
        catch (Exception ex)
        {
            Debug.LogError("Parse Gemini response error: " + ex.Message);
            return "Lỗi phân tích dữ liệu trả về từ Gemini!";
        }
    }

    // Lớp parse JSON cho Gemini
    [Serializable]
    public class GeminiResponseRoot
    {
        public Candidate[] candidates;
    }

    [Serializable]
    public class Candidate
    {
        public Content content;
    }

    [Serializable]
    public class Content
    {
        public Part[] parts;
    }

    [Serializable]
    public class Part
    {
        public string text;
    }
}
