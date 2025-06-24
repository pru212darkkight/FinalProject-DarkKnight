using UnityEngine;

public class WaterfallControll : MonoBehaviour
{
    public GameObject[] waterfallParts; // phần từ trên xuống dưới
    public float delayBetweenParts = 0.3f;

    private bool isTurningOff = false;

    public void StopWaterfallSequence()
    {
        if (!isTurningOff)
            StartCoroutine(TurnOffParts());
    }

    public void StartWaterfallSequence()
    {
        StartCoroutine(TurnOnParts());
    }

    private System.Collections.IEnumerator TurnOffParts()
    {
        isTurningOff = true;
        for (int i = 0; i < waterfallParts.Length; i++)
        {
            var sr = waterfallParts[i].GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;

            var ps = waterfallParts[i].GetComponent<ParticleSystem>();
            if (ps != null) ps.Stop();

            yield return new WaitForSeconds(delayBetweenParts);
        }
    }

    private System.Collections.IEnumerator TurnOnParts()
    {
        // Bật lại từ dưới lên
        for (int i = 0; i <= waterfallParts.Length - 1; i++)
        {
            var sr = waterfallParts[i].GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = true;

            var ps = waterfallParts[i].GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();

            yield return new WaitForSeconds(delayBetweenParts);
        }

        isTurningOff = false; // Cho phép tắt lại lần sau
    }
}
