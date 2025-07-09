using UnityEngine;

public class BossRoomTrigger : MonoBehaviour
{
    public BossHealthBarUI bossHealthBarUI;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bossHealthBarUI.ShowBar();
            // Có thể play nhạc boss, khóa cửa phòng,... tại đây nếu muốn
        }
    }
}
