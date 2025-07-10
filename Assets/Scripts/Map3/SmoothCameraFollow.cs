using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // Player transform
    public bool autoFindPlayer = true;
    
    [Header("Follow Settings")]
    public float followSpeed = 2f;
    public float lookAheadDistance = 2f; // Camera nhìn trước hướng player di chuyển
    public float lookAheadSpeed = 1f;
    
    [Header("Offset Settings")]
    public Vector3 offset = new Vector3(0, 1, -10); // Offset từ player
    public bool useCustomOffset = false;
    
    [Header("Bounds Settings")]
    public bool useBounds = false;
    public Vector2 minBounds = new Vector2(-10, -5);
    public Vector2 maxBounds = new Vector2(10, 5);
    
    [Header("Smoothing Settings")]
    public bool smoothFollow = true;
    public bool smoothLookAhead = true;
    
    [Header("Debug")]
    public bool showDebug = false;
    
    // Private variables
    private Vector3 velocity = Vector3.zero;
    private float currentLookAhead = 0f;
    private PlayerController1 playerController;
    private Vector3 lastPlayerPosition;
    
    void Start()
    {
        // Auto find player if not assigned
        if (autoFindPlayer && target == null)
        {
            // Thử tìm bằng tag trước
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
                playerController = playerObj.GetComponent<PlayerController1>();
                Debug.Log("SmoothCameraFollow: Player found by tag and assigned!");
            }
            else
            {
                // Nếu không tìm thấy bằng tag, thử tìm bằng component
                PlayerController1 playerComp = FindObjectOfType<PlayerController1>();
                if (playerComp != null)
                {
                    target = playerComp.transform;
                    playerController = playerComp;
                    Debug.Log("SmoothCameraFollow: Player found by component and assigned!");
                }
                else
                {
                    Debug.LogError("SmoothCameraFollow: Player not found! Make sure player has 'Player' tag or PlayerController1 component!");
                }
            }
        }

        if (target != null)
        {
            lastPlayerPosition = target.position;

            // Set initial camera position
            Vector3 initialPos = target.position + offset;
            if (useBounds)
            {
                initialPos.x = Mathf.Clamp(initialPos.x, minBounds.x, maxBounds.x);
                initialPos.y = Mathf.Clamp(initialPos.y, minBounds.y, maxBounds.y);
            }
            transform.position = initialPos;
            Debug.Log($"SmoothCameraFollow: Camera positioned at {transform.position}, following {target.name}");
        }
        else
        {
            Debug.LogError("SmoothCameraFollow: No target assigned! Camera will not follow.");
        }
    }
    
    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("SmoothCameraFollow: Target is null in LateUpdate!");
            return;
        }

        // Calculate target position
        Vector3 targetPosition = CalculateTargetPosition();

        // Apply bounds if enabled
        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
        }

        // Move camera to target position
        if (smoothFollow)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 1f / followSpeed);
        }
        else
        {
            transform.position = targetPosition;
        }

        // Update last player position
        lastPlayerPosition = target.position;

        // Debug info - always show for troubleshooting
        if (showDebug || Time.frameCount % 60 == 0) // Show every 60 frames
        {
            Debug.Log($"SmoothCameraFollow: Camera at {transform.position}, Target at {target.position}, TargetPos: {targetPosition}");
        }
    }
    
    private Vector3 CalculateTargetPosition()
    {
        Vector3 targetPos = target.position + offset;
        
        // Look ahead based on player movement
        if (lookAheadDistance > 0)
        {
            float playerMovementX = 0f;
            
            // Get player input for look ahead
            if (playerController != null)
            {
                playerMovementX = playerController.MoveInput.x;
            }
            else
            {
                // Fallback: calculate movement from position change
                Vector3 movement = target.position - lastPlayerPosition;
                playerMovementX = movement.x / Time.deltaTime;
                playerMovementX = Mathf.Clamp(playerMovementX, -1f, 1f);
            }
            
            // Calculate look ahead
            float targetLookAhead = playerMovementX * lookAheadDistance;
            
            if (smoothLookAhead)
            {
                currentLookAhead = Mathf.Lerp(currentLookAhead, targetLookAhead, lookAheadSpeed * Time.deltaTime);
            }
            else
            {
                currentLookAhead = targetLookAhead;
            }
            
            targetPos.x += currentLookAhead;
        }
        
        return targetPos;
    }
    
    // Public methods for external control
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            playerController = target.GetComponent<PlayerController1>();
            lastPlayerPosition = target.position;
        }
    }
    
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
    
    public void SetBounds(Vector2 min, Vector2 max)
    {
        minBounds = min;
        maxBounds = max;
        useBounds = true;
    }
    
    public void DisableBounds()
    {
        useBounds = false;
    }
    
    // Snap camera to target immediately (useful for teleports)
    public void SnapToTarget()
    {
        if (target != null)
        {
            Vector3 snapPosition = CalculateTargetPosition();
            if (useBounds)
            {
                snapPosition.x = Mathf.Clamp(snapPosition.x, minBounds.x, maxBounds.x);
                snapPosition.y = Mathf.Clamp(snapPosition.y, minBounds.y, maxBounds.y);
            }
            transform.position = snapPosition;
            velocity = Vector3.zero;
            currentLookAhead = 0f;
        }
    }
    
    // Gizmos for debugging
    void OnDrawGizmosSelected()
    {
        if (useBounds)
        {
            Gizmos.color = Color.yellow;
            Vector3 center = new Vector3((minBounds.x + maxBounds.x) / 2, (minBounds.y + maxBounds.y) / 2, transform.position.z);
            Vector3 size = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0);
            Gizmos.DrawWireCube(center, size);
        }
        
        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.position + offset);
        }
    }
}
