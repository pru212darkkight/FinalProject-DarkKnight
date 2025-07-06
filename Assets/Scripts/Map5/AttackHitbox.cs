using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    private BoxCollider2D col;

    [Header("Sát thương mỗi đòn đánh")]
    public float damage = 10f;       
    [Header("Đòn này là phép?")]
    public bool isMagicDamage = false;

    void Awake()
    {
        col = GetComponent<BoxCollider2D>();
    }

    public void Enable() { col.enabled = true; }
    public void Disable() { col.enabled = false; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController1>();
            if (player != null)
            {
                player.TakeDamage(damage, isMagicDamage);
            }
        }
    }
}
