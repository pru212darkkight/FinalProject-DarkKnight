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
        // 1. Lấy chỉ số player hiện tại
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

        // 2. Danh sách trang bị đang mặc (tên)
        List<string> equippedNames = inventory.equippedItems.Select(i => i.itemName).ToList();

        // 3. Danh sách item player sở hữu (tên, để check item shop trùng)
        List<string> allItems = inventory.ownedItems.Select(i => i.itemName).ToList();
        var ownedItemNames = new HashSet<string>(allItems);

        // 4. Danh sách shop: chỉ gửi item CHƯA SỞ HỮU và GIỚI HẠN số lượng (VD: 6-8 item)
        List<ItemLogData> shopLog = shopItems
            .Where(item => !ownedItemNames.Contains(item.itemName))
            .OrderByDescending(i => i.price)  // Có thể ưu tiên item giá cao (hoặc đổi .OrderBy(...) tuỳ bạn)
            .Take(8) // Lấy tối đa 8 item
            .Select(item => new ItemLogData
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

        // 5. Tạo log trận thua (toàn bộ info trận thua lấy từ defeatLog)
        var log = new PlayerLog
        {
            timeSurvived = defeatLog.timeSurvived,
            topDamageEnemy = defeatLog.topDamageEnemy,
            deathReason = defeatLog.deathReason,
            playerEquipment = equippedNames,
            playerStats = stats,
            damageTaken = new Dictionary<string, DamageLog>(defeatLog.damageFromEachEnemy)
        };

        // 6. Gom lại thành dữ liệu hoàn chỉnh
        return new GeminiRequestData
        {
            player_log = log,
            player_items = allItems,
            player_gold = playerMoney.coins, // Tổng tiền hiện tại
            shop_items = shopLog
        };
    }
}
