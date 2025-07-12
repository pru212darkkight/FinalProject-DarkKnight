using UnityEngine;

public class ThunderHitbox : MonoBehaviour
{
    private float damage;
    private LayerMask playerLayer;
    private bool isMagic;
    private bool hasHit = false;
    private string enemyName;
    private BoxCollider2D box;

    public void Init(float _damage, LayerMask _playerLayer, bool _isMagic, string Name)
    {
        damage = _damage;
        playerLayer = _playerLayer;
        isMagic = _isMagic;
        box = GetComponent<BoxCollider2D>();
        if (box != null)
            box.enabled = false; // Tắt collider ngay lúc spawn!
        enemyName = Name;
    }

    // Gọi từ Animation Event (đúng frame sét chạm đất)
    public void ActivateHitbox()
    {
        hasHit = false; // Cho phép nhận damage lại
        if (box != null)
            box.enabled = true; // Bật collider lên để gây damage trigger
    }

    // Sau khi gây damage frame đó xong, gọi lại để tắt
    public void DeactivateHitbox()
    {
        if (box != null)
            box.enabled = false;
    }

    // Trigger chỉ chạy khi collider đang bật!
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;
        // Chỉ check player và đúng layer
        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            var player = collision.GetComponent<PlayerController1>();
            if (player != null)
            {
                player.TakeDamage(damage, isMagic, enemyName);
                hasHit = true;
            }
        }
    }
    // Hết animation thì gọi hàm này (Animation Event)
    public void DestroyAfterAnimation()
    {
        Destroy(gameObject);
    }
}
