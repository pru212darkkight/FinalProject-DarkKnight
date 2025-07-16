using UnityEngine;

public class StormDamage : MonoBehaviour
{
    public float damage = 15f;
    public LayerMask playerLayer;

    void Start()
    {
        Destroy(gameObject, 2f); // Hủy sau 2 giây
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra có phải player không và nằm trong layer cho phép
        if (((1 << collision.gameObject.layer) & playerLayer) != 0 && collision.CompareTag("Player"))
        {
            PlayerController1 player = collision.GetComponent<PlayerController1>();
            if (player != null)
            {
                player.TakeDamage(damage, true, "Death");
                Debug.Log("⚡ Storm triggered damage to player!");
            }
        }
    }
}
