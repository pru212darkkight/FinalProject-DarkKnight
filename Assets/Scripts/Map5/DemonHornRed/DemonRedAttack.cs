using UnityEngine;

public class DemonRedAttack : MonoBehaviour
{
    private Animator animator;
    private DemonRedController controller;

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
        controller = GetComponent<DemonRedController>();
    }

    public void RandomAttack()
    {
        int rand = Random.Range(0, 3);
        if (rand == 0)
        {
            isCombo = false;
            animator.SetBool("IsCombo", false);
            animator.SetTrigger("Attack1");

            // 🎵 Play attack1 sound
            if (AudioManager.Instance != null && AudioManager.Instance.demonRedAttack1 != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.demonRedAttack1);
            }
        }
        else if (rand == 1)
        {
            isCombo = false;
            animator.SetBool("IsCombo", false);
            animator.SetTrigger("Attack2");

            // 🎵 Play attack2 sound
            if (AudioManager.Instance != null && AudioManager.Instance.demonRedAttack2 != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.demonRedAttack2);
            }
        }
        else
        {
            isCombo = true;
            animator.SetBool("IsCombo", true);
            animator.SetTrigger("Attack1");

            // 🎵 Play attack1 sound for combo start
            if (AudioManager.Instance != null && AudioManager.Instance.demonRedAttack1 != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.demonRedAttack1);
            }
        }
    }

    public void OnAttack1End()
    {
        if (!isCombo)
            controller.EndAttack();
        else
        {
            // 🎵 Play attack2 sound for combo transition
            if (AudioManager.Instance != null && AudioManager.Instance.demonRedAttack2 != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.demonRedAttack2);
            }
        }
    }
    public void ResetAttackState()
    {
        isCombo = false;
        if (animator != null)
        {
            animator.SetBool("IsCombo", false); 
            animator.ResetTrigger("Attack1");
            animator.ResetTrigger("Attack2");
        }
    }
    public void OnAttack2End()
    {
        animator.SetBool("IsCombo", false);
        controller.EndAttack();
    }

    public void DealAttack1Damage()
    {
        Vector2 offset = attackBoxOffset;
        // Nếu scale.x > 0 là nhìn TRÁI, box nằm bên TRÁI
        if (transform.localScale.x > 0)
            offset.x = -Mathf.Abs(offset.x);
        else
            offset.x = Mathf.Abs(offset.x);
        Vector2 center = (Vector2)transform.position + offset;

        Collider2D player = Physics2D.OverlapBox(center, attackBoxSize, 0, playerLayer);
        if (player != null)
        {
            PlayerController1 pc = player.GetComponent<PlayerController1>();
            if (pc != null)
                pc.TakeDamage(attack1Damage, false, "Demon Horn Red");
        }
    }

    public void DealAttack2Damage()
    {
        Vector2 offset = attackBoxOffset;
        if (transform.localScale.x > 0)
            offset.x = -Mathf.Abs(offset.x);
        else
            offset.x = Mathf.Abs(offset.x);
        Vector2 center = (Vector2)transform.position + offset;

        Collider2D player = Physics2D.OverlapBox(center, attackBoxSize, 0, playerLayer);
        if (player != null)
        {
            PlayerController1 pc = player.GetComponent<PlayerController1>();
            if (pc != null)
                pc.TakeDamage(attack2Damage, false, "Demon Horn Red");
        }
    }


    // Vẽ vùng attack trong Editor (luôn thấy vùng gây damage đúng hướng)
    void OnDrawGizmosSelected()
    {
        if (Camera.main == null) return;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        if (screenPos.z > 0 &&
            screenPos.x >= 0 && screenPos.x <= Screen.width &&
            screenPos.y >= 0 && screenPos.y <= Screen.height)
        {
            Gizmos.color = Color.red;
            Vector2 offset = attackBoxOffset;
            if (transform.localScale.x > 0)
                offset.x = -Mathf.Abs(offset.x);
            else
                offset.x = Mathf.Abs(offset.x);
            Vector2 center = (Vector2)transform.position + offset;
            Gizmos.DrawWireCube(center, attackBoxSize);
        }
    }

}
