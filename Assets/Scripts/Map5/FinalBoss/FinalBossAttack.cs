using UnityEngine;

public class FinalBossAttack : MonoBehaviour
{
    private Animator animator;
    private FinalBossController controller;

    [Header("Chiêu 1: Thánh Giá (Holy Cross)")]
    public GameObject crossPrefab;
    public float crossHeight = 3.5f;
    public float crossDamage = 30f;
    public float crossRange = 0.8f;
    public float crossDelay = 0.2f;

    [Header("Chiêu 2: Mặt Trăng (Moon Strike)")]
    public GameObject moonPrefab;
    public float moonHeight = 3.5f;
    public float moonDamage = 40f;
    public int moonCount = 3;
    public float moonSpacing = 1.5f;
    public float moonDelay = 0.2f;

    [Header("Chiêu 3: Chưởng Đầu Lâu (Skull Blast)")]
    public GameObject skullBlastPrefab;
    public Transform skullBlastSpawnPoint;   // Điểm spawn cố định (gắn transform trong scene)
    public float skullBlastDamage = 50f;
    public float skullBlastSpeed = 8f;

    public LayerMask playerLayer;

    void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<FinalBossController>();
    }

    /// <summary>
    /// Gọi từ Controller. 1 = thánh giá, 2 = mặt trăng, 3 = chưởng đầu lâu.
    /// </summary>
    public void DoAttack(int type)
    {
        switch (type)
        {
            case 1:
                // 🎵 Play Holy Cross attack sound
                if (AudioManager.Instance != null && AudioManager.Instance.finalBossAttack1 != null)
                {
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.finalBossAttack1);
                }
                animator.SetTrigger("Attack1");
                break;
            case 2:
                // 🎵 Play Moon Strike attack sound
                if (AudioManager.Instance != null && AudioManager.Instance.finalBossAttack2 != null)
                {
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.finalBossAttack2);
                }
                animator.SetTrigger("Attack2");
                break;
            case 3:
                // 🎵 Play Skull Blast attack sound
                if (AudioManager.Instance != null && AudioManager.Instance.finalBossAttack3 != null)
                {
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.finalBossAttack3);
                }
                animator.SetTrigger("Attack3");
                break;
        }
    }

    // RESET trigger sau khi animation chạy xong (gọi trong cuối mỗi Animation bằng Event)
    public void ResetAttackTrigger(int attackType)
    {
        switch (attackType)
        {
            case 1:
                animator.ResetTrigger("Attack1");
                break;
            case 2:
                animator.ResetTrigger("Attack2");
                break;
            case 3:
                animator.ResetTrigger("Attack3");
                break;
        }
    }

    // ===== CHIÊU 1: THÁNH GIÁ =====
    // Gọi từ Animation Event!
    public void CastHolyCross()
    {
        Vector3 playerPos = controller.player.position;
        Vector3 spawnPos = new Vector3(playerPos.x, transform.position.y + crossHeight, 0);

        GameObject cross = Instantiate(crossPrefab, spawnPos, Quaternion.identity);
        ThunderHitbox hitbox = cross.GetComponent<ThunderHitbox>();
        if (hitbox != null)
            hitbox.Init(crossDamage, playerLayer, true, "Final Boss Demon King");
    }

    // ===== CHIÊU 2: MẶT TRĂNG =====
    // Gọi từ Animation Event!
    public void CastMoonStrike()
    {
        Vector3 playerPos = controller.player.position;
        float baseY = transform.position.y + moonHeight;
        float centerX = playerPos.x;

        for (int i = 0; i < moonCount; i++)
        {
            float offset = (i - (moonCount - 1) / 2f) * moonSpacing;
            Vector3 spawnPos = new Vector3(centerX + offset, baseY, 0);

            GameObject moon = Instantiate(moonPrefab, spawnPos, Quaternion.identity);
            ThunderHitbox hitbox = moon.GetComponent<ThunderHitbox>();
            if (hitbox != null)
                hitbox.Init(moonDamage, playerLayer, true, "Final Boss Demon King");
        }
    }

    // ===== CHIÊU 3: CHƯỞNG ĐẦU LÂU =====
    // Gọi từ Animation Event!
    public void CastSkullBlast()
    {
        if (skullBlastSpawnPoint == null) return;
        Vector3 spawn = skullBlastSpawnPoint.position;
        Vector3 target = controller.player.position;
        Vector3 dir = (target - spawn).normalized;

        GameObject skull = Instantiate(skullBlastPrefab, spawn, Quaternion.identity);
        SkullBlastProjectile proj = skull.GetComponent<SkullBlastProjectile>();
        if (proj != null)
            proj.Init(dir, skullBlastSpeed, skullBlastDamage, playerLayer);
    }

    // --- VẼ GIZMOS ---
    void OnDrawGizmosSelected()
    {
        if (Camera.main == null) return;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        if (screenPos.z > 0 &&
            screenPos.x >= 0 && screenPos.x <= Screen.width &&
            screenPos.y >= 0 && screenPos.y <= Screen.height)
        {
            // Vùng Thánh Giá
            Gizmos.color = Color.yellow;
            float px = Application.isPlaying && controller ? controller.player.position.x : transform.position.x;
            Gizmos.DrawWireSphere(
                new Vector3(px, transform.position.y + crossHeight, 0),
                crossRange);

            // Vùng Mặt Trăng
            if (moonCount > 1)
            {
                Gizmos.color = Color.magenta;
                float y = transform.position.y + moonHeight;
                for (int i = 0; i < moonCount; i++)
                {
                    float offset = (i - (moonCount - 1) / 2f) * moonSpacing;
                    Gizmos.DrawWireSphere(new Vector3(px + offset, y, 0), crossRange);
                }
            }
            // Điểm spawn chưởng đầu lâu
            if (skullBlastSpawnPoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(skullBlastSpawnPoint.position, 0.2f);
            }
        }
    }
}
