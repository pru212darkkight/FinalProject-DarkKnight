using UnityEngine;

public class HealthManaRestore : MonoBehaviour
{
    [Header("Restore Settings")]
    [SerializeField] private bool restoreHealth = true;
    [SerializeField] private bool restoreMana = false;
    [SerializeField] private float healthRestorePercent = 50f; // 50% máu
    [SerializeField] private float manaRestorePercent = 50f;   // 50% mana
    
    [Header("Effects")]
    [SerializeField] private GameObject restoreEffect;
    [SerializeField] private AudioClip restoreSound;
    [SerializeField] private bool destroyAfterUse = true;
    
    [Header("Cooldown")]
    [SerializeField] private bool useCooldown = false;
    [SerializeField] private float cooldownTime = 5f;
    
    private bool canUse = true;
    private AudioSource audioSource;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && restoreSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canUse) return;
        
        if (other.CompareTag("Player"))
        {
            PlayerController1 player = other.GetComponent<PlayerController1>();
            if (player != null)
            {
                RestorePlayer(player);
            }
        }
    }
    
    private void RestorePlayer(PlayerController1 player)
    {
        bool restored = false;
        
        // Hồi phục máu
        if (restoreHealth)
        {
            float healthToRestore = player.maxHealth * (healthRestorePercent / 100f);
            if (player.currentHealth < player.maxHealth)
            {
                player.currentHealth = Mathf.Min(player.currentHealth + healthToRestore, player.maxHealth);
                restored = true;
                Debug.Log($"Đã hồi phục {healthToRestore} máu cho player");
                //âm thanh 
                if (AudioManager.Instance != null && AudioManager.Instance.gainHealth != null)
                {
                    AudioManager.Instance.PlayRandomSFX(AudioManager.Instance.gainHealth);
                }
            }
        }
        
        // Hồi phục mana
        if (restoreMana)
        {
            float manaToRestore = player.maxMana * (manaRestorePercent / 100f);
            if (player.mana < player.maxMana)
            {
                player.mana = Mathf.Min(player.mana + manaToRestore, player.maxMana);
                restored = true;
                Debug.Log($"Đã hồi phục {manaToRestore} mana cho player");
            }
        }
        
        if (restored)
        {
            // Phát hiệu ứng
            if (restoreEffect != null)
            {
                Instantiate(restoreEffect, transform.position, Quaternion.identity);
            }
            
            // Phát âm thanh
            if (audioSource != null && restoreSound != null)
            {
                audioSource.PlayOneShot(restoreSound);
            }
            
            // Xử lý cooldown
            if (useCooldown)
            {
                StartCoroutine(CooldownRoutine());
            }
            else if (destroyAfterUse)
            {
                Destroy(gameObject);
            }
        }
    }
    
    private System.Collections.IEnumerator CooldownRoutine()
    {
        canUse = false;
        yield return new WaitForSeconds(cooldownTime);
        canUse = true;
    }
} 