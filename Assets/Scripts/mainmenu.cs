using UnityEngine;

public class mainmenu : MonoBehaviour
{
    [Header("Main Menu Settings")]
    public bool playMusicOnStart = true;
    public bool debugMode = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playMusicOnStart)
        {
            PlayMainMenuMusic();
        }

    }

    void PlayMainMenuMusic()
    {
        // Stop current music first
        if (AudioManager.Instance != null && AudioManager.Instance.musicSource != null)
        {
            AudioManager.Instance.musicSource.Stop();

            if (debugMode)
            {
                Debug.Log("🔇 MainMenu: Stopped previous music before playing Main Menu music");
            }
        }

        // Play Main Menu music khi scene load
        if (AudioManager.Instance != null && AudioManager.Instance.mainMenu != null)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.mainMenu);

            if (debugMode)
            {
                Debug.Log("🎵 MainMenu: Playing Main Menu music");
            }
        }
        else
        {
            // Debug error messages
            if (AudioManager.Instance == null)
            {
                Debug.LogError("🚨 MainMenu: AudioManager.Instance is null!");
            }
            else if (AudioManager.Instance.mainMenu == null)
            {
                Debug.LogError("🚨 MainMenu: AudioManager.Instance.mainMenu is null! Please assign Main Menu music clip in AudioManager.");
            }
        }
    }

    [ContextMenu("Play Main Menu Music")]
    public void ForcePlayMainMenuMusic()
    {
        PlayMainMenuMusic();
    }

    [ContextMenu("Stop Music")]
    public void StopMusic()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.musicSource != null)
        {
            AudioManager.Instance.musicSource.Stop();

            if (debugMode)
            {
                Debug.Log("🔇 MainMenu: Music stopped");
            }
        }
    }

    // Method để play button click sound
    public void PlayButtonClickSound()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.buttonClick != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClick);

            if (debugMode)
            {
                Debug.Log("🔘 MainMenu: Button click sound played");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
