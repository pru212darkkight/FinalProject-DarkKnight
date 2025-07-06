using UnityEngine;

public class TriggerBlockRemover : MonoBehaviour
{
    public GameObject blockToRemove; // Kéo block vào đây trong Inspector

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (blockToRemove != null)
            {
                // blockToRemove.SetActive(false); // Nếu muốn ẩn
                Destroy(blockToRemove); // Nếu muốn xóa hẳn
            }
        }
    }
}
