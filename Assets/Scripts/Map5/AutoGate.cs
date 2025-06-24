using UnityEngine;

public class AutoGate : MonoBehaviour
{
    public Animator doorAnimator;  // Gắn Animator của cửa vào đây trong Inspector

    private int playerCount = 0;   // Số lượng phần collider của Player trong vùng cửa

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerCount++;
            if (playerCount == 1)
            {
                // Mở cửa nếu Player vừa mới bước vào
                doorAnimator.ResetTrigger("CloseTrigger");
                doorAnimator.SetTrigger("OpenTrigger");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerCount--;
            if (playerCount <= 0)
            {
                // Đóng cửa khi Player đã đi hết qua vùng trigger
                doorAnimator.ResetTrigger("OpenTrigger");
                doorAnimator.SetTrigger("CloseTrigger");
            }
        }
    }
}
