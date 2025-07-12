using UnityEngine;
using System.Text;
using System.Collections;
using Newtonsoft.Json;
using System;
using UnityEngine.Networking;

public class GeminiApiClient : MonoBehaviour
{
    public string geminiApiKey = "AIzaSyB0WJDcp-3yNNXJfZAOHxBTl5VjDR5blIg";
    public string model = "gemini-1.5-pro-latest";



    public IEnumerator GetAdviceFromGemini(string prompt, string jsonLog, Action<string> onComplete, int retryCount = 3)
    {
        string url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={geminiApiKey}";

        Debug.Log("<color=yellow>---- SEND TO GEMINI ----\nPROMPT:</color>\n" + prompt + "\n<color=yellow>JSON LOG:</color>\n" + jsonLog);

        // Gói prompt + log vào message cho AI
        var payload = new
        {
            contents = new[] {
                new {
                    role = "user",
                    parts = new[] {
                        new {
                            text = prompt + "\nDữ liệu trận thua vừa rồi:\n" + jsonLog + "\n Hãy đưa ra lời khuyên tối ưu về cách cải thiện chiến thuật hoặc nên mua/vận dụng vật phẩm nào (và giải thích lý do)."
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

                    Debug.Log("<color=green>---- RECEIVE FROM GEMINI ----\nRESPONSE:</color>\n" + responseText);

                    string advice = ParseAdviceFromGeminiResponse(responseText);
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
