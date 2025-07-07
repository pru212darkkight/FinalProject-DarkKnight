using UnityEngine;

public class MeduxaAttack : MonoBehaviour
{
    public Transform player;
    public GameObject spikePrefab;
    public float castRange = 6f;
    public float cooldown = 4f;

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
            if (animator)
                animator.SetTrigger("Cast");

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


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, castRange);
    }
}
