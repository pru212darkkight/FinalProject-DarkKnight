using UnityEngine;

public class CoinDrop : MonoBehaviour
{
    [Header("Coin Drop Settings")]
    public int coinAmount = 1;
    public bool showPopup = true;

    // Gọi hàm này khi enemy chết
    public void DropCoin()
    {
        if (coinAmount > 0)
        {
            PlayerMoney money = FindAnyObjectByType<PlayerMoney>();
            if (money != null)
            {
                money.AddCoins(coinAmount);

                // Phát âm thanh rớt tiền
                if (AudioManager.Instance != null && AudioManager.Instance.coinDrop != null)
                {
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.coinDrop);
                }

                if (showPopup)
                {
                    // Tìm popup theo tag để phân biệt với rương
                    GameObject popupObj = GameObject.FindGameObjectWithTag("enemyCoin");
                    if (popupObj != null)
                    {
                        ItemPopupUI popup = popupObj.GetComponent<ItemPopupUI>();
                        if (popup != null)
                        {
                            Debug.Log($"Found enemy coin popup, showing coin at position: {transform.position}");
                            popup.ShowCoinAtWorldPosition(coinAmount, transform.position);
                        }
                        else
                        {
                            Debug.LogWarning("EnemyCoinPopup object does not have ItemPopupUI component!");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("EnemyCoinPopup not found in scene! Make sure there's a popup with tag 'EnemyCoinPopup'.");
                    }
                }
            }
            else
            {
                Debug.LogWarning("PlayerMoney not found in scene!");
            }
        }
    }
} 