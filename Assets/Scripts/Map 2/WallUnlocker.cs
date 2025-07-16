using UnityEngine;

public class WallUnlocker : MonoBehaviour
{
    [Header("Enemy cần theo dõi")]
    public GameObject enemy;

    [Header("Tường chắn sẽ biến mất")]
    public GameObject wall;

    void Update()
    {
        if (enemy == null && wall != null)
        {
            wall.SetActive(false); // hoặc Destroy(wall);
            this.enabled = false; // Tắt script để không kiểm tra nữa
        }
    }
}
