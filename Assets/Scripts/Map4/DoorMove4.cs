using UnityEngine;

public class DoorMove4 : MonoBehaviour
{
    public Transform destinationPoint;
    public float moveSpeed = 2f;
    public float checkRadius = 5f;
    public LayerMask enemyLayer;

    public Transform checkPoint; // Vị trí kiểm tra enemy

    private bool shouldMove = false;
    private bool bridgeReachedDestination = false;

    void Update()
    {
        if (!shouldMove)
        {
            Vector3 checkPos = checkPoint != null ? checkPoint.position : transform.position;

            Collider2D[] enemies = Physics2D.OverlapCircleAll(checkPos, checkRadius, enemyLayer);

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
        else if (!bridgeReachedDestination)
        {
            transform.position = Vector3.MoveTowards(transform.position, destinationPoint.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, destinationPoint.position) < 0.05f)
            {
                bridgeReachedDestination = true;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 checkPos = checkPoint != null ? checkPoint.position : transform.position;
        Gizmos.DrawWireSphere(checkPos, checkRadius);
    }
}
