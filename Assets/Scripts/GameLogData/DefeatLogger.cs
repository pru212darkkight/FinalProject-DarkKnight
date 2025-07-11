using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameLogData;

public class DefeatLogger : MonoBehaviour
{
    public PlayerController1 player;
    public PlayerMoney playerMoney;
    public LevelTimer levelTimer;
    public Inventory inventory;
    public List<ItemData> shopItems; // List<ItemData> từ shop

    // ===== GỌI HÀM NÀY KHI MUỐN TẠO LOG GỬI LÊN AI =====
    public GeminiRequestData BuildGeminiRequest(LastDefeatLog defeatLog)
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

        List<string> equippedNames = inventory.equippedItems.Select(i => i.itemName).ToList();

        // Map log trận thua - dùng hoàn toàn data từ defeatLog!
        var log = new PlayerLog
        {
            timeSurvived = defeatLog.timeSurvived,
            topDamageEnemy = defeatLog.topDamageEnemy,
            deathReason = defeatLog.deathReason,
            playerEquipment = equippedNames,
            playerStats = stats,
            damageTaken = new Dictionary<string, DamageLog>(defeatLog.damageFromEachEnemy) // LẤY TỪ defeatLog
        };

        List<string> allItems = inventory.ownedItems.Select(i => i.itemName).ToList();

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
            player_gold = playerMoney.coins, // Tổng tiền đang có
            shop_items = shopLog
        };
    }
}
