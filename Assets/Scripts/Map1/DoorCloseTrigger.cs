using UnityEngine;

/// <summary>
/// This script is placed on a trigger volume inside the boss room.
/// When the player enters it, it tells the assigned BossDoor to close.
/// </summary>
public class DoorCloseTrigger : MonoBehaviour
{
    [Header("Target Door")]
    [Tooltip("Drag the Boss Door GameObject here in the Inspector.")]
    public BossDoor bossDoor;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that entered the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Check if a door has been assigned to prevent errors
            if (bossDoor != null)
            {
                Debug.Log("Player entered the boss room. Closing the door.");
                // Call the public method on the door script to close it
                bossDoor.ClosePermanently();

                // Deactivate this trigger object so it doesn't run again
                gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("No BossDoor has been assigned to this trigger!", this.gameObject);
            }
        }
    }
}
