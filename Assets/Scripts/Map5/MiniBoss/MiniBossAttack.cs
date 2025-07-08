using UnityEngine;

public class MiniBossAttack : MonoBehaviour
{
    private Animator animator;
    private MiniBossController controller;

    public float attack1Damage = 20f;
    public float attack2Damage = 35f;
    public LayerMask playerLayer;

    // Vùng đánh dạng Box phía trước
    public Vector2 attackBoxSize = new Vector2(1.5f, 1.2f); // chỉnh theo ý bạn
    public Vector2 attackBoxOffset = new Vector2(1f, 0f);

    private bool isCombo = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<MiniBossController>();
    }

    public void RandomAttack()
    {
        int rand = Random.Range(0, 3);
        if (rand == 0)
        {
            isCombo = false;
            animator.SetBool("IsCombo", false);
            animator.SetTrigger("Attack1");
        }
        else if (rand == 1)
        {
            isCombo = false;
            animator.SetBool("IsCombo", false);
            animator.SetTrigger("Attack2");
        }
        else
        {
            isCombo = true;
            animator.SetBool("IsCombo", true);
            animator.SetTrigger("Attack1");
        }
    }

    public void OnAttack1End()
    {
        if (!isCombo)
            controller.EndAttack();
    }

    public void OnAttack2End()
    {
        animator.SetBool("IsCombo", false);
        controller.EndAttack();
    }

    // --- GÂY DAMAGE (tốt nhất là gọi từ Animation Event)
    public void DealAttack1Damage()
    {
        Vector2 offset = attackBoxOffset;
        if (transform.localScale.x < 0)
            offset.x = -Mathf.Abs(offset.x);
        else
            offset.x = Mathf.Abs(offset.x);
        Vector2 center = (Vector2)transform.position + offset;

        Collider2D player = Physics2D.OverlapBox(center, attackBoxSize, 0, playerLayer);
        if (player != null)
        {
            PlayerController1 pc = player.GetComponent<PlayerController1>();
            if (pc != null)
                pc.TakeDamage(attack1Damage, false);
        }
    }

    public void DealAttack2Damage()
    {
        Vector2 offset = attackBoxOffset;
        if (transform.localScale.x < 0)
            offset.x = -Mathf.Abs(offset.x);
        else
            offset.x = Mathf.Abs(offset.x);
        Vector2 center = (Vector2)transform.position + offset;

        Collider2D player = Physics2D.OverlapBox(center, attackBoxSize, 0, playerLayer);
        if (player != null)
        {
            PlayerController1 pc = player.GetComponent<PlayerController1>();
            if (pc != null)
                pc.TakeDamage(attack2Damage, false);
        }
    }

    // Vẽ vùng attack trong Editor (luôn thấy vùng gây damage đúng hướng)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 offset = attackBoxOffset;
        if (transform.localScale.x < 0)
            offset.x = -Mathf.Abs(offset.x);
        else
            offset.x = Mathf.Abs(offset.x);
        Vector2 center = (Vector2)transform.position + offset;
        Gizmos.DrawWireCube(center, attackBoxSize);
    }
}
