using UnityEngine;

public class CloseRangeDamage : MonoBehaviour
{
    public float damage = 20f;
    public float radius = 1f;
    public LayerMask playerLayer;

    void Start()
    {
        Invoke("DealDamage", 0.3f); // Thời điểm gây damage, chỉnh lại cho khớp anim
        Destroy(gameObject, 2f); // Tự hủy sau 2s
    }

    void DealDamage()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, radius, playerLayer);
        if (hit != null && hit.CompareTag("Player"))
        {
            PlayerController1 player = hit.GetComponent<PlayerController1>();
            if (player != null)
            {
                player.TakeDamage(damage, true);
                Debug.Log("💥 Close-range hit!");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
