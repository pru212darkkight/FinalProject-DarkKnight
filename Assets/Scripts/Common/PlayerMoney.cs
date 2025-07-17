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

        // Notify shop managers về money change (quan trọng cho WebGL)
        NotifyShopManagers();
    }

    void NotifyShopManagers()
    {
        // Tìm tất cả ShopManager và update UI
        ShopManager[] shopManagers = FindObjectsOfType<ShopManager>();
        foreach (var shop in shopManagers)
        {
            if (shop != null)
            {
                shop.OnPlayerMoneyChanged();
            }
        }
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
        try
        {
            PlayerPrefs.SetInt("PlayerCoins", coins);
            PlayerPrefs.Save();
            Debug.Log($"💾 Money saved: {coins} coins");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"🚨 Failed to save money: {e.Message}");
        }
    }

    public void LoadMoney()
    {
        try
        {
            coins = PlayerPrefs.GetInt("PlayerCoins", 0);
            Debug.Log($"💰 Money loaded: {coins} coins");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"🚨 Failed to load money: {e.Message}");
            coins = 0; // Fallback value
        }
    }

    public void UpdateUI()
    {
        if (coinText != null)
            coinText.text = coins.ToString();

        // Cập nhật tất cả shop UI khi money thay đổi
        NotifyShopManagers();
    }

    // Method để force refresh tất cả money UI
    [ContextMenu("Force Update All Money UI")]
    public void ForceUpdateAllUI()
    {
        LoadMoney(); // Reload từ PlayerPrefs
        UpdateUI();
        NotifyShopManagers();
        Debug.Log($"🔄 All money UI force updated. Current coins: {coins}");
    }
} 