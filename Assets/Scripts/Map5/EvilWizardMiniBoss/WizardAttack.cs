using System.Collections.Generic;
using UnityEngine;

public class WizardAttack : MonoBehaviour
{
    [Header("Damage & Summon")]
    public float attack1Damage = 10f;
    public float damageInterval = 0.5f;
    public LayerMask playerMask;
    public WizardController wizardController; // Tham chiếu Controller để lấy vùng AOE

    public List<GameObject> summonPrefabs;
    public float summonInterval = 10f;
    public Transform[] summonSpawnPoints;

    [HideInInspector] public Transform playerRef;

    // --- Private ---
    private float summonTimer = 0f;
    private bool canSummon = false;
    private bool aoeActive = false;
    private readonly Dictionary<Collider2D, float> playerLastHit = new Dictionary<Collider2D, float>();

    // --- Summon control từ controller ---
    public void AllowSummon(bool active)
    {
        canSummon = active;
        if (!active)
            summonTimer = 0f;
    }

    // --- Kích hoạt/Ngắt AOE từ animation event ---
    public void StartAOE() => aoeActive = true;
    public void EndAOE()
    {
        aoeActive = false;
        playerLastHit.Clear();
    }

    void Update()
    {
        HandleAOEDamage();
        HandleSummon();
    }

    // --- AOE Damage ---
    void HandleAOEDamage()
    {
        if (!aoeActive || wizardController == null) return;

        float facing = Mathf.Sign(transform.localScale.x);
        Vector2 boxCenter = (Vector2)transform.position +
                            new Vector2(wizardController.aoeZoneOffset.x * facing, wizardController.aoeZoneOffset.y);
        Vector2 boxSize = wizardController.aoeZoneSize;

        Collider2D[] players = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0, playerMask);

        foreach (var col in players)
        {
            if (!col.CompareTag("Player")) continue;
            playerLastHit.TryGetValue(col, out float lastTime);
            if (Time.time - lastTime >= damageInterval)
            {
                var player = col.GetComponent<PlayerController1>();
                if (player != null)
                    player.TakeDamage(attack1Damage, false);
                playerLastHit[col] = Time.time;
            }
        }
        // Remove player đã ra khỏi vùng
        var toRemove = new List<Collider2D>();
        foreach (var pair in playerLastHit)
        {
            bool inside = false;
            foreach (var p in players)
                if (p == pair.Key) { inside = true; break; }
            if (!inside) toRemove.Add(pair.Key);
        }
        foreach (var k in toRemove) playerLastHit.Remove(k);
    }

    // --- Triệu hồi quái ---
    void HandleSummon()
    {
        if (!canSummon) return;
        summonTimer += Time.deltaTime;
        if (summonTimer < summonInterval) return;
        SummonMonster();
        summonTimer = 0f;
    }

    void SummonMonster()
    {
        if (summonPrefabs.Count == 0 || summonSpawnPoints.Length == 0) return;

        GameObject prefab = summonPrefabs[Random.Range(0, summonPrefabs.Count)];
        Transform spawnPoint = summonSpawnPoints[Random.Range(0, summonSpawnPoints.Length)];
        GameObject obj = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        // Gán player cho các script trên quái
        if (playerRef == null) return;
        var components = obj.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (var comp in components)
        {
            var type = comp.GetType();
            var fieldUpper = type.GetField("Player");
            var fieldLower = type.GetField("player");
            if (fieldUpper != null && fieldUpper.FieldType == typeof(Transform))
                fieldUpper.SetValue(comp, playerRef);
            if (fieldLower != null && fieldLower.FieldType == typeof(Transform))
                fieldLower.SetValue(comp, playerRef);
        }
    }
}
