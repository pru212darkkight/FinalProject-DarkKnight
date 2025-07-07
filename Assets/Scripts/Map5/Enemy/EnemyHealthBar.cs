using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Image fillImage;         // Gán Image cho phần fill (thanh đỏ)
    public EnemyHealth enemyHealth; // Gán script máu của quái

    void Update()
    {
        if (enemyHealth != null && fillImage != null)
        {
            fillImage.fillAmount = (float)enemyHealth.currentHealth / enemyHealth.maxHealth;
        }
    }
}
