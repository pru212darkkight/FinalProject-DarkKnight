using UnityEngine;

public class MeduxaAttack : MonoBehaviour
{
    public Transform player;
    public GameObject spikePrefab;
    public GameObject minionPrefab;
    public GameObject poisonPrefab;      // Prefab hiệu ứng đám mây độc (nếu có)
    public Transform poisonSpawnPoint;

    public float castRange = 6f;
    public float cooldown = 4f;
    public float summonRadius = 2f;  // bán kính triệu hồi
    public int minionCount = 3;
    public float poisonRadius = 4f;      // Bán kính ảnh hưởng của độc
    public float poisonDamage = 10f;

    private float nextCastTime = 0f;
    private Animator animator;

    public LayerMask groundLayer;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= castRange && Time.time >= nextCastTime)
        {
            int randomSkill = Random.Range(0, 3); // [0, 3)

            if (randomSkill == 0)
            {
                animator?.SetTrigger("Cast"); // Gọi chiêu spike
            }
            else if (randomSkill == 1)
            {
                animator?.SetTrigger("Cast"); // Triệu hồi minion
            }
            else
            {
                animator?.SetTrigger("Poison"); // Phun độc
            }
            nextCastTime = Time.time + cooldown;
        }
    }



    // GỌI từ Animation Event tại frame tung chiêu
    public void SpawnSpike()
    {
        if (player != null && spikePrefab != null)
        {
            Vector2 rayOrigin = new Vector2(player.position.x, player.position.y);
            float maxDistance = 5f; // Tầm dò xuống đất

            // Bắn raycast xuống
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, maxDistance, groundLayer);

            if (hit.collider != null)
            {
                // Mọc spike tại mặt đất dưới chân player]
                Vector3 offset = new Vector3(-4f, 1.2f, 0f); // lệch phải 1 đơn vị, cao hơn 0.2 đơn vị
                Vector3 spawnPos = new Vector3(player.position.x, hit.point.y, 0f) + offset;
                GameObject spikeInstance = Instantiate(spikePrefab, spawnPos, Quaternion.identity);

                MeduxaSpike spikeScript = spikeInstance.GetComponent<MeduxaSpike>();
                if (spikeScript != null)
                    spikeScript.ActivateAllTraps();

                Destroy(spikeInstance, 1f);
            }
            else
            {
                Debug.Log("Không tìm thấy mặt đất bên dưới player!");
            }
        }
    }

    //public void SummonMinions()
    //{
    //    if (minionPrefab == null) return;

    //    for (int i = 0; i < minionCount; i++)
    //    {
    //        Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * summonRadius;
    //        Instantiate(minionPrefab, spawnPos, Quaternion.identity);
    //    }
    //}

    public void BreathPoison()
    {
        if (poisonSpawnPoint == null) return;

        // Tìm các Collider trong vùng ảnh hưởng
        Collider2D[] hits = Physics2D.OverlapCircleAll(poisonSpawnPoint.position, poisonRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                // Gây sát thương cho Player (dùng PlayerController1)
                PlayerController1 player = hit.GetComponent<PlayerController1>();
                if (player != null)
                {
                    player.TakeDamage(poisonDamage, true);
                    Debug.Log("Player trúng độc!");
                }
            }
        }

        // Tạo hiệu ứng đám mây độc nếu có prefab
        if (poisonPrefab != null)
        {
            // Lấy hướng xoay của enemy
            float directionX = transform.localScale.x;

            // Nếu prefab cần quay theo hướng, chỉnh scale X
            Vector3 spawnScale = poisonPrefab.transform.localScale;
            spawnScale.x = Mathf.Abs(spawnScale.x) * Mathf.Sign(directionX);

            // Tạo poison
            GameObject poison = Instantiate(poisonPrefab, poisonSpawnPoint.position, Quaternion.identity);
            poison.transform.localScale = spawnScale;

            Destroy(poison, 3f);
        }
    }



    void OnDrawGizmosSelected()
    {
        // Vùng tấn công cast
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, castRange);

        // Vùng độc phun ra từ poisonSpawnPoint
        if (poisonSpawnPoint != null)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f); // Xanh lá mờ
            Gizmos.DrawWireSphere(poisonSpawnPoint.position, poisonRadius);
        }
    }




}
