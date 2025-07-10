using UnityEngine;
using System.Collections;

public class SimpleHealthItem : MonoBehaviour
{
    [Header("Heal Settings")]
    public float healAmount = 50f;
    public bool destroyAfterUse = true;
    
    [Header("Visual Effects")]
    public bool showFlashEffect = true;
    public Color flashColor = Color.green;
    public float flashDuration = 0.3f;
    
    [Header("Audio")]
    public AudioClip healSound;
    
    private bool hasBeenUsed = false;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasBeenUsed) return;
        
        if (other.CompareTag("Player"))
        {
            PlayerController1 player = other.GetComponent<PlayerController1>();
            if (player != null)
            {
                HealPlayer(player);
            }
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasBeenUsed) return;
        
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController1 player = collision.gameObject.GetComponent<PlayerController1>();
            if (player != null)
            {
                HealPlayer(player);
            }
        }
    }
    
    void HealPlayer(PlayerController1 player)
    {
        // Check if player needs healing
        if (player.currentHealth >= player.maxHealth)
        {
            Debug.Log("Player already at full health!");
            return;
        }
        
        // Calculate actual heal amount
        float currentHealth = player.currentHealth;
        float maxHealth = player.maxHealth;
        float actualHeal = Mathf.Min(healAmount, maxHealth - currentHealth);
        
        // Heal player
        player.currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        
        Debug.Log($"Healed player for {actualHeal} HP. Health: {player.currentHealth}/{maxHealth}");
        
        // Play sound
        if (healSound != null)
        {
            AudioSource.PlayClipAtPoint(healSound, transform.position);
        }
        
        // Flash effect
        if (showFlashEffect)
        {
            StartCoroutine(FlashPlayer(player));
        }
        
        // Show floating text
        ShowHealText(actualHeal);
        
        hasBeenUsed = true;
        
        // Destroy item
        if (destroyAfterUse)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    
    IEnumerator FlashPlayer(PlayerController1 player)
    {
        SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
        if (playerSprite != null)
        {
            Color originalColor = playerSprite.color;
            playerSprite.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            playerSprite.color = originalColor;
        }
    }
    
    void ShowHealText(float healAmount)
    {
        // Create simple floating text
        GameObject textObj = new GameObject("HealText");
        textObj.transform.position = transform.position + Vector3.up;
        
        TextMesh textMesh = textObj.AddComponent<TextMesh>();
        textMesh.text = $"+{healAmount:F0} HP";
        textMesh.color = Color.green;
        textMesh.fontSize = 16;
        textMesh.anchor = TextAnchor.MiddleCenter;
        
        // Simple animation
        StartCoroutine(AnimateText(textObj));
    }
    
    IEnumerator AnimateText(GameObject textObj)
    {
        Vector3 startPos = textObj.transform.position;
        float timer = 0f;
        float duration = 1f;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            
            // Move up and fade
            textObj.transform.position = startPos + Vector3.up * progress * 2f;
            
            TextMesh textMesh = textObj.GetComponent<TextMesh>();
            Color color = textMesh.color;
            color.a = 1f - progress;
            textMesh.color = color;
            
            yield return null;
        }
        
        Destroy(textObj);
    }
}
