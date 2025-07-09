using UnityEngine;
using System.Collections;

public class Dead4Attack : MonoBehaviour
{
    [Header("Triệu hồi")]
    public GameObject summonPrefab;
    public float summonDelay = 0.5f;

    [Header("Kỹ năng đặc biệt")]
    public GameObject stormPrefab;
    public float stormDelay = 0.3f;

    [Header("Thời gian hồi chiêu chung (Storm & Summon)")]
    public float skillCooldown = 10f;
    private float lastCastTime = -Mathf.Infinity;

    [Header("Cận chiến")]
    public float closeAttackRange = 1.5f;
    public GameObject meleeEffectPrefab;
    public float meleeDelay = 0.2f;
    public float meleeCooldown = 3f;
    private float lastMeleeTime = -Mathf.Infinity;

    [Header("Animator")]
    public Animator animator;
    public string summonTrigger = "Summon";

    [Header("Camera Shake")]
    public float shakeDuration = 0.2f;
    public float shakeIntensity = 0.2f;

    [Header("Target")]
    public Transform player;
    public float detectRange = 6f;

    private bool isCasting = false;
    private GameObject[] summonedEnemies = new GameObject[3];

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null || isCasting) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > detectRange) return;

        if (distance <= closeAttackRange && Time.time - lastMeleeTime >= meleeCooldown)
        {
            StartCoroutine(CloseAttackRoutine());
            return;
        }

        if (Time.time - lastCastTime < skillCooldown) return;

        int randomSkill = Random.Range(0, 2); // 0 = summon, 1 = storm

        if (randomSkill == 0 && AllSummonedEnemiesDead())
        {
            StartCoroutine(SummonRoutine());
        }
        else if (randomSkill == 1)
        {
            StartCoroutine(StormRoutine());
        }
    }

    IEnumerator CloseAttackRoutine()
    {
        lastMeleeTime = Time.time;
        isCasting = true;

        if (animator != null)
            animator.SetTrigger("CloseAttack");

        yield return new WaitForSeconds(meleeDelay);

        if (player != null && meleeEffectPrefab != null)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            Vector3 spawnPos = transform.position + (Vector3)(dir * 1f);
            Instantiate(meleeEffectPrefab, spawnPos, Quaternion.identity);
        }

        isCasting = false;
    }

    IEnumerator SummonRoutine()
    {
        isCasting = true;
        lastCastTime = Time.time;

        if (animator != null)
            animator.SetTrigger(summonTrigger);

        yield return new WaitForSeconds(summonDelay);

        if (player == null) yield break;

        RaycastHit2D hit = Physics2D.Raycast(player.position, Vector2.down, 10f, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            Vector3 basePos = hit.point + Vector2.up * 0.8f;

            for (int i = 0; i < 3; i++)
            {
                Vector3 offset = new Vector3((i - 1) * 1f, 0, 0);
                GameObject enemy = Instantiate(summonPrefab, basePos + offset, Quaternion.identity);
                summonedEnemies[i] = enemy;
            }

            if (CameraShake.Instance != null)
                CameraShake.Instance.Shake(shakeDuration, shakeIntensity);
        }
        else
        {
            Debug.Log("Không tìm thấy mặt đất để triệu hồi.");
        }

        isCasting = false;
    }

    IEnumerator StormRoutine()
    {
        isCasting = true;
        lastCastTime = Time.time;

        if (animator != null)
            animator.SetTrigger("CastStorm");

        yield return new WaitForSeconds(stormDelay);

        if (player != null && stormPrefab != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(player.position, Vector2.down, 10f, LayerMask.GetMask("Ground"));
            if (hit.collider != null)
            {
                Vector3 groundPos = hit.point + Vector2.up * 2f;
                Instantiate(stormPrefab, groundPos, Quaternion.identity);

                if (CameraShake.Instance != null)
                    CameraShake.Instance.Shake(0.25f, 0.3f);
            }
            else
            {
                Debug.Log("Không tìm thấy mặt đất để đặt bão.");
            }
        }

        isCasting = false;
    }

    bool AllSummonedEnemiesDead()
    {
        foreach (var enemy in summonedEnemies)
        {
            if (enemy != null) return false;
        }
        return true;
    }

    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, closeAttackRange);
        }
    }
}
