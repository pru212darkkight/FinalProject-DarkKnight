using UnityEngine;

public class SetUp : MonoBehaviour
{
    void Start()
    {
        // Play Map 2 music khi scene load
        if (AudioManager.Instance != null && AudioManager.Instance.map2 != null)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.map2);
            Debug.Log("🎵 Map2Setup: Playing Map 2 music");
        }
        else
        {
            if (AudioManager.Instance == null)
                Debug.LogError("🚨 Map2Setup: AudioManager.Instance is null!");
            if (AudioManager.Instance != null && AudioManager.Instance.map2 == null)
                Debug.LogError("🚨 Map2Setup: AudioManager.Instance.map2 is null!");
        }
    }
}
