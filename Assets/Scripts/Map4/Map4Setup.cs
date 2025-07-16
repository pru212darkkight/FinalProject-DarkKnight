using UnityEngine;

public class Map4Setup : MonoBehaviour
{
    [Header("Map 4 Settings")]
    public bool playMusicOnStart = true;
    public bool debugMode = true;

    void Start()
    {
        if (playMusicOnStart)
        {
            PlayMap4Music();
        }
        
        if (debugMode)
        {
            Debug.Log("🎮 Map4Setup: Map 4 scene initialized!");
        }
    }

    void PlayMap4Music()
    {
        // Play Map 4 music khi scene load
        if (AudioManager.Instance != null && AudioManager.Instance.map4 != null)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.map4);
            
            if (debugMode)
            {
                Debug.Log("🎵 Map4Setup: Playing Map 4 music");
            }
        }
        else
        {
            // Debug error messages
            if (AudioManager.Instance == null)
            {
                Debug.LogError("🚨 Map4Setup: AudioManager.Instance is null!");
            }
            else if (AudioManager.Instance.map4 == null)
            {
                Debug.LogError("🚨 Map4Setup: AudioManager.Instance.map4 is null! Please assign Map 4 music clip in AudioManager.");
            }
        }
    }

    [ContextMenu("Play Map 4 Music")]
    public void ForcePlayMap4Music()
    {
        PlayMap4Music();
    }
    
    [ContextMenu("Stop Music")]
    public void StopMusic()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.musicSource != null)
        {
            AudioManager.Instance.musicSource.Stop();
            
            if (debugMode)
            {
                Debug.Log("🔇 Map4Setup: Music stopped");
            }
        }
    }

    // Method để play button click sound (nếu có UI buttons trong Map 4)
    public void PlayButtonClickSound()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.buttonClick != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClick);
            
            if (debugMode)
            {
                Debug.Log("🔘 Map4Setup: Button click sound played");
            }
        }
    }

    // Method để stop current music và play Map 4 music
    public void SwitchToMap4Music()
    {
        if (AudioManager.Instance != null)
        {
            // Stop current music first
            if (AudioManager.Instance.musicSource != null)
            {
                AudioManager.Instance.musicSource.Stop();
            }
            
            // Play Map 4 music
            PlayMap4Music();
        }
    }
}
