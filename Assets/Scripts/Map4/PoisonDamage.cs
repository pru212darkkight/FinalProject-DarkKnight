using UnityEngine;

public class PoisonDamage : MonoBehaviour
{
    public float damage = 10f;
    public float slowDuration = 2f;
    public float slowAmount = 0.5f;
    public Color poisonColor = Color.green;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController1 player = other.GetComponent<PlayerController1>();
            if (player != null)
            {
                player.TakeDamage(damage, true, "Medusa");            
                player.ApplyPoisonEffect(slowDuration, slowAmount, poisonColor); // Làm chậm + đổi màu

                Debug.Log("☠️ Player trúng độc!");
            }

            Destroy(gameObject); // Tự hủy đạn/đám mây độc
        }
    }
}
