using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CopyUIFromMap5 : MonoBehaviour
{
    [Header("Copy Settings")]
    public bool copyOnStart = false;
    public string map5SceneName = "Map 5 - Hau"; // Tên scene Map 5

    [Header("UI Style Settings")]
    public bool copyExactStyle = true;
    public bool includeCharacterAvatar = true;
    
    [Header("UI Position Settings")]
    public Vector2 healthBarPosition = new Vector2(50, -50);
    public Vector2 staminaBarPosition = new Vector2(50, -90);
    public Vector2 manaBarPosition = new Vector2(50, -130);
    
    [Header("Manual UI Creation")]
    public bool createManualUI = true;
    public Vector2 barSize = new Vector2(200, 25);
    
    private PlayerUIManager uiManager;
    
    void Start()
    {
        if (copyOnStart)
        {
            CopyUIFromMap5Scene();
        }
        else if (createManualUI)
        {
            CreateSimpleUI();
        }
    }
    
    [ContextMenu("Copy UI From Map 5")]
    public void CopyUIFromMap5Scene()
    {
        // Tìm scene Map 5 trong build settings
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            if (scenePath.Contains("Map 5") || scenePath.Contains(map5SceneName))
            {
                Debug.Log($"Found Map 5 scene: {scenePath}");
                // Load scene additively để copy UI
                StartCoroutine(LoadMap5AndCopyUI(scenePath));
                return;
            }
        }
        
        Debug.LogWarning("Map 5 scene not found! Creating manual UI instead.");
        CreateSimpleUI();
    }
    
    private System.Collections.IEnumerator LoadMap5AndCopyUI(string scenePath)
    {
        // Load Map 5 scene additively
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
        yield return asyncLoad;
        
        // Tìm UI Canvas trong Map 5
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        Canvas map5Canvas = null;
        
        foreach (Canvas canvas in allCanvases)
        {
            if (canvas.gameObject.scene.name.Contains("Map 5"))
            {
                map5Canvas = canvas;
                break;
            }
        }
        
        if (map5Canvas != null)
        {
            // Copy UI elements
            CopyUIElements(map5Canvas);
        }
        else
        {
            Debug.LogWarning("No Canvas found in Map 5! Creating manual UI.");
            CreateSimpleUI();
        }
        
        // Unload Map 5 scene
        SceneManager.UnloadSceneAsync(scenePath);
    }
    
    private void CopyUIElements(Canvas sourceCanvas)
    {
        // Tìm Canvas trong scene hiện tại
        Canvas targetCanvas = FindObjectOfType<Canvas>();
        if (targetCanvas == null)
        {
            CreateCanvas();
            targetCanvas = FindObjectOfType<Canvas>();
        }
        
        // Copy toàn bộ UI từ Map 5
        GameObject copiedUI = Instantiate(sourceCanvas.gameObject, targetCanvas.transform);
        copiedUI.name = "CopiedUI_FromMap5";
        
        // Tìm và setup PlayerUIManager
        SetupUIManager(copiedUI);
        
        Debug.Log("UI copied from Map 5 successfully!");
    }
    
    [ContextMenu("Create Simple UI")]
    public void CreateSimpleUI()
    {
        // Tìm hoặc tạo Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            CreateCanvas();
            canvas = FindObjectOfType<Canvas>();
        }
        
        // Tạo UI Manager
        GameObject uiManagerObj = new GameObject("PlayerUIManager_Simple");
        uiManagerObj.transform.SetParent(canvas.transform);
        uiManager = uiManagerObj.AddComponent<PlayerUIManager>();
        
        // Tạo Health Bar
        Image healthBar = CreateSimpleBar("HealthBar", healthBarPosition, Color.red);
        uiManager.healthBar = healthBar;
        
        // Tạo Stamina Bar
        Image staminaBar = CreateSimpleBar("StaminaBar", staminaBarPosition, Color.yellow);
        uiManager.staminaBar = staminaBar;
        
        // Tạo Mana Bar
        Image manaBar = CreateSimpleBar("ManaBar", manaBarPosition, Color.blue);
        uiManager.manaBar = manaBar;
        
        Debug.Log("Simple UI created successfully!");
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
    }
    
    private Image CreateSimpleBar(string name, Vector2 position, Color color)
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        
        // Background
        GameObject bgObj = new GameObject(name + "_Background");
        bgObj.transform.SetParent(canvas.transform);
        
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
        
        return fillImage;
    }
    
    private void SetupUIManager(GameObject copiedUI)
    {
        // Tìm hoặc thêm PlayerUIManager
        PlayerUIManager manager = copiedUI.GetComponentInChildren<PlayerUIManager>();
        if (manager == null)
        {
            manager = copiedUI.AddComponent<PlayerUIManager>();
        }
        
        // Tự động tìm UI bars trong copied UI
        Image[] images = copiedUI.GetComponentsInChildren<Image>();
        foreach (Image img in images)
        {
            if (img.name.ToLower().Contains("health"))
                manager.healthBar = img;
            else if (img.name.ToLower().Contains("stamina"))
                manager.staminaBar = img;
            else if (img.name.ToLower().Contains("mana"))
                manager.manaBar = img;
        }
        
        uiManager = manager;
    }
    
    [ContextMenu("Remove UI")]
    public void RemoveUI()
    {
        if (uiManager != null)
        {
            DestroyImmediate(uiManager.gameObject);
            Debug.Log("UI removed");
        }
    }
}
