using UnityEngine;
using TMPro;

public class PlayerMoney : MonoBehaviour
{
    public int coins = 0;
    public TextMeshProUGUI coinText; // Kéo UI Text vào nếu muốn hiển thị
    public int sessionCoins = 0; // Vàng kiếm trong màn hiện tại

    void Start()
    {
        LoadMoney();
        UpdateUI();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        sessionCoins += amount;
        UpdateUI();
        SaveMoney();
        Debug.Log($"Nhận {amount} xu. Tổng: {coins} | Màn này: {sessionCoins}");
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            UpdateUI();
            SaveMoney();
            Debug.Log($"Tiêu {amount} xu. Còn lại: {coins}");
            return true;
        }
        Debug.Log("Không đủ tiền!");
        return false;
    }
    // Gọi hàm này khi load scene mới/chơi lại màn:
    public void ResetSessionCoins()
    {
        sessionCoins = 0;
    }
    public void SaveMoney()
    {
        PlayerPrefs.SetInt("PlayerCoins", coins);
        PlayerPrefs.Save();
    }

    public void LoadMoney()
    {
        coins = PlayerPrefs.GetInt("PlayerCoins", 0);
    }

    public void UpdateUI()
    {
        if (coinText != null)
            coinText.text = coins.ToString();
    }
} 