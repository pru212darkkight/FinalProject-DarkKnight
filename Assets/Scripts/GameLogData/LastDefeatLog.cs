using System.Collections.Generic;

public class LastDefeatLog
{
    public float timeSurvived;
    public int playerGold;
    public string topDamageEnemy;
    public string deathReason;
    public Dictionary<string, DamageLog> damageFromEachEnemy = new Dictionary<string, DamageLog>();
}
