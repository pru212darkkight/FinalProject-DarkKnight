using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarManager : MonoBehaviour
{
    [Header("Health Bar Settings")]
    public GameObject healthBarPrefab; // Kéo EnemyHealthBar prefab vào đây
    public bool autoCreateHealthBars = true;
    public bool useWorldSpaceCanvas = true;
    
    [Header("Health Bar Position")]
    public Vector3 healthBarOffset = new Vector3(0, 1.5f, 0);
    public Vector2 healthBarSize = new Vector2(2f, 0.3f);
    
    [Header("Colors")]
    public Color healthColor = new Color(0.8f, 0.2f, 0.2f, 1f);
    public Color backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    
    [Header("Debug")]
    public bool showDebug = true;
    
    private Canvas worldCanvas;
    
    void Start()
    {
        if (autoCreateHealthBars)
        {
            CreateHealthBarsForAllEnemies();
        }
    }
    
    [ContextMenu("Create Health Bars for All Enemies")]
    public void CreateHealthBarsForAllEnemies()
    {
        if (showDebug) Debug.Log("EnemyHealthBarManager: Creating health bars for all enemies...");
        
        // Create world space canvas if needed
        if (useWorldSpaceCanvas)
        {
            CreateWorldSpaceCanvas();
        }
        
        // Find all fish enemies
        UnderwaterEnemyHealth[] fishEnemies = FindObjectsOfType<UnderwaterEnemyHealth>();
        foreach (UnderwaterEnemyHealth fish in fishEnemies)
        {
            CreateHealthBarForEnemy(fish.gameObject, fish);
        }
        
        // Find all mine enemies
        UnderwaterMine[] mineEnemies = FindObjectsOfType<UnderwaterMine>();
        foreach (UnderwaterMine mine in mineEnemies)
        {
            CreateHealthBarForEnemy(mine.gameObject, mine);
        }
        
        if (showDebug) Debug.Log($"EnemyHealthBarManager: Created health bars for {fishEnemies.Length} fish and {mineEnemies.Length} mines");
    }
    
    private void CreateWorldSpaceCanvas()
    {
        if (worldCanvas != null) return;
        
        GameObject canvasObj = new GameObject("EnemyHealthBarsCanvas");
        worldCanvas = canvasObj.AddComponent<Canvas>();
        worldCanvas.renderMode = RenderMode.WorldSpace;
        worldCanvas.worldCamera = Camera.main;
        worldCanvas.sortingOrder = 10;
        
        // Set canvas size and position
        RectTransform canvasRect = worldCanvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(100, 100);
        canvasRect.localScale = Vector3.one * 0.01f; // Scale down for world space
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        if (showDebug) Debug.Log("EnemyHealthBarManager: World space canvas created");
    }
    
    private void CreateHealthBarForEnemy(GameObject enemy, MonoBehaviour healthComponent)
    {
        // Check if enemy already has health bar
        if (enemy.GetComponentInChildren<Image>() != null)
        {
            if (showDebug) Debug.Log($"EnemyHealthBarManager: {enemy.name} already has health bar, skipping");
            return;
        }
        
        GameObject healthBarObj;
        
        if (healthBarPrefab != null)
        {
            // Use prefab
            healthBarObj = Instantiate(healthBarPrefab);
        }
        else
        {
            // Create manually
            healthBarObj = CreateHealthBarManually();
        }
        
        // Set parent
        if (useWorldSpaceCanvas && worldCanvas != null)
        {
            healthBarObj.transform.SetParent(worldCanvas.transform, false);
        }
        else
        {
            healthBarObj.transform.SetParent(enemy.transform, false);
        }
        
        // Position health bar
        healthBarObj.transform.localPosition = healthBarOffset;
        
        // Setup health bar follow script
        HealthBarFollow followScript = healthBarObj.GetComponent<HealthBarFollow>();
        if (followScript == null)
        {
            followScript = healthBarObj.AddComponent<HealthBarFollow>();
        }
        
        followScript.target = enemy.transform;
        followScript.offset = healthBarOffset;
        followScript.followTarget = true;
        followScript.billboardToCamera = true;
        followScript.mainCamera = Camera.main;
        
        // Setup health bar updater
        EnemyHealthBarUpdater updater = healthBarObj.GetComponent<EnemyHealthBarUpdater>();
        if (updater == null)
        {
            updater = healthBarObj.AddComponent<EnemyHealthBarUpdater>();
        }
        
        Image healthBarImage = healthBarObj.GetComponent<Image>();
        if (healthBarImage != null)
        {
            healthBarImage.color = healthColor;
            updater.healthBarImage = healthBarImage;
        }
        
        // Connect to health component
        if (healthComponent is UnderwaterEnemyHealth)
        {
            updater.fishHealth = (UnderwaterEnemyHealth)healthComponent;
        }
        else if (healthComponent is UnderwaterMine)
        {
            updater.mineHealth = (UnderwaterMine)healthComponent;
        }
        
        if (showDebug) Debug.Log($"EnemyHealthBarManager: Created health bar for {enemy.name}");
    }
    
    private GameObject CreateHealthBarManually()
    {
        GameObject healthBarObj = new GameObject("EnemyHealthBar");
        
        // Add RectTransform
        RectTransform rectTransform = healthBarObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = healthBarSize;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        
        // Add CanvasRenderer
        healthBarObj.AddComponent<CanvasRenderer>();
        
        // Add Image component
        Image healthBarImage = healthBarObj.AddComponent<Image>();
        healthBarImage.color = healthColor;
        healthBarImage.type = Image.Type.Filled;
        healthBarImage.fillMethod = Image.FillMethod.Horizontal;
        
        return healthBarObj;
    }
    
    // Method to manually create health bar for specific enemy
    public void CreateHealthBarForSpecificEnemy(GameObject enemy)
    {
        UnderwaterEnemyHealth fishHealth = enemy.GetComponent<UnderwaterEnemyHealth>();
        UnderwaterMine mineHealth = enemy.GetComponent<UnderwaterMine>();
        
        if (fishHealth != null)
        {
            CreateHealthBarForEnemy(enemy, fishHealth);
        }
        else if (mineHealth != null)
        {
            CreateHealthBarForEnemy(enemy, mineHealth);
        }
        else
        {
            Debug.LogWarning($"EnemyHealthBarManager: {enemy.name} doesn't have UnderwaterEnemyHealth or UnderwaterMine component");
        }
    }
}
