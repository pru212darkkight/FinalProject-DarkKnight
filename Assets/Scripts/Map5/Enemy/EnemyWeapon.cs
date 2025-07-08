using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public float attackDamage = 20;
    public Vector2 attackBoxSize = new Vector2(1.5f, 1.2f);   // Rộng x Cao của box gây damage
    public Vector2 attackBoxOffset = new Vector2(1f, 0f);     // Offset về phía trước mặt
    public LayerMask attackMask;

    public void Attack()
    {
        // Sử dụng lossyScale.x để luôn đúng với mọi scale của cha, kể cả nested prefab/phức tạp
        float facing = Mathf.Sign(transform.lossyScale.x);
        Vector2 offset = new Vector2(attackBoxOffset.x * facing, attackBoxOffset.y);
        Vector2 boxCenter = (Vector2)transform.position + offset;

        Collider2D colInfo = Physics2D.OverlapBox(boxCenter, attackBoxSize, 0f, attackMask);
        if (colInfo != null)
        {
            var playerHealth = colInfo.GetComponent<PlayerController1>();
            if (playerHealth != null)
                playerHealth.TakeDamage(attackDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        float facing = Application.isPlaying ? Mathf.Sign(transform.lossyScale.x) : Mathf.Sign(transform.localScale.x);
        Vector2 offset = new Vector2(attackBoxOffset.x * facing, attackBoxOffset.y);
        Vector2 center = (Vector2)transform.position + offset;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, attackBoxSize);
    }
}
