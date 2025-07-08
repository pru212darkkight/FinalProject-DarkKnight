using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2f;
    private Vector3 target;

    [Header("Can be locked from outside")]
    public bool lockByScript = false;

    void Start()
    {
        target = pointA.position;
    }

    void Update()
    {
        if (lockByScript) return; // Bị khoá thì không chạy

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            target = (target == pointA.position) ? pointB.position : pointA.position;
        }
    }

    public void LockPlatform()
    {
        lockByScript = true;
    }
    public void UnlockPlatform()
    {
        lockByScript = false;
        target = pointB.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            collision.transform.SetParent(transform, true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            collision.transform.SetParent(null);
    }
}
