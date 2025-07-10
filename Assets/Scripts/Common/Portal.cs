using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public float pullSpeed = 5f;
    private bool pulling = false;
    private Transform player;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            pulling = true;
            if (player.GetComponent<Rigidbody2D>() != null)
                player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }

    void Update()
    {
        if (pulling && player != null)
        {
            player.position = Vector2.MoveTowards(player.position, transform.position, pullSpeed * Time.unscaledDeltaTime);
            player.localScale = Vector3.Lerp(player.localScale, Vector3.zero, 5f * Time.unscaledDeltaTime);

            float dist = Vector2.Distance(player.position, transform.position);
            if (dist < 0.1f)
            {
                pulling = false;
                SceneManager.LoadScene("Home Village");
            }
        }
    }
}
