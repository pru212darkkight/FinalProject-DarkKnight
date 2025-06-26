using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClimbStep : MonoBehaviour
{
    public float speed = 3f;
    public float stepHeight = 0.25f;
    public float stepCheckDistance = 0.2f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Chỉ di chuyển bằng phím mũi tên
        float moveInput = 0;
        if (Keyboard.current.leftArrowKey.isPressed) moveInput -= 1;
        if (Keyboard.current.rightArrowKey.isPressed) moveInput += 1;

        // Raycast kiểm tra bước lên bậc thang
        if (moveInput != 0)
        {
            Vector2 originLow = transform.position + new Vector3(moveInput * 0.5f, -0.2f, 0);
            Vector2 originHigh = transform.position + new Vector3(moveInput * 0.5f, stepHeight, 0);

            RaycastHit2D hitLow = Physics2D.Raycast(originLow, Vector2.right * moveInput, stepCheckDistance, LayerMask.GetMask("Ground"));
            RaycastHit2D hitHigh = Physics2D.Raycast(originHigh, Vector2.right * moveInput, stepCheckDistance, LayerMask.GetMask("Ground"));

            if (hitLow.collider != null && hitHigh.collider == null)
            {
                transform.position += new Vector3(0, stepHeight, 0);
            }
        }

        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }   
}
