using UnityEngine;
using System.Collections;

public class HealthPotion : MonoBehaviour
{
    [Header("Health Potion Settings")]
    public float healAmount = 50f;
    public bool destroyAfterUse = true;
    public bool canUseMultipleTimes = false;
    
    [Header("Visual Effects")]
    public bool enableGlow = true;
    public Color glowColor = Color.green;
    public float glowSpeed = 2f;
    public bool enableFloating = true;
    public float floatHeight = 0.5f;
    public float floatSpeed = 1f;
    
    [Header("Audio")]
    public AudioClip pickupSound;
    public float soundVolume = 1f;
    
    [Header("Particle Effects")]
    public GameObject healEffect;
    public bool showHealNumbers = true;
    
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Vector3 startPosition;
    private bool hasBeenUsed = false;
    private AudioSource audioSource;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        startPosition = transform.position;
        
        // Setup AudioSource if not exists
        if (audioSource == null && pickupSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = pickupSound;
            audioSource.volume = soundVolume;
            audioSource.playOnAwake = false;
        }
        
        Debug.Log($"HealthPotion initialized: {healAmount} HP heal");
    }
    
    void Update()
    {
        if (hasBeenUsed && !canUseMultipleTimes) return;
        
        // Floating animation
        if (enableFloating)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
        
        // Glow effect
        if (enableGlow && spriteRenderer != null)
        {
            float glow = Mathf.Sin(Time.time * glowSpeed) * 0.3f + 0.7f;
            Color newColor = Color.Lerp(originalColor, glowColor, glow);
            spriteRenderer.color = newColor;
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UsePotion(other.gameObject);
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            UsePotion(collision.gameObject);
        }
    }
    
    void UsePotion(GameObject player)
    {
        // Check if already used and can't use multiple times
        if (hasBeenUsed && !canUseMultipleTimes)
        {
            return;
        }
        
        PlayerController1 playerController = player.GetComponent<PlayerController1>();
        if (playerController == null)
        {
            Debug.LogWarning("HealthPotion: Player doesn't have PlayerController1 component!");
            return;
        }
        
        // Check if player needs healing
        if (playerController.currentHealth >= playerController.maxHealth)
        {
            Debug.Log("HealthPotion: Player already at full health!");
            return;
        }
        
        // Calculate actual heal amount
        float currentHealth = playerController.currentHealth;
        float maxHealth = playerController.maxHealth;
        float actualHeal = Mathf.Min(healAmount, maxHealth - currentHealth);
        
        // Heal player
        playerController.currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        
        Debug.Log($"HealthPotion: Healed player for {actualHeal} HP. Health: {playerController.currentHealth}/{maxHealth}");
        
        // Play sound effect
        if (audioSource != null && pickupSound != null)
        {
            audioSource.Play();
        }
        
        // Show heal effect
        if (healEffect != null)
        {
            GameObject effect = Instantiate(healEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
        
        // Show heal numbers
        if (showHealNumbers)
        {
            ShowHealNumbers(actualHeal);
        }
        
        // Flash player green
        StartCoroutine(FlashPlayerGreen(playerController));
        
        hasBeenUsed = true;
        
        // Destroy or hide potion
        if (destroyAfterUse)
        {
            if (audioSource != null && pickupSound != null)
            {
                // Wait for sound to finish before destroying
                StartCoroutine(DestroyAfterSound());
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            // Hide temporarily
            StartCoroutine(HideTemporarily());
        }
    }
    
    IEnumerator FlashPlayerGreen(PlayerController1 player)
    {
        SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
        if (playerSprite != null)
        {
            Color originalColor = playerSprite.color;
            playerSprite.color = Color.green;
            yield return new WaitForSeconds(0.2f);
            playerSprite.color = originalColor;
        }
    }
    
    IEnumerator DestroyAfterSound()
    {
        // Hide visually but keep playing sound
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
        
        // Wait for sound to finish
        if (audioSource != null && pickupSound != null)
        {
            yield return new WaitForSeconds(pickupSound.length);
        }
        
        Destroy(gameObject);
    }
    
    IEnumerator HideTemporarily()
    {
        // Hide for 5 seconds then reappear
        gameObject.SetActive(false);
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(true);
        hasBeenUsed = false;
    }
    
    void ShowHealNumbers(float healAmount)
    {
        // Create floating text showing heal amount
        GameObject textObj = new GameObject("HealText");
        textObj.transform.position = transform.position + Vector3.up;
        
        TextMesh textMesh = textObj.AddComponent<TextMesh>();
        textMesh.text = $"+{healAmount:F0}";
        textMesh.color = Color.green;
        textMesh.fontSize = 20;
        textMesh.anchor = TextAnchor.MiddleCenter;
        
        // Animate text
        StartCoroutine(AnimateHealText(textObj));
    }
    
    IEnumerator AnimateHealText(GameObject textObj)
    {
        Vector3 startPos = textObj.transform.position;
        Vector3 endPos = startPos + Vector3.up * 2f;
        
        float duration = 1.5f;
        float timer = 0f;
        
        TextMesh textMesh = textObj.GetComponent<TextMesh>();
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            
            // Move up
            textObj.transform.position = Vector3.Lerp(startPos, endPos, progress);
            
            // Fade out
            Color color = textMesh.color;
            color.a = 1f - progress;
            textMesh.color = color;
            
            yield return null;
        }
        
        Destroy(textObj);
    }
    
    // Debug methods
    [ContextMenu("Test Heal")]
    public void TestHeal()
    {
        PlayerController1 player = FindObjectOfType<PlayerController1>();
        if (player != null)
        {
            UsePotion(player.gameObject);
        }
        else
        {
            Debug.LogError("No PlayerController1 found in scene!");
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw heal range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
