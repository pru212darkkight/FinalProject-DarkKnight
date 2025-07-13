using UnityEngine;
using UnityEngine.InputSystem;

public class ClimbLadder : MonoBehaviour
{
    [Header("Input Action Reference")]
    public InputActionReference climbInput; // Gắn trong Inspector

    public float climbSpeed = 3f;
    private bool isClimbing = false;
    private Rigidbody2D rb;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        climbInput.action.Enable(); // Kích hoạt input
    }

    private void OnDisable()
    {
        climbInput.action.Disable(); // Ngắt input
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            isClimbing = true;
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;

            if (AudioManager.Instance != null && AudioManager.Instance.playerFootstep != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.playerFootstep);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            isClimbing = false;
            rb.gravityScale = 1f;
            animator.SetBool("IsClimbing", false);
        }
    }

    private void Update()
    {
        Vector2 climb = climbInput.action.ReadValue<Vector2>();
        float vertical = climb.y;

        if (isClimbing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, vertical * climbSpeed);
            animator.SetBool("IsClimbing", Mathf.Abs(vertical) > 0.1f); // bật animation
        }
        else
        {
            animator.SetBool("IsClimbing", false); // tắt animation nếu rời khỏi thang
        }
    }

}
