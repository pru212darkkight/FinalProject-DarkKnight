using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pausePanel;
    public Button resumeButton;
    public Button homeButton;
    public Button retryButton;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip pauseClip; // Âm thanh phát khi pause

    private bool isPaused = false;

    void Start()
    {
        pausePanel.SetActive(false);
        resumeButton.onClick.AddListener(ResumeGame);
        homeButton.onClick.AddListener(GoToHome);
        retryButton.onClick.AddListener(RestartLevel);
    }

    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (isPaused) ResumeGame();
            else Pause();
        }
    }

    void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        if (audioSource != null && pauseClip != null)
        {
            audioSource.PlayOneShot(pauseClip);
        }
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void GoToHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Scenes/Home Village");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
