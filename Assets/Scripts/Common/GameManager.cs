using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public bool isHomeVillage = false; // tick ở scene Home Village
    public GameObject victoryPanel;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI goldText;
    public Button okButton;

    public PlayerMoney playerMoney;
    public LevelTimer levelTimer;

    public Transform portalSpawnPoint;
    public GameObject portalPrefab;

    public CameraFocusManager cameraFocusManager;
    [Header("Defeat Panel")]
    public GameObject defeatPanel;
    public TextMeshProUGUI defeatTimeText;
    public TextMeshProUGUI defeatGoldText;
    public Button homeButton;
    public Button playAgainButton;

    public static LastDefeatLog lastDefeatLogInstance; // để dùng cross scene
    void Start()
    {
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        okButton.onClick.AddListener(OnOKClick);
        homeButton.onClick.AddListener(OnHomeClick);
        playAgainButton.onClick.AddListener(OnPlayAgainClick);
        playerMoney.ResetSessionCoins();
        levelTimer.ResetTimer();

        // 🎵 Play scene-specific music
        if (isHomeVillage)
        {
            PlayHomeVillageMusic();
        }
        else
        {
            PlaySceneMusic();
        }
    }
    public void ShowDefeatPanel()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.defeatEffect != null)
        {
            AudioManager.Instance.PlayRandomSFX(AudioManager.Instance.defeatEffect);
            Debug.Log("🚪 Boss door closing - playing defeat sound!");
        }
        if (isHomeVillage) return;
        levelTimer.StopTimer();
        defeatTimeText.text = "Time: " + levelTimer.GetTimeString();
        defeatGoldText.text = "Gold: " + playerMoney.sessionCoins.ToString();
        // === Ghi lại log thua tại đây ===
        SaveDefeatLogOnLose();
        defeatPanel.SetActive(true);
        Time.timeScale = 0f;
    }
    void SaveDefeatLogOnLose()
    {
        lastDefeatLogInstance = new LastDefeatLog
        {
            timeSurvived = levelTimer.elapsedTime,
            playerGold = playerMoney.coins,
            topDamageEnemy = GameDefeatData.lastTopDamageEnemy,
            deathReason = GameDefeatData.lastDeathReason,
            damageFromEachEnemy = new Dictionary<string, DamageLog>(GameDefeatData.damageFromEachEnemy)
        };
    }
    void PlayButtonClickSound()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.buttonClick != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClick);
        }
    }

    void OnHomeClick()
    {
        PlayButtonClickSound();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Home Village");
    }

    void OnPlayAgainClick()
    {
        PlayButtonClickSound();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnBossDefeated()
    {
        if (isHomeVillage) return;
        // Dừng timer
        levelTimer.StopTimer();

        // Hiện panel và fill data
        timeText.text = "Time: " + levelTimer.GetTimeString();
        goldText.text = "Gold: " + playerMoney.sessionCoins.ToString();

        if (AudioManager.Instance != null && AudioManager.Instance.victoryEffect != null)
        {
            AudioManager.Instance.PlayRandomSFX(AudioManager.Instance.victoryEffect);
            Debug.Log("🚪 Boss door closing - playing door sound!");
        }
        victoryPanel.SetActive(true);
        Time.timeScale = 0f; // Pause game
    }

    void PlayHomeVillageMusic()
    {
        // Stop current music first
        if (AudioManager.Instance != null && AudioManager.Instance.musicSource != null)
        {
            AudioManager.Instance.musicSource.Stop();
            Debug.Log("🔇 GameManager: Stopped previous music before playing Home Village music");
        }

        // Play Home Village music khi scene load
        if (AudioManager.Instance != null && AudioManager.Instance.homeVillage != null)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.homeVillage);
            Debug.Log("🏘️ GameManager: Playing Home Village music");
        }
        else
        {
            // Debug error messages
            if (AudioManager.Instance == null)
            {
                Debug.LogError("🚨 GameManager: AudioManager.Instance is null!");
            }
            else if (AudioManager.Instance.homeVillage == null)
            {
                Debug.LogError("🚨 GameManager: AudioManager.Instance.homeVillage is null! Please assign Home Village music clip in AudioManager.");
            }
        }
    }

    [ContextMenu("Play Home Village Music")]
    public void ForcePlayHomeVillageMusic()
    {
        PlayHomeVillageMusic();
    }

    void PlaySceneMusic()
    {
        if (AudioManager.Instance == null) return;

        // Stop current music first
        if (AudioManager.Instance.musicSource != null)
        {
            AudioManager.Instance.musicSource.Stop();
            Debug.Log("🔇 GameManager: Stopped previous music before playing scene music");
        }

        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        AudioClip musicToPlay = null;

        // Determine which music to play based on scene name
        if (sceneName.Contains("Map 1"))
        {
            musicToPlay = AudioManager.Instance.map1;
        }
        else if (sceneName.Contains("Map 2"))
        {
            musicToPlay = AudioManager.Instance.map2;
        }
        else if (sceneName.Contains("Map 3"))
        {
            musicToPlay = AudioManager.Instance.map3;
        }
        else if (sceneName.Contains("Map 4"))
        {
            musicToPlay = AudioManager.Instance.map4;
        }
        else if (sceneName.Contains("Map 5"))
        {
            // Map 5 might have its own setup, so we can skip or add map5 clip
            Debug.Log("🎵 GameManager: Map 5 detected, skipping auto music (might have own setup)");
            return;
        }

        // Play the determined music
        if (musicToPlay != null)
        {
            AudioManager.Instance.PlayMusic(musicToPlay);
            Debug.Log($"🎵 GameManager: Playing music for {sceneName}");
        }
        else
        {
            Debug.LogWarning($"🚨 GameManager: No music assigned for scene {sceneName}");
        }
    }

    [ContextMenu("Stop Music")]
    public void StopMusic()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.musicSource != null)
        {
            AudioManager.Instance.musicSource.Stop();
            Debug.Log("🔇 GameManager: Music stopped");
        }
    }

    void OnOKClick()
    {
        PlayButtonClickSound();
        victoryPanel.SetActive(false);
        Time.timeScale = 1f; // Resume game

        // Spawn portal nếu có prefab và spawn point
        if (portalPrefab != null && portalSpawnPoint != null)
        {
            var portalInstance = Instantiate(portalPrefab, portalSpawnPoint.position, Quaternion.identity);

            // Camera pan tới portal rồi quay lại player (nếu có camera manager)
            if (cameraFocusManager != null)
            {
                cameraFocusManager.FocusPortalThenBack(portalInstance.transform, 2f);
            }
            else
            {
                Debug.LogWarning("GameManager: cameraFocusManager is null!");
            }
        }
        else
        {
            Debug.LogWarning("GameManager: portalPrefab or portalSpawnPoint is null!");
        }
    }

}
