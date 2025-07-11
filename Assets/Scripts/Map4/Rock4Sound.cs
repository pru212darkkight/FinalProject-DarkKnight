using UnityEngine;

public class Rock4Sound : MonoBehaviour
{
    public AudioClip soundEffect;
    public string rockTag = "Rock"; 
    private AudioSource audioSource;
    private bool hasPlayed = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = soundEffect;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasPlayed && other.CompareTag(rockTag))
        {
            audioSource.Play();
            hasPlayed = true;
        }
    }
}
