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
    }
    public void ShowDefeatPanel()
    {
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
    void OnHomeClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Home Village");
    }

    void OnPlayAgainClick()
    {
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

        victoryPanel.SetActive(true);
        Time.timeScale = 0f; // Pause game
    }

    void OnOKClick()
    {
        victoryPanel.SetActive(false);
        Time.timeScale = 1f; // Resume game

        // Spawn portal và lưu lại instance
        var portalInstance = Instantiate(portalPrefab, portalSpawnPoint.position, Quaternion.identity);

        // Camera pan tới portal rồi quay lại player
        cameraFocusManager.FocusPortalThenBack(portalInstance.transform, 2f); // 2 giây, chỉnh theo ý bạn
    }

}
