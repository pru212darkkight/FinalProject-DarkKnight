using UnityEngine;

public class SimpleHealthRestore : MonoBehaviour
{
    [Header("Health Restore")]
    [SerializeField] private float healthRestorePercent = 50f; // 50% máu
    
    [Header("Settings")]
    [SerializeField] private bool destroyAfterUse = true;
    [SerializeField] private AudioClip restoreSound;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController1 player = other.GetComponent<PlayerController1>();
            if (player != null && player.currentHealth < player.maxHealth)
            {
                // Hồi phục máu
                float healthToRestore = player.maxHealth * (healthRestorePercent / 100f);
                player.currentHealth = Mathf.Min(player.currentHealth + healthToRestore, player.maxHealth);
                
                Debug.Log($"Đã hồi phục {healthToRestore} máu cho player");
                
                // Phát âm thanh
                if (restoreSound != null)
                {
                    AudioSource.PlayClipAtPoint(restoreSound, transform.position);
                }
                
                // Xóa object sau khi sử dụng
                if (destroyAfterUse)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
} 