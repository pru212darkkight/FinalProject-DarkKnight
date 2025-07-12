using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class SimpleLevelSelectionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button[] levelButtons;
    [SerializeField] private Button closeButton;
    [SerializeField] private Text titleText;
    
    [Header("Level Names")]
    [SerializeField] private string[] levelNames = {
        "Map 1 - Tin",
        "Map 2 - Han", 
        "Map 3 - Tuyen",
        "Map 4 - Phuong",
        "Map 5 - Hau"
    };
    
    [Header("UI Settings")]
    [SerializeField] private string titleTextString = "Chọn Màn Chơi";
    
    [Header("Tutorial Lock Settings")]
    [SerializeField] private bool checkTutorialCompletion = true;
    [SerializeField] private Color lockedButtonColor = Color.gray;
    [SerializeField] private Color lockedButtonTextColor = Color.darkGray;
    [SerializeField] private string lockedButtonText = "🔒 Khóa";
    [SerializeField] private string tutorialNotCompleteText = "Hoàn thành Tutorial để mở khóa";
    
    private bool isVisible = false;
    private bool tutorialCompleted = false;
    
    public Action<int> OnLevelSelected; // Event callback cho Teleporter
    
    void Start()
    {
        // Setup title text
        if (titleText != null)
        {
            titleText.text = titleTextString;
        }
        
        // Setup close button
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideUI);
        }
        
        // Kiểm tra trạng thái tutorial
        CheckTutorialStatus();
        
        // Setup level buttons
        SetupLevelButtons();
        
        // Ẩn UI ban đầu
        gameObject.SetActive(false);
        
        Debug.Log($"SimpleLevelSelectionUI initialized - Tutorial Completed: {tutorialCompleted}");
    }
    
    void SetupLevelButtons()
    {
        if (levelButtons == null || levelButtons.Length == 0)
        {
            Debug.LogWarning("No level buttons assigned to SimpleLevelSelectionUI");
            return;
        }
        
        for (int i = 0; i < levelButtons.Length && i < levelNames.Length; i++)
        {
            if (levelButtons[i] != null)
            {
                int levelIndex = i; // Copy để closure hoạt động đúng
                
                // Kiểm tra xem level có bị khóa không
                bool isLocked = IsLevelLocked(levelIndex);
                
                if (isLocked)
                {
                    // Setup button bị khóa
                    levelButtons[i].onClick.AddListener(() => OnLockedLevelClicked(levelIndex));
                }
                else
                {
                    // Setup button bình thường
                    levelButtons[i].onClick.AddListener(() => {
                        HideUI();
                        OnLevelSelected?.Invoke(levelIndex); // Gọi callback cho Teleporter
                    });
                }
                
                // Setup button text và màu sắc
                Text buttonText = levelButtons[i].GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = isLocked ? lockedButtonText : levelNames[i];
                    buttonText.color = isLocked ? lockedButtonTextColor : Color.black;
                }
                
                // Setup button colors
                ColorBlock colors = levelButtons[i].colors;
                if (isLocked)
                {
                    colors.normalColor = lockedButtonColor;
                    colors.highlightedColor = lockedButtonColor;
                    colors.pressedColor = lockedButtonColor;
                    colors.selectedColor = lockedButtonColor;
                    colors.disabledColor = lockedButtonColor;
                    colors.fadeDuration = 0f; // Không có hiệu ứng fade
                }
                else
                {
                    colors.normalColor = Color.white;
                    colors.highlightedColor = Color.yellow;
                    colors.pressedColor = Color.green;
                    colors.selectedColor = Color.yellow;
                    colors.disabledColor = Color.gray;
                    colors.fadeDuration = 0.1f;
                }
                levelButtons[i].colors = colors;
                
                Debug.Log($"Setup level button {i} for: {levelNames[i]} - Locked: {isLocked}");
            }
        }
    }
    
    public void ShowUI()
    {
        if (isVisible) return;
        
        // Cập nhật trạng thái tutorial trước khi hiển thị
        CheckTutorialStatus();
        
        gameObject.SetActive(true);
        isVisible = true;
        
        // Refresh buttons để cập nhật trạng thái khóa/mở
        RefreshLevelButtons();
        
        // Pause game
        Time.timeScale = 0f;
        
        Debug.Log($"SimpleLevelSelectionUI shown - Tutorial Completed: {tutorialCompleted}");
    }
    
    public void HideUI()
    {
        if (!isVisible) return;
        
        isVisible = false;
        gameObject.SetActive(false);
        
        // Resume game
        Time.timeScale = 1f;
        
        Debug.Log("SimpleLevelSelectionUI hidden");
    }
    
    // Public method để load level trực tiếp
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
    
    // Refresh lại tất cả buttons để cập nhật trạng thái khóa/mở
    void RefreshLevelButtons()
    {
        if (levelButtons == null || levelButtons.Length == 0) return;
        
        for (int i = 0; i < levelButtons.Length && i < levelNames.Length; i++)
        {
            if (levelButtons[i] != null)
            {
                bool isLocked = IsLevelLocked(i);
                
                // Setup button text và màu sắc
                Text buttonText = levelButtons[i].GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = isLocked ? lockedButtonText : levelNames[i];
                    buttonText.color = isLocked ? lockedButtonTextColor : Color.black;
                }
                
                // Setup button colors
                ColorBlock colors = levelButtons[i].colors;
                if (isLocked)
                {
                    colors.normalColor = lockedButtonColor;
                    colors.highlightedColor = lockedButtonColor;
                    colors.pressedColor = lockedButtonColor;
                    colors.selectedColor = lockedButtonColor;
                    colors.disabledColor = lockedButtonColor;
                    // colors.fadeDuration = 0f; // Không có hiệu ứng fade
                }
                else
                {
                    colors.normalColor = Color.white;
                    colors.highlightedColor = Color.yellow;
                    colors.pressedColor = Color.green;
                    colors.selectedColor = Color.yellow;
                    colors.disabledColor = Color.gray;
                    colors.fadeDuration = 0.1f;
                }
                levelButtons[i].colors = colors;
            }
        }
        
        Debug.Log("Level buttons refreshed");
    }
    
    // Public method để refresh trạng thái tutorial (có thể gọi từ bên ngoài)
    public void RefreshTutorialStatus()
    {
        CheckTutorialStatus();
        
        // Nếu UI đang hiển thị, refresh lại buttons
        if (isVisible)
        {
            RefreshLevelButtons();
        }
    }
    
    // Public method để force unlock tất cả level (cho testing)
    public void ForceUnlockAllLevels()
    {
        tutorialCompleted = true;
        if (isVisible)
        {
            RefreshLevelButtons();
        }
        Debug.Log("All levels force unlocked");
    }
    
    // Public method để force lock tất cả level trừ map 1 (cho testing)
    public void ForceLockLevels()
    {
        tutorialCompleted = false;
        if (isVisible)
        {
            RefreshLevelButtons();
        }
        Debug.Log("Levels locked (except map 1)");
    }
} 