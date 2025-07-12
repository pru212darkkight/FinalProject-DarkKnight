using Unity.Cinemachine;
using UnityEngine;

public class BridgeMover : MonoBehaviour
{
    public Transform destinationPoint;
    public float moveSpeed = 2f;
    public float checkRadius = 5f;
    public LayerMask enemyLayer;

    public CinemachineCamera virtualCamera;
    public Transform playerTransform;
    public Transform bridgeFrontPoint;

    public AudioClip bridgeMovingSound; // Âm thanh cầu di chuyển

    private AudioSource audioSource;
    private bool shouldMove = false;
    private bool cameraFollowingBridge = false;
    private bool bridgeReachedDestination = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = bridgeMovingSound;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

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

                if (virtualCamera != null && bridgeFrontPoint != null)
                {
                    virtualCamera.Follow = bridgeFrontPoint;
                    cameraFollowingBridge = true;
                }

                if (!audioSource.isPlaying)
                {
                    audioSource.Play(); // Bắt đầu phát âm
                }
            }
        }
        else if (!bridgeReachedDestination)
        {
            transform.position = Vector3.MoveTowards(transform.position, destinationPoint.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, destinationPoint.position) < 0.05f)
            {
                bridgeReachedDestination = true;

                if (virtualCamera != null && playerTransform != null)
                {
                    virtualCamera.Follow = playerTransform;
                    cameraFollowingBridge = false;
                }

                if (audioSource.isPlaying)
                {
                    audioSource.Stop(); // Dừng âm khi cầu đã đến nơi
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
