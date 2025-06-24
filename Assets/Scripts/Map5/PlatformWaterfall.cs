using UnityEngine;

public class PlatformWaterfall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var waterfall = other.GetComponent<WaterfallControll>();
        if (waterfall != null)
        {
            waterfall.StopWaterfallSequence();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var waterfall = other.GetComponent<WaterfallControll>();
        if (waterfall != null)
        {
            waterfall.StartWaterfallSequence();
        }
    }
}
