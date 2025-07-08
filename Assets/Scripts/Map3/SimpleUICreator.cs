using UnityEngine;
using UnityEngine.UI;

public class SimpleUICreator : MonoBehaviour
{
    [Header("Create UI")]
    public bool createOnStart = true;
    
    [Header("UI Settings")]
    public Vector2 uiPosition = new Vector2(50, -50);
    public Vector2 barSize = new Vector2(200, 25);
    
    void Start()
    {
        Debug.Log("SimpleUICreator: Starting...");
        if (createOnStart)
        {
            CreateSimpleUI();
        }
    }
    
    [ContextMenu("Create Simple UI")]
    public void CreateSimpleUI()
    {
        Debug.Log("SimpleUICreator: Creating UI...");
        
        // Find or create Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.Log("SimpleUICreator: Creating new Canvas...");
            CreateCanvas();
            canvas = FindObjectOfType<Canvas>();
        }
        else
        {
            Debug.Log($"SimpleUICreator: Using existing Canvas: {canvas.name}");
        }
        
        // Create UI Container
        GameObject uiContainer = new GameObject("PlayerUI_Simple");
        uiContainer.transform.SetParent(canvas.transform);
        
        RectTransform containerRect = uiContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0, 1);
        containerRect.anchorMax = new Vector2(0, 1);
        containerRect.anchoredPosition = uiPosition;
        containerRect.sizeDelta = new Vector2(250, 100);
        
        // Create Health Bar
        Image healthBar = CreateBar(uiContainer, "HealthBar", new Vector2(0, 0), Color.red);
        
        // Create Mana Bar  
        Image manaBar = CreateBar(uiContainer, "ManaBar", new Vector2(0, -30), Color.blue);
        
        // Add PlayerUIManager
        PlayerUIManager uiManager = uiContainer.AddComponent<PlayerUIManager>();
        uiManager.healthBar = healthBar;
        uiManager.manaBar = manaBar;
        
        Debug.Log("SimpleUICreator: UI created successfully!");
        Debug.Log($"Health Bar: {healthBar.name}");
        Debug.Log($"Mana Bar: {manaBar.name}");
    }
    
    private void CreateCanvas()
    {
        GameObject canvasObj = new GameObject("PlayerUICanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        Debug.Log("SimpleUICreator: Canvas created");
    }
    
    private Image CreateBar(GameObject parent, string name, Vector2 position, Color color)
    {
        // Background
        GameObject bgObj = new GameObject(name + "_Background");
        bgObj.transform.SetParent(parent.transform);
        
        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 1);
        bgRect.anchorMax = new Vector2(0, 1);
        bgRect.anchoredPosition = position;
        bgRect.sizeDelta = barSize;
        
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.5f);
        
        // Fill
        GameObject fillObj = new GameObject(name + "_Fill");
        fillObj.transform.SetParent(bgObj.transform);
        
        RectTransform fillRect = fillObj.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        
        Image fillImage = fillObj.AddComponent<Image>();
        fillImage.color = color;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        
        // Label
        GameObject labelObj = new GameObject(name + "_Label");
        labelObj.transform.SetParent(bgObj.transform);
        
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        
        Text labelText = labelObj.AddComponent<Text>();
        labelText.text = name.Replace("Bar", "");
        labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        labelText.fontSize = 14;
        labelText.color = Color.white;
        labelText.alignment = TextAnchor.MiddleCenter;
        
        Debug.Log($"SimpleUICreator: Created {name}");
        return fillImage;
    }
}
