using UnityEngine;

public class SimpleManaRestore : MonoBehaviour
{
    [Header("Mana Restore")]
    [SerializeField] private float manaRestorePercent = 50f; // 50% mana
    
    [Header("Settings")]
    [SerializeField] private bool destroyAfterUse = true;
    [SerializeField] private AudioClip restoreSound;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController1 player = other.GetComponent<PlayerController1>();
            if (player != null && player.mana < player.maxMana)
            {
                // Hồi phục mana
                float manaToRestore = player.maxMana * (manaRestorePercent / 100f);
                player.mana = Mathf.Min(player.mana + manaToRestore, player.maxMana);
                
                Debug.Log($"Đã hồi phục {manaToRestore} mana cho player");
                
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