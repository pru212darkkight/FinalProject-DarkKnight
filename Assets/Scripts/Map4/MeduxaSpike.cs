using UnityEngine;
using System.Collections;

public class MeduxaSpike : MonoBehaviour
{
    public GameObject[] trapObjects;
    public float damageAmount = 10f;
    public bool isMagicDamage = true;
    public float delayBeforeDamage = 0.5f;

    private bool damageActivated = false; // cho biết đã gây damage hay chưa
    private Collider2D damageZone;

    void Start()
    {
        foreach (var trap in trapObjects)
            trap.SetActive(false);

        damageZone = GetComponent<Collider2D>();
        if (damageZone != null)
            damageZone.enabled = false; // ban đầu tắt trigger
    }

    public void ActivateAllTraps()
    {
        foreach (GameObject trap in trapObjects)
        {
            trap.SetActive(true);

            Animator anim = trap.GetComponent<Animator>();
            if (anim != null)
                anim.Play(0);
        }

        StartCoroutine(DelayDamage());
    }

    IEnumerator DelayDamage()
    {
        yield return new WaitForSeconds(delayBeforeDamage);

        damageActivated = true;

        if (damageZone != null)
            damageZone.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!damageActivated) return;

        if (other.CompareTag("Player"))
        {
            PlayerController1 player = other.GetComponent<PlayerController1>();
            if (player != null)
            {
                player.TakeDamage(damageAmount, isMagicDamage,"Medusa");
                damageActivated = false; // chỉ gây 1 lần
                Debug.Log("💥 Spike dealt delayed damage to player!");
            }
        }
    }
}
