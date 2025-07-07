using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Map5UIExtractor : MonoBehaviour
{
    [Header("Extract Settings")]
    public bool extractOnStart = true;
    
    [Header("Target Position")]
    public Vector2 uiPosition = new Vector2(20, -20); // Top-left position
    
    [Header("References")]
    public Canvas targetCanvas;
    
    private PlayerUIManager uiManager;
    private GameObject extractedUI;
    
    void Start()
    {
        Debug.Log("Map5UIExtractor: Start() called");
        if (extractOnStart)
        {
            Debug.Log("Map5UIExtractor: Starting extraction...");
            StartCoroutine(ExtractMap5UI());
        }
        else
        {
            Debug.Log("Map5UIExtractor: Extract on start is disabled. Use 'Extract Map 5 UI' button.");
        }
    }
    
    [ContextMenu("Extract Map 5 UI")]
    public void ExtractUIFromMap5()
    {
        Debug.Log("Map5UIExtractor: Manual extraction triggered");
        StartCoroutine(ExtractMap5UI());
    }
    
    private IEnumerator ExtractMap5UI()
    {
        Debug.Log("Starting Map 5 UI extraction...");
        
        // Load Map 5 scene additively
        string scenePath = "Assets/Scenes/Map 5 - Hau.unity";
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Map 5 - Hau", LoadSceneMode.Additive);
        
        yield return asyncLoad;
        
        Debug.Log("Map 5 scene loaded, searching for UI...");
        
        // Find all GameObjects in Map 5 scene
        Scene map5Scene = SceneManager.GetSceneByName("Map 5 - Hau");
        if (map5Scene.isLoaded)
        {
            GameObject[] rootObjects = map5Scene.GetRootGameObjects();
            
            foreach (GameObject rootObj in rootObjects)
            {
                // Look for Canvas or UI elements
                Canvas canvas = rootObj.GetComponentInChildren<Canvas>();
                if (canvas != null)
                {
                    Debug.Log($"Found Canvas: {canvas.name}");
                    ExtractUIFromCanvas(canvas);
                    break;
                }
                
                // Also check for UI elements without Canvas
                Image[] images = rootObj.GetComponentsInChildren<Image>();
                if (images.Length > 0)
                {
                    Debug.Log($"Found UI elements in: {rootObj.name}");
                    ExtractUIFromGameObject(rootObj);
                    break;
                }
            }
        }
        
        // Unload Map 5 scene
        yield return new WaitForSeconds(0.5f); // Wait a bit for extraction
        SceneManager.UnloadSceneAsync("Map 5 - Hau");
        
        Debug.Log("Map 5 UI extraction completed!");
    }
    
    private void ExtractUIFromCanvas(Canvas sourceCanvas)
    {
        // Find or create target canvas
        if (targetCanvas == null)
        {
            targetCanvas = FindObjectOfType<Canvas>();
            if (targetCanvas == null)
            {
                CreateTargetCanvas();
            }
        }
        
        // Clone the entire UI structure
        extractedUI = Instantiate(sourceCanvas.gameObject);
        extractedUI.name = "ExtractedUI_FromMap5";
        
        // Remove Canvas component from cloned object (we'll parent it to existing canvas)
        Canvas clonedCanvas = extractedUI.GetComponent<Canvas>();
        if (clonedCanvas != null)
        {
            DestroyImmediate(clonedCanvas);
        }
        
        // Remove CanvasScaler and GraphicRaycaster if present
        CanvasScaler scaler = extractedUI.GetComponent<CanvasScaler>();
        if (scaler != null) DestroyImmediate(scaler);
        
        GraphicRaycaster raycaster = extractedUI.GetComponent<GraphicRaycaster>();
        if (raycaster != null) DestroyImmediate(raycaster);
        
        // Parent to target canvas
        extractedUI.transform.SetParent(targetCanvas.transform, false);
        
        // Position the UI
        RectTransform rectTransform = extractedUI.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchorMin = new Vector2(0, 1); // Top-left
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.anchoredPosition = uiPosition;
        }
        
        // Setup UI Manager
        SetupUIManager();
        
        Debug.Log("UI extracted and positioned successfully!");
    }
    
    private void ExtractUIFromGameObject(GameObject sourceObj)
    {
        // Find or create target canvas
        if (targetCanvas == null)
        {
            targetCanvas = FindObjectOfType<Canvas>();
            if (targetCanvas == null)
            {
                CreateTargetCanvas();
            }
        }
        
        // Clone the UI GameObject
        extractedUI = Instantiate(sourceObj);
        extractedUI.name = "ExtractedUI_FromMap5";
        
        // Parent to target canvas
        extractedUI.transform.SetParent(targetCanvas.transform, false);
        
        // Position the UI
        RectTransform rectTransform = extractedUI.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = extractedUI.AddComponent<RectTransform>();
        }
        
        rectTransform.anchorMin = new Vector2(0, 1); // Top-left
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.anchoredPosition = uiPosition;
        
        // Setup UI Manager
        SetupUIManager();
        
        Debug.Log("UI GameObject extracted and positioned successfully!");
    }
    
    private void CreateTargetCanvas()
    {
        GameObject canvasObj = new GameObject("PlayerUICanvas_Map3");
        targetCanvas = canvasObj.AddComponent<Canvas>();
        targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        targetCanvas.sortingOrder = 10;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        Debug.Log("Target Canvas created for Map 3");
    }
    
    private void SetupUIManager()
    {
        if (extractedUI == null) return;
        
        // Add PlayerUIManager to extracted UI
        uiManager = extractedUI.GetComponent<PlayerUIManager>();
        if (uiManager == null)
        {
            uiManager = extractedUI.AddComponent<PlayerUIManager>();
        }
        
        // Auto-detect UI bars
        Image[] allImages = extractedUI.GetComponentsInChildren<Image>();
        
        foreach (Image img in allImages)
        {
            string imgName = img.name.ToLower();
            
            // Try to identify health bar (usually red or has "health" in name)
            if (imgName.Contains("health") || imgName.Contains("hp") || 
                (img.color.r > 0.7f && img.color.g < 0.3f && img.color.b < 0.3f))
            {
                uiManager.healthBar = img;
                Debug.Log($"Health bar detected: {img.name}");
            }
            // Try to identify mana bar (usually blue or has "mana" in name)
            else if (imgName.Contains("mana") || imgName.Contains("mp") || 
                     (img.color.b > 0.7f && img.color.r < 0.3f && img.color.g < 0.3f))
            {
                uiManager.manaBar = img;
                Debug.Log($"Mana bar detected: {img.name}");
            }
            // Try to identify stamina bar (usually yellow/green or has "stamina" in name)
            else if (imgName.Contains("stamina") || imgName.Contains("energy") ||
                     (img.color.g > 0.7f && img.color.r > 0.5f && img.color.b < 0.3f))
            {
                uiManager.staminaBar = img;
                Debug.Log($"Stamina bar detected: {img.name}");
            }
        }
        
        Debug.Log("UI Manager setup completed!");
    }
    
    [ContextMenu("Remove Extracted UI")]
    public void RemoveExtractedUI()
    {
        if (extractedUI != null)
        {
            DestroyImmediate(extractedUI);
            Debug.Log("Extracted UI removed");
        }
    }
    
    [ContextMenu("List All UI Elements")]
    public void ListUIElements()
    {
        if (extractedUI != null)
        {
            Image[] images = extractedUI.GetComponentsInChildren<Image>();
            Debug.Log($"Found {images.Length} Image components:");
            
            foreach (Image img in images)
            {
                Debug.Log($"- {img.name}: Color={img.color}, Type={img.type}");
            }
        }
    }
}
