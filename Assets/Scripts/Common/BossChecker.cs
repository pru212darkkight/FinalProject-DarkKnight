using UnityEngine;

public class BossChecker : MonoBehaviour
{
    public GameObject[] bosses;            // Kéo tất cả boss vào array này
    public GameManager gameManager;
    private bool victoryShown = false;

    void Update()
    {
        if (victoryShown) return;

        bool allDead = true;
        foreach (var boss in bosses)
        {
            if (boss != null)
            {
                allDead = false;
                break;
            }
        }
        if (allDead)
        {
            victoryShown = true;
            gameManager.OnBossDefeated();
        }
    }
}
