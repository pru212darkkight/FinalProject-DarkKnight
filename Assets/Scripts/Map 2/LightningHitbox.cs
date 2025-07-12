using UnityEngine;

public class LightningHitbox : MonoBehaviour
{
    private float damage;
    private LayerMask playerLayer;
    private bool isMagic;
    private bool hasHit = false;

    private BoxCollider2D box;

    public void Init(float _damage, LayerMask _playerLayer, bool _isMagic)
    {
        damage = _damage;
        playerLayer = _playerLayer;
        isMagic = _isMagic;
        box = GetComponent<BoxCollider2D>();
        if (box != null)
            box.enabled = false; // Tắt collider ngay lúc spawn!
    }

    // Gọi từ Animation Event (frame tia sét chạm đất)
    public void ActivateHitbox()
    {
        hasHit = false;
        if (box != null)
            box.enabled = true; // Bật collider để gây damage
    }

    // Tắt collider sau khi đã gây damage
    public void DeactivateHitbox()
    {
        if (box != null)
            box.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            var player = collision.GetComponent<PlayerController1>();
            if (player != null)
            {
                player.TakeDamage(damage, isMagic);
                hasHit = true;
            }
        }
    }

    // Gọi khi kết thúc animation (qua Animation Event)
    public void DestroyAfterAnimation()
    {
        Destroy(gameObject);
    }
}
