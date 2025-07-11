using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject victoryPanel;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI goldText;
    public Button okButton;

    public PlayerMoney playerMoney;
    public LevelTimer levelTimer;

    public Transform portalSpawnPoint;
    public GameObject portalPrefab;

    public CameraFocusManager cameraFocusManager;

    void Start()
    {
        victoryPanel.SetActive(false);
        okButton.onClick.AddListener(OnOKClick);
        playerMoney.ResetSessionCoins();
        levelTimer.ResetTimer();
    }

    public void OnBossDefeated()
    {
        // Dừng timer
        levelTimer.StopTimer();

        // Hiện panel và fill data
        timeText.text = "Time: " + levelTimer.GetTimeString();
        goldText.text = "Gold: " + playerMoney.sessionCoins.ToString();

        victoryPanel.SetActive(true);
        //Time.timeScale = 0f; // Pause game
    }

    void OnOKClick()
    {
        victoryPanel.SetActive(false);
        //Time.timeScale = 1f; // Resume game

        // Spawn portal và lưu lại instance
        var portalInstance = Instantiate(portalPrefab, portalSpawnPoint.position, Quaternion.identity);

        // Camera pan tới portal rồi quay lại player
        cameraFocusManager.FocusPortalThenBack(portalInstance.transform, 2f); // 2 giây, chỉnh theo ý bạn
    }

}
