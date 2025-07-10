using TMPro;
using UnityEngine;

public class PlayerStatsPanel : MonoBehaviour
{
    public TMP_Text healthText, strengthText, staminaText;
    public TMP_Text speedText, manaText;
    public TMP_Text manaRegenText, magicResistText, armorText;
    public TMP_Text healthRegenText, staminaRegenText, jumpText;

    public void UpdateStats(PlayerController1 player)
    {
        healthText.text = "Health: " + player.maxHealth;
        strengthText.text = "Strength: " + player.strength;
        staminaText.text = "Stamina: " + player.stamina;
        speedText.text = "Speed: " + player.speed;
        manaText.text = "Mana: " + player.maxMana;
        manaRegenText.text = "Mana Regen: " + player.manaRegenRate;
        magicResistText.text = "Magic Resist: " + player.magicResist;
        armorText.text = "Armor: " + player.armor;
        healthRegenText.text = "Health Regen: " + player.healthRecoveryRate;
        staminaRegenText.text = "Stamina Regen: " + player.staminaRegenRate;
        jumpText.text = "Jump: " + player.jumpForce;

    }
}
