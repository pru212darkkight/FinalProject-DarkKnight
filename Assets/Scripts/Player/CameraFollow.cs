using UnityEngine;

public class CameraFollow:MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float timeOffset;
    [SerializeField] Vector3 offsetPos;
    [SerializeField] Vector3 boundsMin;
    [SerializeField] Vector3 boundsMax;

    private void Start()
    {
        // Auto find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("CameraFollow: Player found and assigned!");
            }
            else
            {
                Debug.LogWarning("CameraFollow: Player not found! Looking for PlayerController1...");
                PlayerController1 playerController = FindObjectOfType<PlayerController1>();
                if (playerController != null)
                {
                    player = playerController.transform;
                    Debug.Log("CameraFollow: PlayerController1 found and assigned!");
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            Vector3 startPos = transform.position;
            Vector3 targetPos = player.position;

            targetPos.x += offsetPos.x;
            targetPos.y += offsetPos.y;
            targetPos.z = transform.position.z;

            targetPos.x = Mathf.Clamp(targetPos.x, boundsMin.x, boundsMax.x);
            targetPos.y = Mathf.Clamp(targetPos.y, boundsMin.y, boundsMax.y);

            float t = 1f - Mathf.Pow(1f - timeOffset, Time.deltaTime * 30);

            transform.position = Vector3.Lerp(startPos, targetPos, t);

        }
    }

}
