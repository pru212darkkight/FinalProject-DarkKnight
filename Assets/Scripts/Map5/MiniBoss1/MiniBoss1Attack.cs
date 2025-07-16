using UnityEngine;

public class MiniBoss1Attack : MonoBehaviour
{
    private Animator animator;
    private MiniBoss1Controller controller;

    public float attack1Damage = 20f;
    public LayerMask playerLayer;

    // --- Đánh cận chiến ---
    public Vector2 attackBoxSize = new Vector2(1.5f, 1.2f);
    public Vector2 attackBoxOffset = new Vector2(1f, 0f);

    // --- Sấm sét ---
    public GameObject thunderPrefab;
    public float thunderHeight = 3.0f;       // Điều chỉnh độ cao nơi spawn thunder
    public float thunderDistance = 1.2f;     // Điều chỉnh khoảng cách giữa các thunder
    public float thunderDamage = 35f;
    public float thunderDelay = 0.3f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<MiniBoss1Controller>();
    }

    public void RandomAttack(bool allowMelee)
    {
        // Nếu được phép attack gần thì random cast/attack1
        if (allowMelee)
        {
            int rand = Random.Range(0, 2); // 0 = Attack, 1 = Cast
            if (rand == 0)
            {
                // 🎵 Play attack sound for melee
                if (AudioManager.Instance != null && AudioManager.Instance.miniBoss1Attack != null)
                {
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.miniBoss1Attack);
                    Debug.Log("⚔️ MiniBoss1 Melee Attack - playing sound!");
                }
                animator.SetTrigger("Attack");
            }
            else
            {
                // 🎵 Play attack sound for cast
                if (AudioManager.Instance != null && AudioManager.Instance.miniBoss1Attack != null)
                {
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.miniBoss1Attack);
                    Debug.Log("⚡ MiniBoss1 Thunder Cast - playing sound!");
                }
                animator.SetTrigger("Cast");
            }
        }
        else
        {
            // 🎵 Play attack sound for cast only
            if (AudioManager.Instance != null && AudioManager.Instance.miniBoss1Attack != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.miniBoss1Attack);
                Debug.Log("⚡ MiniBoss1 Thunder Cast (ranged) - playing sound!");
            }
            animator.SetTrigger("Cast"); // Chỉ cast
        }
    }


    // Cận chiến
    public void DealAttack1Damage()
    {
        int dir = transform.localScale.x > 0 ? -1 : 1; // scale.x > 0 nhìn trái, < 0 nhìn phải
        Vector2 offset = new Vector2(Mathf.Abs(attackBoxOffset.x) * dir, attackBoxOffset.y);
        Vector2 center = (Vector2)transform.position + offset;

        Collider2D player = Physics2D.OverlapBox(center, attackBoxSize, 0, playerLayer);
        if (player != null)
        {
            PlayerController1 pc = player.GetComponent<PlayerController1>();
            if (pc != null)
                pc.TakeDamage(attack1Damage, false, "Bringer Of Death");
        }
    }

    public void OnAttackEnd() => controller.EndAttack();

    // CAST SKILL
    public void CastThunder()
    {
        // Chỉ lấy X của player, còn Y luôn = boss.y + thunderHeight
        float baseY = transform.position.y + thunderHeight;
        float px = controller.player.position.x;

        Vector3[] thunderPositions = new Vector3[]
        {
            new Vector3(px, baseY, 0),
            new Vector3(px - thunderDistance, baseY, 0),
            new Vector3(px + thunderDistance, baseY, 0)
        };

        foreach (var pos in thunderPositions)
        {
            GameObject thunder = Instantiate(thunderPrefab, pos, Quaternion.identity);
            ThunderHitbox hitbox = thunder.GetComponent<ThunderHitbox>();
            if (hitbox != null)
                hitbox.Init(thunderDamage, playerLayer, true, "Bringer Of Death");
        }
    }

    public void OnCastEnd() => controller.EndAttack();

    void OnDrawGizmosSelected()
    {
        // Melee
        Gizmos.color = Color.red;
        int dir = transform.localScale.x > 0 ? -1 : 1; // Flip theo hướng nhìn
        Vector2 offset = new Vector2(Mathf.Abs(attackBoxOffset.x) * dir, attackBoxOffset.y);
        Vector2 center = (Vector2)transform.position + offset;
        Gizmos.DrawWireCube(center, attackBoxSize);

        // Thunder
        Transform playerTf = controller != null ? controller.player : null;
        float px = playerTf != null ? playerTf.position.x : transform.position.x;
        float baseY = transform.position.y + thunderHeight;

        Vector3[] thunderPositions = new Vector3[]
        {
            new Vector3(px, baseY, 0),
            new Vector3(px - thunderDistance, baseY, 0),
            new Vector3(px + thunderDistance, baseY, 0)
        };

        Gizmos.color = Color.cyan;
        foreach (var pos in thunderPositions)
            Gizmos.DrawWireSphere(pos, 0.15f);
    }
}
