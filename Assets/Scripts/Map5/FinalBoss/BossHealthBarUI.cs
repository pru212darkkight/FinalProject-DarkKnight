using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    public Image fillImage;    // Kéo BossHealthFill vào slot này
    public EnemyHealth boss;   // Kéo object Boss (EnemyHealth) vào slot này

    void Start()
    {
        gameObject.SetActive(false); // Ẩn lúc chưa vào phòng
    }

    void Update()
    {
        if (boss != null && fillImage != null)
        {
            fillImage.fillAmount = boss.currentHealth / boss.maxHealth;
            if (boss.isDead)
            {
                HideBar();
            }
        }
    }

    public void ShowBar()
    {
        gameObject.SetActive(true);
    }

    public void HideBar()
    {
        gameObject.SetActive(false);
    }
}
