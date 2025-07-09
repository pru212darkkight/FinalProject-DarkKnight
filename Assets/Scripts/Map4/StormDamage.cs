using UnityEngine;

public class StormDamage : MonoBehaviour
{
    public float damage = 15f;
    public float radius = 1.5f;
    public LayerMask playerLayer;

    // Hàm này sẽ được gọi đúng lúc animation tới giữa (qua Animation Event)
    void Start()
    {
        Destroy(gameObject, 2f); // Tự hủy sau 2 giây
    }
    public void DealDamage()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, radius, playerLayer);
        if (hit != null && hit.CompareTag("Player"))
        {
            PlayerController1 player = hit.GetComponent<PlayerController1>();
            if (player != null)
            {
                player.TakeDamage(damage, true);
                Debug.Log("⚡ Storm hit player!");

            }
        }
    }
}
