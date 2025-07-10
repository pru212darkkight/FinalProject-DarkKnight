using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    public static ScreenFader Instance;

    void Awake()
    {
        // Singleton pattern với DontDestroyOnLoad
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Khởi tạo fadeImage nếu chưa có
            if (fadeImage == null)
            {
                fadeImage = GetComponentInChildren<Image>();
            }
            
            if (fadeImage != null)
            {
                var c = fadeImage.color;
                c.a = 0f; // Mặc định là trong suốt
                fadeImage.color = c;
                fadeImage.raycastTarget = false;
            }
        }
        else if (Instance != this)
        {
            // Nếu đã có instance khác, destroy object này
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Kiểm tra và tạo lại fadeImage nếu bị mất khi sang scene mới
        if (fadeImage == null)
        {
            var img = GetComponentInChildren<Image>();
            if (img == null)
            {
                // Gọi lại CreateScreenFader để tạo lại UI
                CreateScreenFader();
            }
            else
            {
                fadeImage = img;
            }
        }

        // Nếu màn hình đang đen, tự động fade in
        if (fadeImage != null && fadeImage.color.a > 0.99f)
        {
            StartCoroutine(FadeInAfterFirstFrame());
        }
    }

    void Start()
    {
        // Chỉ tự động fade in nếu alpha = 1 (tức là vừa chuyển scene)
        if (fadeImage != null && fadeImage.color.a > 0.99f)
        {
            StartCoroutine(FadeInAfterFirstFrame());
        }
    }

    IEnumerator FadeInAfterFirstFrame()
    {
        yield return null; // Chờ 1 frame để đảm bảo Image đã render
        yield return FadeInWithDelay(0.05f);
    }

    public IEnumerator FadeOut()
    {
        if (fadeImage == null) yield break;
        fadeImage.raycastTarget = true;
        float t = 0f;
        Color c = fadeImage.color;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Clamp01(t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = 1f;
        fadeImage.color = c;
    }

    public IEnumerator FadeIn()
    {
        if (fadeImage == null) yield break;
        float t = 0f;
        Color c = fadeImage.color;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            c.a = 1f - Mathf.Clamp01(t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = 0f;
        fadeImage.color = c;
        fadeImage.raycastTarget = false;
    }

    IEnumerator FadeInWithDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        yield return FadeIn();
    }

    // Cho phép gọi fade in từ code khác nếu cần
    public static void FadeInOnSceneStart()
    {
        if (Instance != null)
        {
            Instance.StartCoroutine(Instance.FadeIn());
        }
    }

    // Phương thức để tạo ScreenFader nếu chưa có
    public static ScreenFader CreateScreenFader()
    {
        if (Instance == null)
        {
            GameObject faderObject = new GameObject("ScreenFader");
            ScreenFader fader = faderObject.AddComponent<ScreenFader>();
            
            // Tạo UI Image cho fade
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObject = new GameObject("FadeCanvas");
                canvas = canvasObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 999; // Đảm bảo hiển thị trên cùng
                canvasObject.AddComponent<CanvasScaler>();
                canvasObject.AddComponent<GraphicRaycaster>();
            }
            
            GameObject imageObject = new GameObject("FadeImage");
            imageObject.transform.SetParent(canvas.transform, false);
            
            Image fadeImage = imageObject.AddComponent<Image>();
            fadeImage.color = Color.black;
            fadeImage.raycastTarget = false;
            
            // Set RectTransform để phủ toàn màn hình
            RectTransform rectTransform = fadeImage.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            fader.fadeImage = fadeImage;
            faderObject.transform.SetParent(canvas.transform);
        }
        
        return Instance;
    }
} 