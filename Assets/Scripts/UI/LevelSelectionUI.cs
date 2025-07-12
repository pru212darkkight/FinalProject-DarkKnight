using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelSelectionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button closeButton;
    [SerializeField] private Text titleText;
    
    [Header("UI Settings")]
    [SerializeField] private string titleTextString = "Chọn Màn Chơi";
    [SerializeField] private Color buttonNormalColor = Color.white;
    [SerializeField] private Color buttonHoverColor = Color.yellow;
    [SerializeField] private Color buttonPressedColor = Color.green;
    
    [Header("Tutorial Lock Settings")]
    [SerializeField] private bool checkTutorialCompletion = true;
    [SerializeField] private Color lockedButtonColor = Color.gray;
    [SerializeField] private Color lockedButtonTextColor = Color.darkGray;
    [SerializeField] private string lockedButtonText = "🔒 Khóa";
    [SerializeField] private string tutorialNotCompleteText = "Hoàn thành Tutorial để mở khóa";
    
    [Header("Animation")]
    [SerializeField] private Animator uiAnimator;
    [SerializeField] private string showAnimationTrigger = "Show";
    [SerializeField] private string hideAnimationTrigger = "Hide";
    
    private string[] availableLevels;
    private string[] levelDisplayNames;
    private List<Button> levelButtons = new List<Button>();
    private List<Text> levelButtonTexts = new List<Text>();
    private bool isVisible = false;
    private bool tutorialCompleted = false;
    
    void Start()
    {
        // Tìm Animator nếu chưa được gán
        if (uiAnimator == null)
        {
            uiAnimator = GetComponent<Animator>();
        }
        
        // Tìm button container nếu chưa được gán
        if (buttonContainer == null)
        {
            buttonContainer = transform.Find("ButtonContainer");
            if (buttonContainer == null)
            {
                Debug.LogWarning("ButtonContainer not found, using this transform as container");
                buttonContainer = transform;
            }
        }
        
        // Setup close button
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideUI);
        }
        
        // Setup title text
        if (titleText != null)
        {
            titleText.text = titleTextString;
        }
        
        // Kiểm tra trạng thái tutorial
        CheckTutorialStatus();
        
        // Ẩn UI ban đầu
        gameObject.SetActive(false);
        
        Debug.Log($"LevelSelectionUI initialized - Animator: {uiAnimator != null}, Container: {buttonContainer != null}, Tutorial Completed: {tutorialCompleted}");
    }
    
    public void SetupLevels(string[] levels, string[] displayNames)
    {
        availableLevels = levels;
        levelDisplayNames = displayNames;
        
        Debug.Log($"LevelSelectionUI setup with {levels.Length} levels");
    }
    
    public void ShowUI()
    {
        if (isVisible) return;
        
        // Cập nhật trạng thái tutorial trước khi hiển thị
        CheckTutorialStatus();
        
        gameObject.SetActive(true);
        isVisible = true;
        
        // Tạo buttons cho các level
        CreateLevelButtons();
        
        // Chạy animation hiện UI
        if (uiAnimator != null)
        {
            uiAnimator.SetTrigger(showAnimationTrigger);
            Debug.Log("Playing show animation for LevelSelectionUI");
        }
        
        // Pause game (tùy chọn)
        Time.timeScale = 0f;
        
        Debug.Log($"LevelSelectionUI shown - Tutorial Completed: {tutorialCompleted}");
    }
    
    public void HideUI()
    {
        if (!isVisible) return;
        
        isVisible = false;
        
        // Chạy animation ẩn UI
        if (uiAnimator != null)
        {
            uiAnimator.SetTrigger(hideAnimationTrigger);
            Debug.Log("Playing hide animation for LevelSelectionUI");
            
            // Ẩn UI sau khi animation kết thúc
            Invoke(nameof(DeactivateUI), 0.5f);
        }
        else
        {
            DeactivateUI();
        }
        
        // Resume game
        Time.timeScale = 1f;
        
        Debug.Log("LevelSelectionUI hidden");
    }
    
    void DeactivateUI()
    {
        gameObject.SetActive(false);
        ClearLevelButtons();
    }
    
    void CreateLevelButtons()
    {
        ClearLevelButtons();
        
        if (availableLevels == null || availableLevels.Length == 0)
        {
            Debug.LogWarning("No levels available for LevelSelectionUI");
            return;
        }
        
        for (int i = 0; i < availableLevels.Length; i++)
        {
            CreateLevelButton(i);
        }
        
        Debug.Log($"Created {levelButtons.Count} level buttons");
    }
    
    void CreateLevelButton(int levelIndex)
    {
        if (levelButtonPrefab == null)
        {
            // Tạo button mặc định nếu không có prefab
            CreateDefaultLevelButton(levelIndex);
            return;
        }
        
        GameObject buttonObj = Instantiate(levelButtonPrefab, buttonContainer);
        Button button = buttonObj.GetComponent<Button>();
        Text buttonText = buttonObj.GetComponentInChildren<Text>();
        
        if (button != null && buttonText != null)
        {
            // Kiểm tra xem level có bị khóa không
            bool isLocked = IsLevelLocked(levelIndex);
            
            // Setup button text
            string displayName = levelIndex < levelDisplayNames.Length ? levelDisplayNames[levelIndex] : availableLevels[levelIndex];
            buttonText.text = isLocked ? lockedButtonText : displayName;
            
            // Setup button click
            int levelIndexCopy = levelIndex; // Cần copy để closure hoạt động đúng
            if (isLocked)
            {
                button.onClick.AddListener(() => OnLockedLevelClicked(levelIndexCopy));
            }
            else
            {
                button.onClick.AddListener(() => LoadLevel(levelIndexCopy));
            }
            
            // Setup button colors
            ColorBlock colors = button.colors;
            if (isLocked)
            {
                colors.normalColor = lockedButtonColor;
                colors.highlightedColor = lockedButtonColor;
                colors.pressedColor = lockedButtonColor;
                buttonText.color = lockedButtonTextColor;
            }
            else
            {
                colors.normalColor = buttonNormalColor;
                colors.highlightedColor = buttonHoverColor;
                colors.pressedColor = buttonPressedColor;
                buttonText.color = Color.black;
            }
            button.colors = colors;
            
            levelButtons.Add(button);
            levelButtonTexts.Add(buttonText);
            
            Debug.Log($"Created level button for: {displayName} - Locked: {isLocked}");
        }
        else
        {
            Debug.LogError($"Failed to create level button for index {levelIndex}");
        }
    }
    
    void CreateDefaultLevelButton(int levelIndex)
    {
        GameObject buttonObj = new GameObject($"LevelButton_{levelIndex}");
        buttonObj.transform.SetParent(buttonContainer);
        
        // Thêm các component cần thiết
        RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
        Image image = buttonObj.AddComponent<Image>();
        Button button = buttonObj.AddComponent<Button>();
        
        // Tạo text child
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        Text text = textObj.AddComponent<Text>();
        
        // Kiểm tra xem level có bị khóa không
        bool isLocked = IsLevelLocked(levelIndex);
        
        // Setup text
        string displayName = levelIndex < levelDisplayNames.Length ? levelDisplayNames[levelIndex] : availableLevels[levelIndex];
        text.text = isLocked ? lockedButtonText : displayName;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 24;
        text.color = isLocked ? lockedButtonTextColor : Color.black;
        text.alignment = TextAnchor.MiddleCenter;
        
        // Setup rect transforms
        rectTransform.sizeDelta = new Vector2(200, 50);
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        // Setup button
        int levelIndexCopy = levelIndex;
        if (isLocked)
        {
            button.onClick.AddListener(() => OnLockedLevelClicked(levelIndexCopy));
        }
        else
        {
            button.onClick.AddListener(() => LoadLevel(levelIndexCopy));
        }
        
        // Setup colors
        ColorBlock colors = button.colors;
        if (isLocked)
        {
            colors.normalColor = lockedButtonColor;
            colors.highlightedColor = lockedButtonColor;
            colors.pressedColor = lockedButtonColor;
        }
        else
        {
            colors.normalColor = buttonNormalColor;
            colors.highlightedColor = buttonHoverColor;
            colors.pressedColor = buttonPressedColor;
        }
        button.colors = colors;
        
        levelButtons.Add(button);
        levelButtonTexts.Add(text);
        
        Debug.Log($"Created default level button for: {displayName} - Locked: {isLocked}");
    }
    
    void ClearLevelButtons()
    {
        foreach (Button button in levelButtons)
        {
            if (button != null)
            {
                DestroyImmediate(button.gameObject);
            }
        }
        levelButtons.Clear();
        levelButtonTexts.Clear();
        
        Debug.Log("Cleared all level buttons");
    }
    
    void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= availableLevels.Length)
        {
            Debug.LogError($"Invalid level index: {levelIndex}");
            return;
        }
        
        string levelName = availableLevels[levelIndex];
        Debug.Log($"Loading level: {levelName}");
        
        // Resume game trước khi load scene
        Time.timeScale = 1f;
        
        // Load scene
        SceneManager.LoadScene(levelName);
    }
    
    // Public method để load level trực tiếp (có thể gọi từ button khác)
    public void LoadLevelByName(string levelName)
    {
        Debug.Log($"Loading level by name: {levelName}");
        
        // Resume game trước khi load scene
        Time.timeScale = 1f;
        
        SceneManager.LoadScene(levelName);
    }
    
    // Public method để kiểm tra UI có đang hiện không
    public bool IsVisible()
    {
        return isVisible;
    }
    
    // Kiểm tra trạng thái tutorial
    void CheckTutorialStatus()
    {
        if (checkTutorialCompletion && TutorialDataManager.Instance != null)
        {
            tutorialCompleted = TutorialDataManager.Instance.IsTutorialCompleted();
            Debug.Log($"Tutorial status checked - Completed: {tutorialCompleted}");
        }
        else
        {
            tutorialCompleted = true; // Nếu không check tutorial thì coi như đã hoàn thành
            Debug.Log("Tutorial check disabled or TutorialDataManager not found");
        }
    }
    
    // Kiểm tra xem level có bị khóa không
    bool IsLevelLocked(int levelIndex)
    {
        if (!checkTutorialCompletion) return false; // Nếu không check tutorial thì không khóa level nào
        if (tutorialCompleted) return false; // Nếu đã hoàn thành tutorial thì mở tất cả level
        if (levelIndex == 0) return false; // Map 1 luôn mở
        
        return true; // Các map khác bị khóa nếu chưa hoàn thành tutorial
    }
    
    // Xử lý khi click vào level bị khóa
    void OnLockedLevelClicked(int levelIndex)
    {
        Debug.Log($"Locked level clicked: {levelIndex}");
        
        // Hiển thị thông báo cho player
        ShowLockedLevelMessage();
    }
    
    // Hiển thị thông báo level bị khóa
    void ShowLockedLevelMessage()
    {
        // Có thể hiển thị popup hoặc thông báo
        Debug.Log(tutorialNotCompleteText);
        
        // Nếu có UI notification system, có thể gọi ở đây
        // Ví dụ: NotificationManager.Instance.ShowMessage(tutorialNotCompleteText);
    }
    
    // Public method để refresh trạng thái tutorial (có thể gọi từ bên ngoài)
    public void RefreshTutorialStatus()
    {
        CheckTutorialStatus();
        
        // Nếu UI đang hiển thị, refresh lại buttons
        if (isVisible)
        {
            CreateLevelButtons();
        }
    }
    
    // Public method để force unlock tất cả level (cho testing)
    public void ForceUnlockAllLevels()
    {
        tutorialCompleted = true;
        if (isVisible)
        {
            CreateLevelButtons();
        }
        Debug.Log("All levels force unlocked");
    }
    
    // Public method để force lock tất cả level trừ map 1 (cho testing)
    public void ForceLockLevels()
    {
        tutorialCompleted = false;
        if (isVisible)
        {
            CreateLevelButtons();
        }
        Debug.Log("Levels locked (except map 1)");
    }
} 