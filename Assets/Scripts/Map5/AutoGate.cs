using UnityEngine;

public class AutoGate : MonoBehaviour
{
    public Animator doorAnimator;
    public GameObject[] guardEnemies;
    public Collider2D blockCollider;

    [SerializeField] private bool onlyCloseOnce = false; // <--- THÊM BIẾN NÀY ĐỂ CHỌN KIỂU CỔNG
    private bool permanentlyClosed = false;

    private int playerCount = 0;
    private bool doorOpened = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !permanentlyClosed)
        {
            playerCount++;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !permanentlyClosed)
        {
            playerCount--;
            if (playerCount <= 0)
            {
                // Nếu cửa đã mở và player đã rời khỏi => đóng cửa
                if (doorOpened)
                {
                    doorAnimator.ResetTrigger("OpenTrigger");
                    doorAnimator.SetTrigger("CloseTrigger");
                    if (blockCollider != null)
                        blockCollider.enabled = true;
                    doorOpened = false;

                    // Nếu là cổng đóng 1 lần, đóng xong sẽ khóa luôn!
                    if (onlyCloseOnce)
                        permanentlyClosed = true;
                }
            }
        }
    }

    private void Update()
    {
        // Nếu là cổng khóa vĩnh viễn thì không mở nữa
        if (permanentlyClosed)
        {
            if (blockCollider != null) blockCollider.enabled = true;
            return;
        }

        // Khi enemy chết hết, DÙ CÓ PLAYER TRONG TRIGGER HAY KHÔNG, luôn mở cửa, tắt collider
        if (AllEnemiesDead())
        {
            if (playerCount > 0 && !doorOpened)
            {
                doorAnimator.ResetTrigger("CloseTrigger");
                doorAnimator.SetTrigger("OpenTrigger");
                doorOpened = true;
            }
            if (blockCollider != null && blockCollider.enabled)
            {
                blockCollider.enabled = false;
            }
        }
    }

    private bool AllEnemiesDead()
    {
        foreach (GameObject enemy in guardEnemies)
        {
            if (enemy != null)
            {
                var health = enemy.GetComponent<EnemyHealth>();
                if (health != null && !health.isDead)
                    return false;
            }
        }
        return true;
    }
}
