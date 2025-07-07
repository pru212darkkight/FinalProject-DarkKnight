using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [Header("UI References")]
    public Image healthBar;
    public Image staminaBar;
    public Image manaBar;
    
    [Header("UI Text (Optional)")]
    public Text healthText;
    public Text staminaText;
    public Text manaText;
    
    [Header("Player Reference")]
    public PlayerController1 player;
    
    [Header("UI Settings")]
    public bool showText = true;
    public bool autoFindPlayer = true;
    
    void Start()
    {
        // Tự động tìm player nếu chưa assign
        if (autoFindPlayer && player == null)
        {
            player = FindObjectOfType<PlayerController1>();
        }
        
        // Assign UI bars to player if found
        if (player != null)
        {
            player.healthBar = healthBar;
            player.staminaBar = staminaBar;
            player.manaBar = manaBar;
            
            Debug.Log("Player UI Manager: UI bars assigned to player successfully!");
        }
        else
        {
            Debug.LogWarning("Player UI Manager: Player not found!");
        }
    }
    
    void Update()
    {
        if (player == null) return;
        
        // Update UI bars (backup in case player doesn't update them)
        UpdateUIBars();
        
        // Update text if enabled
        if (showText)
        {
            UpdateUIText();
        }
    }
    
    private void UpdateUIBars()
    {
        if (healthBar != null)
            healthBar.fillAmount = player.currentHealth / player.maxHealth;
            
        if (staminaBar != null)
            staminaBar.fillAmount = player.stamina / player.maxStamina;
            
        if (manaBar != null)
            manaBar.fillAmount = player.mana / player.maxMana;
    }
    
    private void UpdateUIText()
    {
        if (healthText != null)
            healthText.text = $"{Mathf.Ceil(player.currentHealth)}/{player.maxHealth}";
            
        if (staminaText != null)
            staminaText.text = $"{Mathf.Ceil(player.stamina)}/{player.maxStamina}";
            
        if (manaText != null)
            manaText.text = $"{Mathf.Ceil(player.mana)}/{player.maxMana}";
    }
    
    // Public methods to manually assign UI elements
    public void SetHealthBar(Image healthBarImage)
    {
        healthBar = healthBarImage;
        if (player != null)
            player.healthBar = healthBar;
    }
    
    public void SetStaminaBar(Image staminaBarImage)
    {
        staminaBar = staminaBarImage;
        if (player != null)
            player.staminaBar = staminaBar;
    }
    
    public void SetManaBar(Image manaBarImage)
    {
        manaBar = manaBarImage;
        if (player != null)
            player.manaBar = manaBar;
    }
    
    // Method to refresh player reference
    public void RefreshPlayerReference()
    {
        player = FindObjectOfType<PlayerController1>();
        if (player != null)
        {
            player.healthBar = healthBar;
            player.staminaBar = staminaBar;
            player.manaBar = manaBar;
        }
    }
}
