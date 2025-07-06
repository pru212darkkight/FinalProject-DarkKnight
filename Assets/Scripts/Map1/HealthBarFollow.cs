using UnityEngine;

public class HealthBarFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // Enemy transform (wolf)
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Offset từ enemy
    
    [Header("Follow Settings")]
    public bool followTarget = true;
    public bool billboardToCamera = true; // Luôn hướng về camera
    public float smoothSpeed = 5f; // Tốc độ follow mượt
    
    [Header("Camera Settings")]
    public Camera mainCamera;
    
    private Vector3 desiredPosition;
    private Vector3 smoothedPosition;
    
    void Start()
    {
        // Tự động tìm camera nếu không gán
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        // Tự động tìm target nếu không gán (tìm enemy gần nhất)
        if (target == null)
        {
            Enemy enemy = GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                target = enemy.transform;
            }
        }
    }
    
    void Update()
    {
        if (target == null || !followTarget) return;
        
        // Tính vị trí mong muốn
        desiredPosition = target.position + offset;
        
        // Smooth follow
        smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
        
        // Billboard effect (luôn hướng về camera)
        if (billboardToCamera && mainCamera != null)
        {
            transform.LookAt(mainCamera.transform);
            transform.Rotate(0, 180, 0); // Xoay 180 độ để mặt trước hướng camera
        }
    }
    
    // Hàm để set target từ script khác
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    // Hàm để set offset từ script khác
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
    
    // Hàm để enable/disable follow
    public void SetFollow(bool enable)
    {
        followTarget = enable;
    }
    
    // Hàm để enable/disable billboard
    public void SetBillboard(bool enable)
    {
        billboardToCamera = enable;
    }
    
    // Hiển thị gizmos trong Scene view để debug
    void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(target.position + offset, 0.1f);
            Gizmos.DrawLine(target.position, target.position + offset);
        }
    }
} 