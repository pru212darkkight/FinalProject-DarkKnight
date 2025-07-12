[System.Serializable]
public class DamageLog
{
    public float physical = 0;
    public float magic = 0;
    public float total => physical + magic;
}
