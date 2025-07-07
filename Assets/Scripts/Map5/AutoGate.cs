using UnityEngine;

public class AutoGate : MonoBehaviour
{
    public Animator doorAnimator;
    public GameObject[] guardEnemies;
    public Collider2D blockCollider;

    private int playerCount = 0;
    private bool doorOpened = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerCount++;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
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
                }
            }
        }
    }

    private void Update()
    {
        // Khi enemy chết hết, DÙ CÓ PLAYER TRONG TRIGGER HAY KHÔNG, luôn mở cửa, tắt collider
        if (AllEnemiesDead())
        {
            // Nếu có player trong trigger và cửa chưa mở thì mở cửa
            if (playerCount > 0 && !doorOpened)
            {
                doorAnimator.ResetTrigger("CloseTrigger");
                doorAnimator.SetTrigger("OpenTrigger");
                doorOpened = true;
            }
            // Dù có player hay không, collider luôn phải tắt khi đủ điều kiện
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
