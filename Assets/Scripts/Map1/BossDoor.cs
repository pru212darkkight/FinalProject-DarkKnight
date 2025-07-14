using UnityEngine;
using System.Collections; // Required for using Coroutines

/// <summary>
/// A simple script for a boss door that drops down into place over time.
/// It will reset to its open state every time the scene is loaded.
/// </summary>
public class BossDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [Tooltip("How far the door will move down on the Y-axis to close.")]
    public float dropAmount = 5f;

    [Tooltip("How many seconds the drop animation should take. Higher number = slower drop.")]
    public float animationDuration = 2f; // Controls the speed of the drop

    // --- State ---
    private Vector3 openPosition;
    private bool isClosed = false; // Tracks if the door is closed in the current scene
    private bool isAnimating = false; // To prevent triggering the animation multiple times

    void Awake()
    {
        // Store the initial open position when the scene starts.
        openPosition = transform.position;
    }

    void Start()
    {
        // The door always starts in the open position.
        transform.position = openPosition;
    }

    /// <summary>
    /// Instantly moves the door to its final closed position.
    /// </summary>
    private void SetToClosedState()
    {
        transform.position = new Vector3(openPosition.x, openPosition.y - dropAmount, openPosition.z);
        isClosed = true;
    }

    /// <summary>
    /// Public method to close the door forever. This is called by the DoorCloseTrigger.
    /// </summary>
    public void ClosePermanently()
    {
        // Don't do anything if it's already closed or currently animating.
        if (isClosed || isAnimating)
        {
            return;
        }

        Debug.Log("Door is closing for this scene.");
        // Start the animation instead of moving instantly
        StartCoroutine(DropDownAnimation());
    }

    /// <summary>
    /// Coroutine that handles the dropdown animation over time.
    /// </summary>
    private IEnumerator DropDownAnimation()
    {
        isAnimating = true;

        // 🎵 Phát âm thanh đóng cửa boss
        if (AudioManager.Instance != null && AudioManager.Instance.bossDoorClose != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.bossDoorClose);
            Debug.Log("🚪 Boss door closing - playing door sound!");
        }

        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(openPosition.x, openPosition.y - dropAmount, openPosition.z);
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            // Move the door a little bit each frame using Lerp (Linear Interpolation)
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / animationDuration);

            // Wait for the next frame
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Animation finished, now finalize the state
        isAnimating = false;
        SetToClosedState(); // Snap to the final position to ensure it's perfect
    }
}
