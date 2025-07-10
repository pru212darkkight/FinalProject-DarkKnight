using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    public float elapsedTime = 0f;
    private bool isRunning = true;

    void Update()
    {
        if (isRunning)
            elapsedTime += Time.deltaTime;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public string GetTimeString()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        isRunning = true;
    }
}
