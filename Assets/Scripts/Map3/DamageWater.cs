using UnityEngine;
using System.Collections;

public class DamageWater : MonoBehaviour
{
    [Header("Water Damage Settings")]
    public bool enableWaterDamage = true;
    public float damageInterval = 1f; // Mỗi 1 giây
    public float healthLossPercent = 1f; // 1% mỗi lần

    [Header("Effects")]
    public bool showFlash = true;
    public Color flashColor = new Color(0.3f, 0.7f, 1f, 0.5f);
    public float flashTime = 0.3f;

    private PlayerController1 player;
    private SpriteRenderer playerSprite;
    private Color originalColor;
    private Coroutine damageCoroutine;

    void Start()
    {
        // Chờ 1 frame để player khởi tạo xong
        StartCoroutine(InitializeAfterDelay());
    }

    private IEnumerator InitializeAfterDelay()
    {
        // Chờ 0.1 giây để player setup xong
        yield return new WaitForSeconds(0.1f);

        player = FindAnyObjectByType<PlayerController1>();

        if (player != null)
        {
            playerSprite = player.GetComponent<SpriteRenderer>();
            if (playerSprite != null)
            {
                originalColor = playerSprite.color;
            }

            Debug.Log($"DamageWater: Player found! Health: {player.currentHealth}/{player.maxHealth}");

            // Đảm bảo player có máu trước khi bắt đầu damage
            if (player.currentHealth <= 0)
            {
                Debug.LogWarning("DamageWater: Player health is 0, setting to max health");
                player.currentHealth = player.maxHealth;
                UpdatePlayerUI();
            }

            if (enableWaterDamage)
            {
                StartWaterDamage();
            }
        }
        else
        {
            Debug.LogError("DamageWater: Player not found!");
        }
    }

    public void StartWaterDamage()
    {
        if (player == null) return;

        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }

        damageCoroutine = StartCoroutine(WaterDamageLoop());
        Debug.Log($"DamageWater: Started! {healthLossPercent}% HP every {damageInterval}s");
    }

    public void StopWaterDamage()
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
            Debug.Log("DamageWater: Stopped!");
        }
    }

    private IEnumerator WaterDamageLoop()
    {
        Debug.Log("DamageWater: WaterDamageLoop started!");
        Debug.Log($"Initial values - enableWaterDamage: {enableWaterDamage}, player: {player != null}, currentHealth: {player?.currentHealth}");

        while (enableWaterDamage && player != null)
        {
            Debug.Log($"DamageWater: Waiting {damageInterval} seconds...");
            yield return new WaitForSeconds(damageInterval);

            // Kiểm tra player còn sống bằng cách check currentHealth
            if (player.currentHealth <= 0)
            {
                Debug.Log("DamageWater: Player health is 0, breaking loop");
                break;
            }

            // Tính damage
            float damage = (player.maxHealth * healthLossPercent) / 100f;
            // Lưu health trước khi damage
            float healthBefore = player.currentHealth;

            // Trừ máu trực tiếp (vì TakeDamage có thể bị block bởi defend)
            player.TakeDamage(damage, true, "Water Pressure per second");

            // QUAN TRỌNG: Update lastDamageTime để ngăn health regen
            System.Reflection.FieldInfo lastDamageTimeField = typeof(PlayerController1).GetField("lastDamageTime",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (lastDamageTimeField != null)
            {
                lastDamageTimeField.SetValue(player, Time.time);            }

            // Update UI bars manually
            UpdatePlayerUI();

            // Kiểm tra health sau damage
            float healthAfter = player.currentHealth;

            // Flash effect
            if (showFlash && playerSprite != null)
            {
                StartCoroutine(FlashEffect());
            }

            // Kiểm tra nếu hết máu
            if (player.currentHealth <= 0)
            {
                Debug.Log("DamageWater: Player drowned!");
                // Gọi Die() method của player
                player.TakeDamage(0.1f, true, "Water Pressure per second"); // Trigger death
                break;
            }
        }

        Debug.Log("DamageWater: WaterDamageLoop ended!");
        damageCoroutine = null;
    }

    private IEnumerator FlashEffect()
    {
        if (playerSprite == null) yield break;

        playerSprite.color = flashColor;
        yield return new WaitForSeconds(flashTime);
        playerSprite.color = originalColor;
    }

    private void UpdatePlayerUI()
    {
        // Tìm UI bars trực tiếp và update
        UnityEngine.UI.Image healthBar = null;
        UnityEngine.UI.Image manaBar = null;
        UnityEngine.UI.Image staminaBar = null;

        // Tìm các UI bars có thể có (từ FixedUICreator)
        string[] healthBarNames = {"Health_Fill", "HealthBar_Fill", "PlayerHealth_Fill"};
        string[] manaBarNames = {"Mana_Fill", "ManaBar_Fill", "PlayerMana_Fill"};
        string[] staminaBarNames = {"Stamina_Fill", "StaminaBar_Fill", "PlayerStamina_Fill"};

        // Tìm health bar
        foreach (string name in healthBarNames)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null)
            {
                healthBar = obj.GetComponent<UnityEngine.UI.Image>();
                if (healthBar != null)
                {
                    Debug.Log($"DamageWater: Found health bar: {name}");
                    break;
                }
            }
        }

        // Tìm mana bar
        foreach (string name in manaBarNames)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null)
            {
                manaBar = obj.GetComponent<UnityEngine.UI.Image>();
                if (manaBar != null) break;
            }
        }

        // Tìm stamina bar
        foreach (string name in staminaBarNames)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null)
            {
                staminaBar = obj.GetComponent<UnityEngine.UI.Image>();
                if (staminaBar != null) break;
            }
        }

        // Update UI bars
        if (healthBar != null)
        {
            healthBar.fillAmount = player.currentHealth / player.maxHealth;
            Debug.Log($"DamageWater: Health bar updated - {player.currentHealth:F1}/{player.maxHealth} = {healthBar.fillAmount:F2}");
        }
        else
        {
            Debug.LogWarning("DamageWater: Could not find health bar to update");
        }

        if (manaBar != null)
        {
            manaBar.fillAmount = player.mana / player.maxMana;
        }

        if (staminaBar != null)
        {
            staminaBar.fillAmount = player.stamina / player.maxStamina;
        }
    }

    // Test methods để debug
    [ContextMenu("Test Damage Once")]
    public void TestDamageOnce()
    {
        if (player != null)
        {
            float damage = (player.maxHealth * healthLossPercent) / 100f;
            float healthBefore = player.currentHealth;

            Debug.Log($"TEST: Before damage - Health: {healthBefore:F1}/{player.maxHealth}");

            // Trừ máu trực tiếp
            player.currentHealth -= damage;
            player.currentHealth = Mathf.Max(0, player.currentHealth);

            // Update lastDamageTime để ngăn health regen
            System.Reflection.FieldInfo lastDamageTimeField = typeof(PlayerController1).GetField("lastDamageTime",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (lastDamageTimeField != null)
            {
                lastDamageTimeField.SetValue(player, Time.time);
            }

            UpdatePlayerUI();

            Debug.Log($"TEST: After damage - Health: {player.currentHealth:F1}/{player.maxHealth}");
            Debug.Log($"TEST: Damage applied: {damage:F1}, Actual loss: {healthBefore - player.currentHealth:F1}");

            // Flash effect
            if (showFlash && playerSprite != null)
            {
                StartCoroutine(FlashEffect());
            }
        }
        else
        {
            Debug.LogError("TEST: Player is null!");
        }
    }

    [ContextMenu("Show Player Info")]
    public void ShowPlayerInfo()
    {
        if (player != null)
        {
            Debug.Log($"PLAYER INFO:");
            Debug.Log($"- Health: {player.currentHealth:F1}/{player.maxHealth}");
            Debug.Log($"- Position: {player.transform.position}");
            Debug.Log($"- GameObject: {player.gameObject.name}");
            Debug.Log($"- Active: {player.gameObject.activeInHierarchy}");
        }
        else
        {
            Debug.LogError("Player is null!");
        }
    }

    [ContextMenu("Show Script Status")]
    public void ShowScriptStatus()
    {
        Debug.Log($"SCRIPT STATUS:");
        Debug.Log($"- Enable Water Damage: {enableWaterDamage}");
        Debug.Log($"- Damage Interval: {damageInterval}s");
        Debug.Log($"- Health Loss Percent: {healthLossPercent}%");
        Debug.Log($"- Coroutine Running: {damageCoroutine != null}");
        Debug.Log($"- Player Found: {player != null}");
        Debug.Log($"- GameObject Active: {gameObject.activeInHierarchy}");
    }

    void OnDestroy()
    {
        StopWaterDamage();
    }
}