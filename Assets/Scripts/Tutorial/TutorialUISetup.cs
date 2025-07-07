using UnityEngine;
using UnityEngine.UI;

public class TutorialUISetup : MonoBehaviour
{
    [Header("Tutorial UI Prefab")]
    public GameObject tutorialUIPrefab;
    
    [Header("UI Settings")]
    public Vector2 panelSize = new Vector2(400, 200);
    public Vector2 defaultPosition = new Vector2(0, 200);
    public Color backgroundColor = new Color(0, 0, 0, 0.8f);
    public Color textColor = Color.white;
    public Font textFont;
    public int fontSize = 16;
    
    [Header("Arrow Settings")]
    public Sprite arrowSprite;
    public Vector2 arrowSize = new Vector2(50, 50);
    public Color arrowColor = Color.yellow;
    
    [Header("Button Settings")]
    public string skipButtonText = "Skip Tutorial";
    public string nextButtonText = "Next";
    public Vector2 buttonSize = new Vector2(100, 30);
    
    void Start()
    {
        if (tutorialUIPrefab == null)
        {
            CreateTutorialUIPrefab();
        }
    }
    
    [ContextMenu("Create Tutorial UI")]
    public void CreateTutorialUIPrefab()
    {
        // Create main tutorial panel
        GameObject tutorialPanel = new GameObject("TutorialPanel");
        tutorialPanel.transform.SetParent(transform);
        
        // Add Canvas Group for fade effects
        CanvasGroup canvasGroup = tutorialPanel.AddComponent<CanvasGroup>();
        
        // Add Image component for background
        Image panelImage = tutorialPanel.AddComponent<Image>();
        panelImage.color = backgroundColor;
        panelImage.raycastTarget = false; // Don't block input
        
        // Set panel size and position
        RectTransform panelRect = tutorialPanel.GetComponent<RectTransform>();
        panelRect.sizeDelta = panelSize;
        panelRect.anchoredPosition = defaultPosition;
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        
        // Create instruction text
        GameObject instructionTextObj = new GameObject("InstructionText");
        instructionTextObj.transform.SetParent(tutorialPanel.transform);
        
        Text instructionText = instructionTextObj.AddComponent<Text>();
        instructionText.text = "Tutorial instruction will appear here...";
        instructionText.color = textColor;
        instructionText.font = textFont != null ? textFont : Resources.GetBuiltinResource<Font>("Arial.ttf");
        instructionText.fontSize = fontSize;
        instructionText.alignment = TextAnchor.MiddleCenter;
        instructionText.horizontalOverflow = HorizontalWrapMode.Wrap;
        instructionText.verticalOverflow = VerticalWrapMode.Overflow;
        
        RectTransform textRect = instructionTextObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 40); // Leave space for buttons
        textRect.offsetMax = new Vector2(-10, -10);
        
        // Create input prompt text
        GameObject inputPromptObj = new GameObject("InputPromptText");
        inputPromptObj.transform.SetParent(tutorialPanel.transform);
        
        Text inputPromptText = inputPromptObj.AddComponent<Text>();
        inputPromptText.text = "Press: WASD";
        inputPromptText.color = Color.yellow;
        inputPromptText.font = textFont != null ? textFont : Resources.GetBuiltinResource<Font>("Arial.ttf");
        inputPromptText.fontSize = fontSize - 2;
        inputPromptText.alignment = TextAnchor.MiddleCenter;
        inputPromptText.fontStyle = FontStyle.Bold;
        
        RectTransform promptRect = inputPromptObj.GetComponent<RectTransform>();
        promptRect.anchorMin = new Vector2(0, 0);
        promptRect.anchorMax = new Vector2(1, 0);
        promptRect.offsetMin = new Vector2(10, 5);
        promptRect.offsetMax = new Vector2(-10, 35);
        
        // Create instruction image (optional)
        GameObject imageObj = new GameObject("InstructionImage");
        imageObj.transform.SetParent(tutorialPanel.transform);
        
        Image instructionImage = imageObj.AddComponent<Image>();
        instructionImage.color = Color.white;
        instructionImage.raycastTarget = false;
        
        RectTransform imageRect = imageObj.GetComponent<RectTransform>();
        imageRect.anchorMin = new Vector2(0.5f, 0.5f);
        imageRect.anchorMax = new Vector2(0.5f, 0.5f);
        imageRect.sizeDelta = new Vector2(80, 80);
        imageRect.anchoredPosition = new Vector2(0, 20);
        imageObj.SetActive(false); // Hidden by default
        
        // Create skip button
        GameObject skipButtonObj = CreateButton("SkipButton", skipButtonText, new Vector2(-buttonSize.x/2 - 5, 5));
        skipButtonObj.transform.SetParent(tutorialPanel.transform);
        
        // Create next button
        GameObject nextButtonObj = CreateButton("NextButton", nextButtonText, new Vector2(buttonSize.x/2 + 5, 5));
        nextButtonObj.transform.SetParent(tutorialPanel.transform);
        nextButtonObj.SetActive(false); // Hidden by default
        
        // Create arrow object
        GameObject arrowObj = new GameObject("TutorialArrow");
        arrowObj.transform.SetParent(transform);
        
        Image arrowImage = arrowObj.AddComponent<Image>();
        arrowImage.sprite = arrowSprite != null ? arrowSprite : CreateDefaultArrowSprite();
        arrowImage.color = arrowColor;
        arrowImage.raycastTarget = false;
        
        RectTransform arrowRect = arrowObj.GetComponent<RectTransform>();
        arrowRect.sizeDelta = arrowSize;
        arrowRect.anchoredPosition = Vector2.zero;
        arrowRect.anchorMin = new Vector2(0.5f, 0.5f);
        arrowRect.anchorMax = new Vector2(0.5f, 0.5f);
        arrowRect.pivot = new Vector2(0.5f, 0.5f);
        arrowObj.SetActive(false); // Hidden by default
        
        // Add TutorialManager component
        TutorialManager tutorialManager = tutorialPanel.AddComponent<TutorialManager>();
        tutorialManager.tutorialPanel = tutorialPanel;
        tutorialManager.instructionText = instructionText;
        tutorialManager.instructionImage = instructionImage;
        tutorialManager.inputPromptText = inputPromptText;
        tutorialManager.arrowObject = arrowObj;
        tutorialManager.skipButton = skipButtonObj.GetComponent<Button>();
        tutorialManager.nextButton = nextButtonObj.GetComponent<Button>();
        
        // Add AudioSource for sounds
        AudioSource audioSource = tutorialPanel.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        
        tutorialUIPrefab = tutorialPanel;
        
        Debug.Log("Tutorial UI created successfully!");
    }
    
    private GameObject CreateButton(string name, string text, Vector2 position)
    {
        GameObject buttonObj = new GameObject(name);
        
        // Add Image component for button background
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        
        // Add Button component
        Button button = buttonObj.AddComponent<Button>();
        
        // Set button colors
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 0.9f);
        colors.pressedColor = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        button.colors = colors;
        
        // Set button size and position
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.sizeDelta = buttonSize;
        buttonRect.anchoredPosition = position;
        buttonRect.anchorMin = new Vector2(0.5f, 0);
        buttonRect.anchorMax = new Vector2(0.5f, 0);
        buttonRect.pivot = new Vector2(0.5f, 0);
        
        // Create button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);
        
        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = text;
        buttonText.color = Color.white;
        buttonText.font = textFont != null ? textFont : Resources.GetBuiltinResource<Font>("Arial.ttf");
        buttonText.fontSize = fontSize - 2;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        return buttonObj;
    }
    
    private Sprite CreateDefaultArrowSprite()
    {
        // Create a simple arrow texture
        Texture2D arrowTexture = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];
        
        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                // Create a simple arrow shape
                bool isArrow = false;
                
                // Arrow body
                if (x >= 8 && x <= 24 && y >= 12 && y <= 20)
                {
                    isArrow = true;
                }
                // Arrow head
                else if (x >= 20 && x <= 28 && y >= 8 && y <= 24)
                {
                    if (x + y >= 28 && x + y <= 52 && x - y >= -4 && x - y <= 20)
                    {
                        isArrow = true;
                    }
                }
                
                pixels[y * 32 + x] = isArrow ? Color.yellow : Color.clear;
            }
        }
        
        arrowTexture.SetPixels(pixels);
        arrowTexture.Apply();
        
        return Sprite.Create(arrowTexture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
    }
    
    [ContextMenu("Setup Tutorial in Scene")]
    public void SetupTutorialInScene()
    {
        if (tutorialUIPrefab == null)
        {
            CreateTutorialUIPrefab();
        }
        
        // Find or create Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("TutorialCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // Instantiate tutorial UI
        GameObject tutorialUI = Instantiate(tutorialUIPrefab, canvas.transform);
        tutorialUI.name = "TutorialUI";
        
        // Add Map1TutorialSetup
        Map1TutorialSetup tutorialSetup = tutorialUI.AddComponent<Map1TutorialSetup>();
        TutorialManager tutorialManager = tutorialUI.GetComponent<TutorialManager>();
        tutorialSetup.tutorialManager = tutorialManager;
        
        Debug.Log("Tutorial setup completed in scene!");
    }
} 