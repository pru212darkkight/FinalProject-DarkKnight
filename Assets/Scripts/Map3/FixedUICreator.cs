using UnityEngine;
using UnityEngine.UI;

public class FixedUICreator : MonoBehaviour
{
    [Header("Create Fixed UI")]
    public bool createOnStart = true;
    public bool replaceExistingUI = true;
    
    [Header("UI Position (Screen Space)")]
    public Vector2 uiPosition = new Vector2(50, -50); // Top-left corner
    public Vector2 barSize = new Vector2(200, 25);
    public float barSpacing = 30f;
    
    [Header("UI Style - Map 5 Exact Colors")]
    public Color healthColor = new Color(0.1f, 0.9f, 0.1f, 1f); // Bright green like Map 5
    public Color manaColor = new Color(0.1f, 0.5f, 1f, 1f);     // Bright blue
    public Color staminaColor = new Color(1f, 0.9f, 0.1f, 1f);  // Bright yellow
    public Color backgroundColor = new Color(0, 0, 0, 0.8f);

    [Header("Map 5 Style Settings")]
    public bool useMap5ExactStyle = true;
    public float barBorderWidth = 1f;
    public Color borderColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    
    [Header("Avatar")]
    public bool includeAvatar = true;
    public Vector2 avatarSize = new Vector2(60, 60);
    public Sprite playerAvatarSprite;
    
    private Canvas fixedCanvas;
    private GameObject fixedUIContainer;
    private PlayerUIManager fixedUIManager;
    
    void Start()
    {
        Debug.Log("FixedUICreator: Starting...");
        if (createOnStart)
        {
            CreateFixedUI();
        }
    }
    
    [ContextMenu("Create Fixed UI")]
    public void CreateFixedUI()
    {
        Debug.Log("FixedUICreator: Creating Map 5 style fixed UI...");

        // Remove existing UI if requested
        if (replaceExistingUI)
        {
            RemoveExistingPlayerUI();
        }

        // Create Screen Space Canvas
        CreateFixedCanvas();

        // Create UI Container
        CreateUIContainer();

        // Create Avatar (if enabled)
        if (includeAvatar)
        {
            CreatePlayerAvatar();
        }

        // Create bars with Map 5 exact style
        Image healthBar, manaBar, staminaBar;

        if (useMap5ExactStyle)
        {
            healthBar = CreateMap5StyleBar("Health", new Vector2(0, 0), healthColor);
            manaBar = CreateMap5StyleBar("Mana", new Vector2(0, -barSpacing), manaColor);
            staminaBar = CreateMap5StyleBar("Stamina", new Vector2(0, -barSpacing * 2), staminaColor);
        }
        else
        {
            healthBar = CreateFixedBar("HealthBar", new Vector2(0, 0), healthColor);
            manaBar = CreateFixedBar("ManaBar", new Vector2(0, -barSpacing), manaColor);
            staminaBar = CreateFixedBar("StaminaBar", new Vector2(0, -barSpacing * 2), staminaColor);
        }

        // Setup UI Manager
        SetupFixedUIManager(healthBar, manaBar, staminaBar);

        Debug.Log("FixedUICreator: Map 5 style UI created successfully!");
        Debug.Log($"UI Position: {uiPosition}");
        Debug.Log($"Canvas Render Mode: {fixedCanvas.renderMode}");
    }
    
    private void RemoveExistingPlayerUI()
    {
        // Find and remove existing UI
        PlayerUIManager[] existingManagers = FindObjectsOfType<PlayerUIManager>();
        foreach (PlayerUIManager manager in existingManagers)
        {
            if (manager.gameObject.name.Contains("PlayerUI") || 
                manager.gameObject.name.Contains("ExtractedUI"))
            {
                Debug.Log($"Removing existing UI: {manager.gameObject.name}");
                DestroyImmediate(manager.gameObject);
            }
        }
        
        // Also remove any Canvas that might be following player
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            if (canvas.renderMode == RenderMode.WorldSpace && 
                canvas.gameObject.name.Contains("PlayerUI"))
            {
                Debug.Log($"Removing world space UI canvas: {canvas.gameObject.name}");
                DestroyImmediate(canvas.gameObject);
            }
        }
    }
    
    private void CreateFixedCanvas()
    {
        GameObject canvasObj = new GameObject("FixedPlayerUICanvas");
        fixedCanvas = canvasObj.AddComponent<Canvas>();
        
        // IMPORTANT: Screen Space Overlay for fixed position
        fixedCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fixedCanvas.sortingOrder = 100; // High priority to be on top
        
        // Canvas Scaler for responsive UI
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        // Graphic Raycaster for UI interaction
        canvasObj.AddComponent<GraphicRaycaster>();
        
        Debug.Log("FixedUICreator: Screen Space Canvas created");
    }
    
    private void CreateUIContainer()
    {
        fixedUIContainer = new GameObject("FixedPlayerUI");
        fixedUIContainer.transform.SetParent(fixedCanvas.transform, false);
        
        RectTransform containerRect = fixedUIContainer.AddComponent<RectTransform>();
        
        // Anchor to top-left corner
        containerRect.anchorMin = new Vector2(0, 1);
        containerRect.anchorMax = new Vector2(0, 1);
        containerRect.pivot = new Vector2(0, 1);
        
        // Set position from top-left
        containerRect.anchoredPosition = uiPosition;
        containerRect.sizeDelta = new Vector2(barSize.x + 20, (barSpacing * 3) + 20);
        
        Debug.Log($"FixedUICreator: UI Container created at {uiPosition}");
    }
    
    private void CreatePlayerAvatar()
    {
        GameObject avatarObj = new GameObject("PlayerAvatar");
        avatarObj.transform.SetParent(fixedUIContainer.transform, false);
        
        RectTransform avatarRect = avatarObj.AddComponent<RectTransform>();
        avatarRect.anchorMin = new Vector2(0, 1);
        avatarRect.anchorMax = new Vector2(0, 1);
        avatarRect.pivot = new Vector2(0, 1);
        avatarRect.anchoredPosition = new Vector2(-70, 0); // Left of bars
        avatarRect.sizeDelta = avatarSize;
        
        Image avatarImage = avatarObj.AddComponent<Image>();
        
        // Try to find player sprite if not assigned
        if (playerAvatarSprite == null)
        {
            PlayerController1 player = FindObjectOfType<PlayerController1>();
            if (player != null)
            {
                SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
                if (playerSprite != null)
                {
                    playerAvatarSprite = playerSprite.sprite;
                }
            }
        }
        
        if (playerAvatarSprite != null)
        {
            avatarImage.sprite = playerAvatarSprite;
            avatarImage.color = Color.white;
        }
        else
        {
            // Default avatar (colored circle)
            avatarImage.color = new Color(0.8f, 0.6f, 0.4f, 1f); // Skin color
        }
        
        // Add border
        GameObject borderObj = new GameObject("AvatarBorder");
        borderObj.transform.SetParent(avatarObj.transform, false);
        
        RectTransform borderRect = borderObj.AddComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = new Vector2(-2, -2);
        borderRect.offsetMax = new Vector2(2, 2);
        
        Image borderImage = borderObj.AddComponent<Image>();
        borderImage.color = new Color(0.8f, 0.8f, 0.8f, 1f);
        
        Debug.Log("FixedUICreator: Player avatar created");
    }
    
    private Image CreateFixedBar(string barName, Vector2 localPosition, Color barColor)
    {
        // Background
        GameObject bgObj = new GameObject(barName + "_Background");
        bgObj.transform.SetParent(fixedUIContainer.transform, false);
        
        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 1);
        bgRect.anchorMax = new Vector2(0, 1);
        bgRect.pivot = new Vector2(0, 1);
        bgRect.anchoredPosition = localPosition;
        bgRect.sizeDelta = barSize;
        
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = backgroundColor;
        
        // Fill (the actual bar)
        GameObject fillObj = new GameObject(barName + "_Fill");
        fillObj.transform.SetParent(bgObj.transform, false);
        
        RectTransform fillRect = fillObj.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(2, 2); // Small padding
        fillRect.offsetMax = new Vector2(-2, -2);
        
        Image fillImage = fillObj.AddComponent<Image>();
        fillImage.color = barColor;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        
        // Label
        GameObject labelObj = new GameObject(barName + "_Label");
        labelObj.transform.SetParent(bgObj.transform, false);
        
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        
        Text labelText = labelObj.AddComponent<Text>();
        labelText.text = barName.Replace("Bar", "");
        labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        labelText.fontSize = 12;
        labelText.color = Color.white;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.fontStyle = FontStyle.Bold;
        
        Debug.Log($"FixedUICreator: Created {barName} at {localPosition}");
        return fillImage;
    }

    private Image CreateMap5StyleBar(string barName, Vector2 localPosition, Color barColor)
    {
        // Outer border (Map 5 style)
        GameObject borderObj = new GameObject(barName + "_Border");
        borderObj.transform.SetParent(fixedUIContainer.transform, false);

        RectTransform borderRect = borderObj.AddComponent<RectTransform>();
        borderRect.anchorMin = new Vector2(0, 1);
        borderRect.anchorMax = new Vector2(0, 1);
        borderRect.pivot = new Vector2(0, 1);
        borderRect.anchoredPosition = localPosition;
        borderRect.sizeDelta = new Vector2(barSize.x + 4, barSize.y + 4); // Slightly larger for border

        Image borderImage = borderObj.AddComponent<Image>();
        borderImage.color = borderColor;

        // Background (dark)
        GameObject bgObj = new GameObject(barName + "_Background");
        bgObj.transform.SetParent(borderObj.transform, false);

        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = new Vector2(barBorderWidth, barBorderWidth);
        bgRect.offsetMax = new Vector2(-barBorderWidth, -barBorderWidth);

        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f); // Very dark background

        // Fill (the actual colored bar)
        GameObject fillObj = new GameObject(barName + "_Fill");
        fillObj.transform.SetParent(bgObj.transform, false);

        RectTransform fillRect = fillObj.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(1, 1); // Small inner padding
        fillRect.offsetMax = new Vector2(-1, -1);

        Image fillImage = fillObj.AddComponent<Image>();
        fillImage.color = barColor;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;

        // Shine effect (Map 5 style)
        GameObject shineObj = new GameObject(barName + "_Shine");
        shineObj.transform.SetParent(fillObj.transform, false);

        RectTransform shineRect = shineObj.AddComponent<RectTransform>();
        shineRect.anchorMin = new Vector2(0, 0.7f);
        shineRect.anchorMax = new Vector2(1, 1);
        shineRect.offsetMin = Vector2.zero;
        shineRect.offsetMax = Vector2.zero;

        Image shineImage = shineObj.AddComponent<Image>();
        shineImage.color = new Color(1f, 1f, 1f, 0.3f); // White shine

        // Text label (smaller, cleaner)
        GameObject labelObj = new GameObject(barName + "_Label");
        labelObj.transform.SetParent(bgObj.transform, false);

        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        Text labelText = labelObj.AddComponent<Text>();
        labelText.text = barName;
        labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        labelText.fontSize = 10;
        labelText.color = Color.white;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.fontStyle = FontStyle.Bold;

        // Add shadow effect
        Shadow shadow = labelObj.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.8f);
        shadow.effectDistance = new Vector2(1, -1);

        Debug.Log($"FixedUICreator: Created Map 5 style {barName} at {localPosition}");
        return fillImage;
    }
    
    private void SetupFixedUIManager(Image healthBar, Image manaBar, Image staminaBar)
    {
        fixedUIManager = fixedUIContainer.AddComponent<PlayerUIManager>();
        fixedUIManager.healthBar = healthBar;
        fixedUIManager.manaBar = manaBar;
        fixedUIManager.staminaBar = staminaBar;
        fixedUIManager.autoFindPlayer = true;
        
        Debug.Log("FixedUICreator: UI Manager setup completed");
        Debug.Log("Fixed UI will NOT follow player - it stays in screen corner!");
    }
    
    [ContextMenu("Remove Fixed UI")]
    public void RemoveFixedUI()
    {
        if (fixedCanvas != null)
        {
            DestroyImmediate(fixedCanvas.gameObject);
            Debug.Log("Fixed UI removed");
        }
    }
    
    [ContextMenu("Test UI Position")]
    public void TestUIPosition()
    {
        if (fixedUIContainer != null)
        {
            RectTransform rect = fixedUIContainer.GetComponent<RectTransform>();
            Debug.Log($"UI Container Position: {rect.anchoredPosition}");
            Debug.Log($"Canvas Render Mode: {fixedCanvas.renderMode}");
            Debug.Log($"Canvas Sorting Order: {fixedCanvas.sortingOrder}");
        }
    }
}
