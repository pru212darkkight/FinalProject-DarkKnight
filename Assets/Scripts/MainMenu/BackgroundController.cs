using UnityEngine;

public class BackgroundChanger : MonoBehaviour
{
    public GameObject[] backgrounds;      // Danh sách các ảnh nền là GameObject
    public float changeInterval = 2f;     // Thời gian đổi ảnh (giây)

    private int currentIndex = 0;
    private float timer;

    void Start()
    {
        ShowOnlyCurrentBackground();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= changeInterval)
        {
            timer = 0f;
            ShowNextBackground();
        }
    }

    void ShowOnlyCurrentBackground()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].SetActive(i == currentIndex);
        }
    }

    void ShowNextBackground()
    {
        currentIndex = (currentIndex + 1) % backgrounds.Length;
        ShowOnlyCurrentBackground();
    }
}
