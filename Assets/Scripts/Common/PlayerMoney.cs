using UnityEngine;
using TMPro;

public class PlayerMoney : MonoBehaviour
{
    public int coins = 0;
    public TextMeshProUGUI coinText; // Kéo UI Text vào nếu muốn hiển thị

    void Start()
    {
        LoadMoney();
        UpdateUI();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateUI();
        SaveMoney();
        Debug.Log($"Nhận {amount} xu. Tổng: {coins}");
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