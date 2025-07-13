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
                player.maxHealth = 10000;
                player.currentHealth = 10000;
                player.strength = 100;
                player.UpdateUI();
                txtResult.text = "You have received 10000 health and 100 strength!";
                isValid = true;
                break;

            case "tintientai":
                playerMoney.AddCoins(9999);
                txtResult.text = "You have received 9999 gold!";
                isValid = true;
                break;

            case "tuyentutung":
                player.maxHealth = 100;
                player.currentHealth = 100;
                playerMoney.coins = 0;
                player.UpdateUI();
                playerMoney.UpdateUI();
                txtResult.text = "Reset health to 100 and lost all gold!";
                isValid = true;
                break;

            default:
                txtResult.text = "Invalid code!";
                break;
        }
    }
}
