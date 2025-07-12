using System.Collections.Generic;

public static class GameDefeatData
{
    public static string lastDeathReason = "";
    public static string lastTopDamageEnemy = "";
    public static Dictionary<string, float> damageFromEachEnemy = new Dictionary<string, float>();

    public static void LogEnemyDamage(string enemyName, float damage)
    {
        if (damageFromEachEnemy.ContainsKey(enemyName))
            damageFromEachEnemy[enemyName] += damage;
        else
            damageFromEachEnemy[enemyName] = damage;
    }

    public static void Reset()
    {
        lastDeathReason = "";
        lastTopDamageEnemy = "";
        damageFromEachEnemy.Clear();
    }
}
