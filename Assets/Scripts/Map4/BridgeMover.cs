using UnityEngine;

public class BridgeMover : MonoBehaviour
{
    public Transform destinationPoint; // sB
    public float moveSpeed = 2f;
    public float checkRadius = 5f;
    public LayerMask enemyLayer;

    private bool shouldMove = false;

    void Update()
    {
        if (!shouldMove)
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, checkRadius, enemyLayer);

            int aliveCount = 0;
            foreach (var enemy in enemies)
            {
                if (enemy != null && enemy.gameObject.activeInHierarchy)
                {
                    aliveCount++;
                }
            }

            if (aliveCount == 0)
            {
                shouldMove = true;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, destinationPoint.position, moveSpeed * Time.deltaTime);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
