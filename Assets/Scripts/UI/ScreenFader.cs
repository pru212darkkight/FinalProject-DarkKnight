using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    public static ScreenFader Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        if (fadeImage != null)
        {
            var c = fadeImage.color;
            c.a = 0f; // Mặc định là trong suốt, không đen
            fadeImage.color = c;
            fadeImage.raycastTarget = false;
        }
    }

    void OnEnable()
    {
        // Không set alpha = 1 ở đây nữa, chỉ giữ mặc định alpha = 0
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
        yield return FadeInWithDelay(0.1f);
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
} 