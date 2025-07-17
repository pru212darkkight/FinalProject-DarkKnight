using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GiftCodeManager : MonoBehaviour
{
    public GameObject panelGiftCode;
    public TMP_InputField inputCode;
    public Button btnEnter;
    public Button btnClose;
    public TextMeshProUGUI txtResult;

    // Tham chiếu tới Player, PlayerMoney
    public PlayerController1 player;
    public PlayerMoney playerMoney;
    public ShopManager shopManager;

    private void Start()
    {
        panelGiftCode.SetActive(false);
        btnEnter.onClick.AddListener(OnEnterCode);
        btnClose.onClick.AddListener(ClosePanel);
    }

    public void ShowPanel()
    {
        panelGiftCode.SetActive(true);
        inputCode.text = "";
        txtResult.text = "";
        inputCode.ActivateInputField();
    }

    public void ClosePanel()
    {
        panelGiftCode.SetActive(false);
        inputCode.text = "";
        txtResult.text = "";
    }

    void OnEnterCode()
    {
        string code = inputCode.text.Trim().ToLower();
        if (string.IsNullOrEmpty(code))
        {
            txtResult.text = "Vui lòng nhập mã code!";
            return;
        }

        bool isValid = false;

        switch (code)
        {
            case "hauhero":
                player.baseMaxHealth = 10000;
                player.baseStrength = 100;
                PlayerPrefs.SetFloat("PlayerBaseMaxHealth", 10000f); // Lưu lại
                PlayerPrefs.SetFloat("PlayerBaseStrength", 100f);
                PlayerPrefs.Save();

                player.ApplyEquipmentStats(true);
                player.UpdateUI();
                txtResult.text = "You have received 10000 health and 100 strength!";
                isValid = true;
                break;

            case "tintientai":
                playerMoney.AddCoins(9999);
                txtResult.text = "You have received 9999 gold!";
                isValid = true;

                if (shopManager != null)
                    shopManager.UpdateMoneyUI(); // ✅ cập nhật UI shop
                break;

            case "tuyentutung":
                player.baseMaxHealth = 100;
                player.baseStrength = 10;
                PlayerPrefs.SetFloat("PlayerBaseMaxHealth", 100f);
                PlayerPrefs.SetFloat("PlayerBaseStrength", 10f);
                PlayerPrefs.Save();

                player.ApplyEquipmentStats(true);
                playerMoney.coins = 0;
                playerMoney.SaveMoney();
                playerMoney.UpdateUI();
                txtResult.text = "Reset health to 100 and lost all gold!";
                isValid = true;

                if (shopManager != null)
                    shopManager.UpdateMoneyUI();
                break;
             case "hanhaihuoc":
                // Xóa hết item sở hữu và trang bị
                if (player != null && player.inventory != null)
                {
                    player.inventory.ownedItems.Clear();
                    player.inventory.equippedItems.Clear();
                    player.inventory.SaveInventory(); // Lưu lại trạng thái mới
                    player.ApplyEquipmentStats(true); // Reset chỉ số, hồi đầy máu/mana/stamina

                    txtResult.text = "Bạn đã bị mất sạch toàn bộ item!";
                }
                else
                {
                    txtResult.text = "Không tìm thấy inventory của player!";
                }
                isValid = true;
                break;

            default:
                txtResult.text = "Invalid code!";
                break;
        }
    }
}
