using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameLogData;

public class DefeatLogger : MonoBehaviour
{
    // Kéo vào inspector hoặc tìm qua code
    public PlayerController1 player;
    public PlayerMoney playerMoney;
    public LevelTimer levelTimer;
    public Inventory inventory;
    public List<ItemData> shopItems; // List<ItemData> từ shop

    // ===== GỌI HÀM NÀY KHI MUỐN TẠO LOG GỬI LÊN AI =====
    public GeminiRequestData BuildGeminiRequest(string topDamageEnemy, string deathReason)
    {
        // Map stats player
        var stats = new PlayerStats
        {
            maxHp = (int)player.maxHealth,
            strenght = (int)player.strength,
            maxStamina = (int)player.maxStamina,
            maxMana = (int)player.maxMana,
            speed = player.speed,
            armor = (int)player.armor,
            manaRegen = player.manaRegenRate,
            magicResist = (int)player.magicResist,
            jump = (int)player.jumpForce,
            healthRegen = player.healthRecoveryRate,
            staminaRegen = player.staminaRegenRate
        };

        // Map trang bị đang mặc
        List<string> equippedNames = inventory.equippedItems.Select(i => i.itemName).ToList();

        // Map log trận thua
        var log = new PlayerLog
        {
            timeSurvived = levelTimer.elapsedTime,
            topDamageEnemy = topDamageEnemy,
            deathReason = deathReason,
            playerEquipment = equippedNames,
            playerStats = stats
        };

        // Map các item player đang sở hữu (toàn bộ, không chỉ trang bị)
        List<string> allItems = inventory.ownedItems.Select(i => i.itemName).ToList();

        // Map item shop
        List<ItemLogData> shopLog = shopItems.Select(item => new ItemLogData
        {
            name = item.itemName,
            type = item.itemType.ToString(),
            price = item.price,
            stats = new Dictionary<string, float>
            {
                { "healthBonus", item.healthBonus },
                { "staminaBonus", item.staminaBonus },
                { "manaBonus", item.manaBonus },
                { "strengthBonus", item.strengthBonus },
                { "armorBonus", item.armorBonus },
                { "magicResistBonus", item.magicResistBonus },
                { "healthRegenBonus", item.healthRegenBonus },
                { "staminaRegenBonus", item.staminaRegenBonus },
                { "manaRegenBonus", item.manaRegenBonus },
                { "moveSpeedBonus", item.moveSpeedBonus },
                { "jumpBonus", item.jumpBonus }
            }
        }).ToList();

        // Gom lại thành gói dữ liệu hoàn chỉnh
        return new GeminiRequestData
        {
            player_log = log,
            player_items = allItems,
            player_gold = playerMoney.coins,
            shop_items = shopLog
        };
    }
}
