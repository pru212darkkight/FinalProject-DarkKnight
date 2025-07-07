using UnityEngine;

public class MiniBossAttack : MonoBehaviour
{
    private Animator animator;
    private MiniBossController controller;

    public float attack1Damage = 20f;
    public float attack2Damage = 35f;
    public float attackRange = 1.5f;
    public LayerMask playerLayer;

    private bool isCombo = false; // Đánh dấu có đang combo không

    void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<MiniBossController>();
    }

    // Gọi khi boss tiến tới đủ gần player
    public void RandomAttack()
    {
        int rand = Random.Range(0, 3); // 0, 1, 2
        if (rand == 0)
        {
            isCombo = false;
            animator.SetBool("IsCombo", false);  // Đánh thường
            animator.SetTrigger("Attack1");
        }
        else if (rand == 1)
        {
            isCombo = false;
            animator.SetBool("IsCombo", false);  // Đánh thường
            animator.SetTrigger("Attack2");
        }
        else
        {
            isCombo = true;
            animator.SetBool("IsCombo", true);   // Bật combo
            animator.SetTrigger("Attack1");
        }
    }


    // Gọi ở cuối animation Attack1, chỉ khi là combo thì chuyển tiếp Attack2
    public void OnAttack1End()
    {
        if (!isCombo)
        {
            controller.EndAttack(); // Chỉ khi không combo thì về idle ngay
        }
    }

    // Gọi ở cuối animation Attack2
    public void OnAttack2End()
    {
        animator.SetBool("IsCombo", false); // Tắt combo về mặc định để lần sau dùng tiếp
        controller.EndAttack();
    }

    // Gọi ở frame thích hợp để gây damage
    public void DealAttack1Damage()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
        if (player != null)
        {
            PlayerController1 pc = player.GetComponent<PlayerController1>();
            if (pc != null)
                pc.TakeDamage(attack1Damage, false);
        }
    }

    public void DealAttack2Damage()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
        if (player != null)
        {
            PlayerController1 pc = player.GetComponent<PlayerController1>();
            if (pc != null)
                pc.TakeDamage(attack2Damage, false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
