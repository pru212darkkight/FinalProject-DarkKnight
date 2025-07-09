using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Vector3 originalPos;
    private float shakeDuration = 0f;
    private float shakeAmount = 0.1f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            shakeDuration -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = originalPos;
        }
    }

    public void Shake(float duration, float intensity)
    {
        shakeDuration = duration;
        shakeAmount = intensity;
    }
}
