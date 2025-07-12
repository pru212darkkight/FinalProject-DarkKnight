using UnityEngine;
using Unity.Cinemachine;

public class DoorMove4 : MonoBehaviour
{
    public Transform destinationPoint;
    public float moveSpeed = 2f;
    public float checkRadius = 5f;
    public LayerMask enemyLayer;
    public Transform checkPoint;

    public AudioClip moveSound;

    public CinemachineCamera virtualCamera;
    public Transform playerTransform;
    public Transform doorFrontPoint;

    private bool shouldMove = false;
    private bool bridgeReachedDestination = false;
    private bool cameraFollowingDoor = false;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = moveSound;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

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

                if (virtualCamera != null && doorFrontPoint != null)
                {
                    virtualCamera.Follow = doorFrontPoint;
                    cameraFollowingDoor = true;
                }
            }
        }
        else if (!bridgeReachedDestination)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            transform.position = Vector3.MoveTowards(transform.position, destinationPoint.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, destinationPoint.position) < 0.05f)
            {
                bridgeReachedDestination = true;

                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }

                if (virtualCamera != null && playerTransform != null)
                {
                    virtualCamera.Follow = playerTransform;
                    cameraFollowingDoor = false;
                }
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
