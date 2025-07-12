using UnityEngine;

[System.Serializable]
public class EnemyTypeSettings
{
    public string enemyName;
    public float maxHealth = 4f;
    public Vector3 healthBarOffset = new Vector3(0, 1f, 0);
    public Color healthBarColor = Color.red;
    public GameObject deathEffect;
}

public class UnderwaterEnemySetup : MonoBehaviour
{
    [Header("Enemy Type Settings")]
    public EnemyTypeSettings fishBigSettings = new EnemyTypeSettings 
    { 
        enemyName = "FishBig", 
        maxHealth = 4f, 
        healthBarOffset = new Vector3(0, 1.2f, 0),
        healthBarColor = Color.red
    };
    
    public EnemyTypeSettings fishMidSettings = new EnemyTypeSettings 
    { 
        enemyName = "FishMid", 
        maxHealth = 3f, 
        healthBarOffset = new Vector3(0, 1f, 0),
        healthBarColor = Color.red
    };
    
    public EnemyTypeSettings fishDartSettings = new EnemyTypeSettings 
    { 
        enemyName = "FishDart", 
        maxHealth = 2f, 
        healthBarOffset = new Vector3(0, 0.8f, 0),
        healthBarColor = Color.red
    };

    [Header("Mine Settings")]
    public float mineHealth = 2f;
    public float mineExplosionDamage = 5f;
    public float mineExplosionRadius = 2f;
    public Vector3 mineHealthBarOffset = new Vector3(0, 1f, 0);
    public Color mineHealthBarColor = Color.yellow;

    [Header("Setup Options")]
    public bool setupOnStart = true;
    public bool debugMode = true;

    private void Start()
    {
        if (setupOnStart)
        {
            // Debug trước khi setup
            DebugListAllFishObjects();
            SetupAllUnderwaterEnemies();
        }
    }

    [ContextMenu("Setup All Underwater Enemies")]
    public void SetupAllUnderwaterEnemies()
    {
        SetupFishEnemies();
        SetupMines();
        SetupPlayerAttack();
        
        if (debugMode)
        {
            Debug.Log("Underwater enemy setup completed!");
        }
    }

    void SetupFishEnemies()
    {
        // Setup FishBig
        GameObject[] fishBigs = GameObject.FindGameObjectsWithTag("Untagged");
        foreach (GameObject obj in fishBigs)
        {
            if (obj.name.Contains("FishBig"))
            {
                SetupFishEnemy(obj, fishBigSettings);
            }
        }

        // Setup FishMid
        foreach (GameObject obj in fishBigs)
        {
            if (obj.name.Contains("FishMid"))
            {
                SetupFishEnemy(obj, fishMidSettings);
            }
        }

        // Setup FishDart
        foreach (GameObject obj in fishBigs)
        {
            if (obj.name.Contains("FishDart"))
            {
                SetupFishEnemy(obj, fishDartSettings);
            }
        }
    }

    void SetupFishEnemy(GameObject fishObj, EnemyTypeSettings settings)
    {
        // Thêm Map3EnemyDestroyer để tự động destroy khi chết
        Map3EnemyDestroyer destroyer = fishObj.GetComponent<Map3EnemyDestroyer>();
        if (destroyer == null)
        {
            destroyer = fishObj.AddComponent<Map3EnemyDestroyer>();
        }

        // Đảm bảo có Collider2D và set trigger
        Collider2D col = fishObj.GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        // Đảm bảo có tag phù hợp (có thể tạo tag "Enemy" nếu cần)
        if (fishObj.tag == "Untagged")
        {
            // fishObj.tag = "Enemy";  // Uncomment nếu có tag Enemy
        }

        if (debugMode)
        {
            Debug.Log($"Setup {settings.enemyName} with Map3EnemyDestroyer: Health={settings.maxHealth}, Offset={settings.healthBarOffset}");
        }
    }

    void SetupMines()
    {
        // Tìm tất cả mine objects
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("Untagged");
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Mine"))
            {
                SetupMineEnemy(obj);
            }
        }
    }

    void SetupMineEnemy(GameObject mineObj)
    {
        // Kiểm tra xem đã có UnderwaterMine chưa
        UnderwaterMine mine = mineObj.GetComponent<UnderwaterMine>();
        if (mine == null)
        {
            mine = mineObj.AddComponent<UnderwaterMine>();
        }

        // Apply settings
        mine.maxHealth = mineHealth;
        mine.currentHealth = mineHealth;
        mine.explosionDamage = mineExplosionDamage;
        mine.explosionRadius = mineExplosionRadius;
        mine.healthBarOffset = mineHealthBarOffset;

        // Đảm bảo có Collider2D và set trigger
        Collider2D col = mineObj.GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        if (debugMode)
        {
            Debug.Log($"Setup Mine: Health={mineHealth}, ExplosionDamage={mineExplosionDamage}, Radius={mineExplosionRadius}");
        }
    }

    void SetupPlayerAttack()
    {
        // Tìm player và thêm UnderwaterPlayerAttack
        PlayerController1 player = FindObjectOfType<PlayerController1>();
        if (player != null)
        {
            UnderwaterPlayerAttack playerAttack = player.GetComponent<UnderwaterPlayerAttack>();
            if (playerAttack == null)
            {
                playerAttack = player.gameObject.AddComponent<UnderwaterPlayerAttack>();
            }

            // Set default attack settings
            playerAttack.attackRange = 1.5f;
            playerAttack.attackCooldown = 0.5f;
            playerAttack.attackOffset = new Vector2(0.8f, 0f);  // Attack phía trước player

            if (debugMode)
            {
                Debug.Log("Setup Player Attack System");
            }
        }
        else
        {
            Debug.LogWarning("PlayerController1 not found! Cannot setup player attack.");
        }
    }

    [ContextMenu("Remove All Underwater Enemy Components")]
    public void RemoveAllUnderwaterEnemyComponents()
    {
        // Remove UnderwaterEnemyHealth components
        UnderwaterEnemyHealth[] enemyHealths = FindObjectsOfType<UnderwaterEnemyHealth>();
        foreach (UnderwaterEnemyHealth health in enemyHealths)
        {
            DestroyImmediate(health);
        }

        // Remove UnderwaterMine components
        UnderwaterMine[] mines = FindObjectsOfType<UnderwaterMine>();
        foreach (UnderwaterMine mine in mines)
        {
            DestroyImmediate(mine);
        }

        // Remove UnderwaterPlayerAttack components
        UnderwaterPlayerAttack[] playerAttacks = FindObjectsOfType<UnderwaterPlayerAttack>();
        foreach (UnderwaterPlayerAttack attack in playerAttacks)
        {
            DestroyImmediate(attack);
        }

        Debug.Log("Removed all underwater enemy components!");
    }

    [ContextMenu("Debug: List All Fish Objects")]
    public void DebugListAllFishObjects()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int fishCount = 0;
        int mineCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Fish"))
            {
                Debug.Log($"Found Fish: {obj.name} at {obj.transform.position}");
                fishCount++;
            }
            else if (obj.name.Contains("Mine"))
            {
                Debug.Log($"Found Mine: {obj.name} at {obj.transform.position}");
                mineCount++;
            }
        }

        Debug.Log($"Total Fish: {fishCount}, Total Mines: {mineCount}");
    }
}
