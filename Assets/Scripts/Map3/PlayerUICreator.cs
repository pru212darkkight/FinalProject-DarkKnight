using UnityEngine;
using UnityEngine.UI;

public class PlayerUICreator : MonoBehaviour
{
    [Header("UI Settings")]
    public bool createUIOnStart = true;
    public Vector2 healthBarPosition = new Vector2(50, -50);
    public Vector2 staminaBarPosition = new Vector2(50, -100);
    public Vector2 manaBarPosition = new Vector2(50, -150);
    public Vector2 barSize = new Vector2(200, 20);
    
    [Header("Colors")]
    public Color healthColor = Color.red;
    public Color staminaColor = Color.yellow;
    public Color manaColor = Color.blue;
    public Color backgroundColor = new Color(0, 0, 0, 0.5f);
    
    [Header("References")]
    public Canvas targetCanvas;
    
    private PlayerUIManager uiManager;
    
    void Start()
    {
        if (createUIOnStart)
        {
            CreatePlayerUI();
        }
    }
    
    [ContextMenu("Create Player UI")]
    public void CreatePlayerUI()
    {
        // Find or create Canvas
        if (targetCanvas == null)
        {
            targetCanvas = FindObjectOfType<Canvas>();
            if (targetCanvas == null)
            {
                CreateCanvas();
            }
        }
        
        // Create UI Manager GameObject
        GameObject uiManagerObj = new GameObject("PlayerUIManager");
        uiManagerObj.transform.SetParent(targetCanvas.transform);
        uiManager = uiManagerObj.AddComponent<PlayerUIManager>();
        
        // Create Health Bar
        Image healthBar = CreateBar("HealthBar", healthBarPosition, healthColor);
        uiManager.healthBar = healthBar;
        
        // Create Stamina Bar
        Image staminaBar = CreateBar("StaminaBar", staminaBarPosition, staminaColor);
        uiManager.staminaBar = staminaBar;
        
        // Create Mana Bar
        Image manaBar = CreateBar("ManaBar", manaBarPosition, manaColor);
        uiManager.manaBar = manaBar;
        
        Debug.Log("Player UI created successfully!");
    }
    
    private void CreateCanvas()
    {
        GameObject canvasObj = new GameObject("PlayerUICanvas");
        targetCanvas = canvasObj.AddComponent<Canvas>();
        targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        targetCanvas.sortingOrder = 10; // Ensure UI is on top
        
        // Add Canvas Scaler
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        // Add Graphic Raycaster
        canvasObj.AddComponent<GraphicRaycaster>();
        
        Debug.Log("Canvas created for Player UI");
    }
    
    private Image CreateBar(string name, Vector2 position, Color fillColor)
    {
        // Create background
        GameObject backgroundObj = new GameObject(name + "_Background");
        backgroundObj.transform.SetParent(targetCanvas.transform);
        
        RectTransform bgRect = backgroundObj.AddComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 1); // Top-left anchor
        bgRect.anchorMax = new Vector2(0, 1);
        bgRect.anchoredPosition = position;
        bgRect.sizeDelta = barSize;
        
        Image bgImage = backgroundObj.AddComponent<Image>();
        bgImage.color = backgroundColor;
        
        // Create fill
        GameObject fillObj = new GameObject(name + "_Fill");
        fillObj.transform.SetParent(backgroundObj.transform);
        
        RectTransform fillRect = fillObj.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        
        Image fillImage = fillObj.AddComponent<Image>();
        fillImage.color = fillColor;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        
        // Create label
        GameObject labelObj = new GameObject(name + "_Label");
        labelObj.transform.SetParent(backgroundObj.transform);
        
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
        
        return fillImage;
    }
    
    [ContextMenu("Remove Player UI")]
    public void RemovePlayerUI()
    {
        if (uiManager != null)
        {
            DestroyImmediate(uiManager.gameObject);
            Debug.Log("Player UI removed");
        }
    }
}
