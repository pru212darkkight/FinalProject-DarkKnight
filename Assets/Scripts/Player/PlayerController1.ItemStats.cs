using UnityEngine;

public partial class PlayerController1 : MonoBehaviour
{
    public Inventory inventory; // Gán qua Inspector hoặc tìm bằng code

    public int baseAttack = 10;
    public int baseDefense = 5;
    public int baseHealth = 100;
    public int baseMana = 50;

    public int totalAttack;
    public int totalDefense;
    public int totalHealth;
    public int totalMana;

    public void UpdateStatsFromEquipment()
    {
        totalAttack = baseAttack;
        totalDefense = baseDefense;
        totalHealth = baseHealth;
        totalMana = baseMana;

        if (inventory != null)
        {
            foreach (var item in inventory.equippedItems)
            {
                totalAttack += item.attackBonus;
                totalDefense += item.defenseBonus;
                totalHealth += item.healthBonus;
                totalMana += item.manaBonus;
            }
        }
    }
} 