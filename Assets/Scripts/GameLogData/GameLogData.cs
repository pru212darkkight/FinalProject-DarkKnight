using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogData
{
    [Serializable]
    public class PlayerStats
    {
        public int maxHp; // maxHealth
        public int strenght; // strength
        public int maxStamina; // maxStamina
        public int maxMana; // maxMana
        public float speed; // moveSpeed
        public int armor; // armor 
        public float manaRegen; // manaRegenRate
        public int magicResist; // magicResist
        public int jump; // jumpForce
        public float healthRegen; // healthRecoveryRate
        public float staminaRegen; // staminaRegenRate
    }

    [System.Serializable]
    public class ItemLogData
    {
        public string name;    // itemName
        public string type;    // itemType.ToString()
        public int price;
        public Dictionary<string, float> stats; // Gộp tất cả các chỉ số vào stats
    }

    [Serializable]
    public class PlayerLog
    {
        public float timeSurvived;        // lấy từ LevelTimer
        public string topDamageEnemy;     // phải có cơ chế log enemy nào gây damage nhiều nhất
        public string deathReason;        // lý do chết, tuỳ bạn gán lúc Die()
        public List<string> playerEquipment; // list các item đang trang bị (inventory.equippedItems.Select(x=>x.itemName))
        public PlayerStats playerStats;       // như trên
    }

    [Serializable]
    public class GeminiRequestData
    {
        public PlayerLog player_log;
        public List<string> player_items;        // List tên các item player đang sở hữu (có thể lấy từ inventory)
        public int player_gold;                  // từ PlayerMoney.coins
        public List<ItemLogData> shop_items;     // List các item trong shop (giống ItemLogData)
    }
}
