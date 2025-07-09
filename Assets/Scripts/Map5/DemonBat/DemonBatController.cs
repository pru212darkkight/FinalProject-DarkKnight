using UnityEngine;

public class DemonBatController : MonoBehaviour
{
    [Header("Patrol")]
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 3f;

    [Header("Detection")]
    public Transform player;
    public float detectRadius = 5f; // Vùng phát hiện là hình tròn

    [Header("Health Regen")]
    public float healthRegenRate = 5f; // Tốc độ hồi máu mỗi giây

    private Transform currentTarget;
    private Animator animator;
    private Rigidbody2D rb;
    private DemonBatAttack attackScript;
    private EnemyHealth healthScript;

    private enum State { Patrol, Attack, Return }
    private State currentState = State.Patrol;
    private float patrolY; // Độ cao Y cố định để bay
    private bool reachedPoint = false; // Để tránh đảo target liên tục khi vừa tới điểm

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        attackScript = GetComponent<DemonBatAttack>();
        healthScript = GetComponent<EnemyHealth>();
        currentTarget = pointB;
        patrolY = transform.position.y; // Lưu độ cao khi khởi tạo để dùng trong bay
    }

    void Update()
    {
        if (healthScript != null && healthScript.isDead) return;

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                HealthRegen(); // <-- Hồi máu trong trạng thái tuần tra
                if (PlayerInDetectZone())
                    currentState = State.Attack;
                break;

            case State.Attack:
                animator.SetBool("IsRunning", false);
                LookAtTarget(player.position.x);
                MoveToTarget(new Vector2(player.position.x, patrolY));
                attackScript.Attack();

                if (!PlayerInDetectZone())
                    currentState = State.Return;
                break;

            case State.Return:
                // Bay về điểm gần nhất, giữ nguyên Y (patrolY)
                Vector2 returnTarget = new Vector2(ClosestPatrolPoint().position.x, patrolY);
                if (Vector2.Distance(transform.position, returnTarget) > 0.15f)
                {
                    MoveToTarget(returnTarget);
                    animator.SetBool("IsRunning", true);
                    LookAtTarget(returnTarget.x);
                }
                else
                {
                    // Khi về gần đúng điểm tuần tra sẽ chọn điểm tiếp theo
                    currentTarget = (ClosestPatrolPoint() == pointA) ? pointB : pointA;
                    currentState = State.Patrol;
                }
                HealthRegen();
                break;
        }
    }

    void Patrol()
    {
        animator.SetBool("IsRunning", true);
        Vector2 patrolTarget = new Vector2(currentTarget.position.x, patrolY);
        MoveToTarget(patrolTarget);

        float dist = Vector2.Distance(transform.position, patrolTarget);

        // Fix đảo target liên tục: chỉ đổi khi vừa rời điểm rồi quay lại
        if (!reachedPoint && dist < 0.15f)
        {
            reachedPoint = true;
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
        else if (dist >= 0.15f)
        {
            reachedPoint = false;
        }

        LookAtTarget(currentTarget.position.x);
    }

    void MoveToTarget(Vector2 targetPos)
    {
        Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, moveSpeed * Time.deltaTime);
        rb.MovePosition(newPos);
    }

    bool PlayerInDetectZone()
    {
        // Phát hiện trong vòng tròn quanh bat
        return Vector2.Distance(player.position, transform.position) <= detectRadius;
    }

    Transform ClosestPatrolPoint()
    {
        float dA = Mathf.Abs(transform.position.x - pointA.position.x);
        float dB = Mathf.Abs(transform.position.x - pointB.position.x);
        return (dA < dB) ? pointA : pointB;
    }

    void LookAtTarget(float targetX)
    {
        Vector3 scale = transform.localScale;
        if (targetX > transform.position.x)
            scale.x = -Mathf.Abs(scale.x); // Quay đầu sang phải
        else if (targetX < transform.position.x)
            scale.x = Mathf.Abs(scale.x);  // Quay đầu sang trái
        transform.localScale = scale;
    }

    // Hồi máu từ từ khi không combat
    void HealthRegen()
    {
        if (healthScript != null && healthScript.currentHealth < healthScript.maxHealth)
        {
            healthScript.currentHealth += healthRegenRate * Time.deltaTime;
            healthScript.currentHealth = Mathf.Min(healthScript.currentHealth, healthScript.maxHealth);
        }
    }

    // Vẽ vùng phát hiện hình tròn và điểm tuần tra
    void OnDrawGizmosSelected()
    {
        if (Camera.main == null) return;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        if (screenPos.z > 0 &&
            screenPos.x >= 0 && screenPos.x <= Screen.width &&
            screenPos.y >= 0 && screenPos.y <= Screen.height)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectRadius); // Vùng phát hiện là hình tròn

            Gizmos.color = Color.yellow;
            if (pointA != null) Gizmos.DrawSphere(pointA.position, 0.15f);
            if (pointB != null) Gizmos.DrawSphere(pointB.position, 0.15f);
        }
    }
}
