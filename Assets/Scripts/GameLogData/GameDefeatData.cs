using System.Collections.Generic;

public static class GameDefeatData
{
    public static string lastDeathReason = "";
    public static string lastTopDamageEnemy = "";
    public static Dictionary<string, DamageLog> damageFromEachEnemy = new Dictionary<string, DamageLog>();

    public static void LogEnemyDamage(string enemyName, float damage, bool isMagic)
    {
        if (!damageFromEachEnemy.ContainsKey(enemyName))
            damageFromEachEnemy[enemyName] = new DamageLog();

        if (isMagic)
            damageFromEachEnemy[enemyName].magic += damage;
        else
            damageFromEachEnemy[enemyName].physical += damage;
    }

    public static void Reset()
    {
        lastDeathReason = "";
        lastTopDamageEnemy = "";
        damageFromEachEnemy.Clear();
    }
}
