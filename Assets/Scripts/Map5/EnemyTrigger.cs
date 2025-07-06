using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    public EnemyAI enemyAI;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player vào vùng!");
            enemyAI.isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player ra khỏi vùng!");
            enemyAI.isPlayerInTrigger = false;
        }
    }
}
