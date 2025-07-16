using UnityEngine;

public class Map5Setup : MonoBehaviour
{
    [Header("Map 5 Music Setup")]
    public bool playMusicOnStart = true;
    public bool debugMode = true;
    
    void Start()
    {
        if (playMusicOnStart)
        {
            PlayMap5Music();
        }
        
        if (debugMode)
        {
            Debug.Log("🎮 Map5Setup: Map 5 scene initialized!");
        }
    }
    
    void PlayMap5Music()
    {
        // Play Map 5 music khi scene load
        if (AudioManager.Instance != null && AudioManager.Instance.map5 != null)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.map5);
            
            if (debugMode)
            {
                Debug.Log("🎵 Map5Setup: Playing Map 5 music");
            }
        }
        else
        {
            // Debug error messages
            if (AudioManager.Instance == null)
            {
                Debug.LogError("🚨 Map5Setup: AudioManager.Instance is null!");
            }
            else if (AudioManager.Instance.map5 == null)
            {
                Debug.LogError("🚨 Map5Setup: AudioManager.Instance.map5 is null! Please assign Map 5 music clip in AudioManager.");
            }
        }
    }
    
    [ContextMenu("Play Map 5 Music")]
    public void ForcePlayMap5Music()
    {
        PlayMap5Music();
    }
    
    [ContextMenu("Stop Music")]
    public void StopMusic()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.musicSource != null)
        {
            AudioManager.Instance.musicSource.Stop();
            
            if (debugMode)
            {
                Debug.Log("🔇 Map5Setup: Music stopped");
            }
        }
    }
}
