using UnityEngine;
using UnityEngine.UI;

public class Map5UICreator : MonoBehaviour
{
    [Header("UI Settings")]
    public bool createUIOnStart = true;
    public Vector2 healthBarPosition = new Vector2(50, -50);
    public Vector2 staminaBarPosition = new Vector2(50, -90);
    public Vector2 manaBarPosition = new Vector2(50, -130);
    public Vector2 barSize = new Vector2(200, 25);
    
    [Header("Map 5 Sprites")]
    public Sprite healthBarBackground;
    public Sprite healthBarFill;
    public Sprite staminaBarBackground;
    public Sprite staminaBarFill;
    public Sprite manaBarBackground;
    public Sprite manaBarFill;
    
    [Header("Colors")]
    public Color healthColor = new Color(0.8f, 0.2f, 0.2f, 1f); // Dark red
    public Color staminaColor = new Color(1f, 0.8f, 0.2f, 1f);  // Golden yellow
    public Color manaColor = new Color(0.2f, 0.4f, 0.8f, 1f);   // Dark blue
    
    [Header("References")]
    public Canvas targetCanvas;
    
    private PlayerUIManager uiManager;
    
    void Start()
    {
        if (createUIOnStart)
        {
            LoadMap5Sprites();
            CreateMap5StyleUI();
        }
    }
    
    [ContextMenu("Load Map 5 Sprites")]
    public void LoadMap5Sprites()
    {
        // Load sprites from Map 5 assets
        // Health bar sprites
        Sprite[] healthSprites = Resources.LoadAll<Sprite>("Maps/Map 5 - Hau/Enemies/health_bar");
        if (healthSprites.Length > 0)
        {
            healthBarFill = healthSprites[0];
        }
        
        Sprite[] emptySprites = Resources.LoadAll<Sprite>("Maps/Map 5 - Hau/Enemies/empty_bar");
        if (emptySprites.Length > 0)
        {
            healthBarBackground = emptySprites[0];
            staminaBarBackground = emptySprites[0];
            manaBarBackground = emptySprites[0];
        }
        
        // Use the same fill sprite for all bars but with different colors
        staminaBarFill = healthBarFill;
        manaBarFill = healthBarFill;
        
        Debug.Log("Map 5 sprites loaded!");
    }
    
    [ContextMenu("Create Map 5 Style UI")]
    public void CreateMap5StyleUI()
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
        GameObject uiManagerObj = new GameObject("PlayerUIManager_Map5Style");
        uiManagerObj.transform.SetParent(targetCanvas.transform);
        uiManager = uiManagerObj.AddComponent<PlayerUIManager>();
        
        // Create Health Bar with Map 5 style
        Image healthBar = CreateMap5Bar("HealthBar", healthBarPosition, healthColor, healthBarBackground, healthBarFill);
        uiManager.healthBar = healthBar;
        
        // Create Stamina Bar with Map 5 style
        Image staminaBar = CreateMap5Bar("StaminaBar", staminaBarPosition, staminaColor, staminaBarBackground, staminaBarFill);
        uiManager.staminaBar = staminaBar;
        
        // Create Mana Bar with Map 5 style
        Image manaBar = CreateMap5Bar("ManaBar", manaBarPosition, manaColor, manaBarBackground, manaBarFill);
        uiManager.manaBar = manaBar;
        
        Debug.Log("Map 5 style Player UI created successfully!");
    }
    
    private void CreateCanvas()
    {
        GameObject canvasObj = new GameObject("PlayerUICanvas_Map5");
        targetCanvas = canvasObj.AddComponent<Canvas>();
        targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        targetCanvas.sortingOrder = 10;
        
        // Add Canvas Scaler
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        // Add Graphic Raycaster
        canvasObj.AddComponent<GraphicRaycaster>();
        
        Debug.Log("Canvas created for Map 5 style Player UI");
    }
    
    private Image CreateMap5Bar(string name, Vector2 position, Color fillColor, Sprite bgSprite, Sprite fillSprite)
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
        if (bgSprite != null)
        {
            bgImage.sprite = bgSprite;
            bgImage.type = Image.Type.Sliced; // For better scaling
        }
        else
        {
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f); // Dark background
        }
        
        // Create fill
        GameObject fillObj = new GameObject(name + "_Fill");
        fillObj.transform.SetParent(backgroundObj.transform);
        
        RectTransform fillRect = fillObj.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(2, 2); // Small padding
        fillRect.offsetMax = new Vector2(-2, -2);
        
        Image fillImage = fillObj.AddComponent<Image>();
        if (fillSprite != null)
        {
            fillImage.sprite = fillSprite;
            fillImage.type = Image.Type.Sliced;
        }
        fillImage.color = fillColor;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        
        // Create label with better styling
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
        labelText.fontSize = 12;
        labelText.color = Color.white;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.fontStyle = FontStyle.Bold;
        
        // Add outline for better readability
        Outline outline = labelObj.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(1, 1);
        
        return fillImage;
    }
    
    [ContextMenu("Remove Map 5 UI")]
    public void RemoveMap5UI()
    {
        if (uiManager != null)
        {
            DestroyImmediate(uiManager.gameObject);
            Debug.Log("Map 5 style Player UI removed");
        }
    }
    
    // Method to manually assign sprites if auto-load fails
    public void SetSprites(Sprite healthBg, Sprite healthFill, Sprite staminaBg, Sprite staminaFill, Sprite manaBg, Sprite manaFill)
    {
        healthBarBackground = healthBg;
        healthBarFill = healthFill;
        staminaBarBackground = staminaBg;
        staminaBarFill = staminaFill;
        manaBarBackground = manaBg;
        manaBarFill = manaFill;
    }
}
