using UnityEngine;

public class Trap4 : MonoBehaviour
{
    [Header("Trap Settings")]
    public Animator trapAnimator;
    public float fallSpeed = 5f;
    public bool useGravity = true;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip warningSound;

    private bool falling = false;
    private Rigidbody2D rb;

    void Start()
    {
        if (useGravity)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.bodyType = RigidbodyType2D.Kinematic;
        }

        if (trapAnimator == null)
            trapAnimator = GetComponent<Animator>();
    }

    // Hàm gọi từ trigger
    public void ActivateTrap()
    {
        if (audioSource && warningSound)
            audioSource.PlayOneShot(warningSound);

        if (trapAnimator)
            trapAnimator.SetTrigger("Activate");
        else
            StartFalling(); // Nếu không có animator thì rơi luôn
    }

    // Gọi từ Animation Event
    public void StartFalling()
    {
        if (useGravity && rb != null)
            rb.bodyType = RigidbodyType2D.Dynamic;
        else
            falling = true;

        Destroy(gameObject, 3f);
    }

    void Update()
    {
        if (falling)
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }
}
