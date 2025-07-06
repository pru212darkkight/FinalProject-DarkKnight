using UnityEngine;

public abstract class EnemyManager : MonoBehaviour
{
    [SerializeField] protected float enemyMoveSpeed = 1.0f;
    protected PlayerController1 player;

    protected virtual void Start()
    {
        player = FindAnyObjectByType<PlayerController1>();
    }

    protected virtual void Update()
    {
        MoveToPlayer();
    }

    protected void MoveToPlayer()
    {
        if (player != null)
        {
            // Di chuyển chỉ theo trục X
            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = new Vector3(player.transform.position.x, currentPosition.y, currentPosition.z);
            transform.position = Vector3.MoveTowards(currentPosition, targetPosition, enemyMoveSpeed * Time.deltaTime);

            FlipEnemy();
        }
    }

    protected void FlipEnemy()
    {
        if (player != null)
        {
            bool facingRight = player.transform.position.x > transform.position.x;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (facingRight ? 1 : -1);
            transform.localScale = scale;
        }
    }
}
