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
    
    private bool isVisible = false;
    
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
        
        // Setup level buttons
        SetupLevelButtons();
        
        // Ẩn UI ban đầu
        gameObject.SetActive(false);
        
        Debug.Log("SimpleLevelSelectionUI initialized");
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
                levelButtons[i].onClick.AddListener(() => {
                    HideUI();
                    OnLevelSelected?.Invoke(levelIndex); // Gọi callback cho Teleporter
                });
                
                // Setup button text nếu có
                Text buttonText = levelButtons[i].GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = levelNames[i];
                }
                
                Debug.Log($"Setup level button {i} for: {levelNames[i]}");
            }
        }
    }
    
    public void ShowUI()
    {
        if (isVisible) return;
        
        gameObject.SetActive(true);
        isVisible = true;
        
        // Pause game
        Time.timeScale = 0f;
        
        Debug.Log("SimpleLevelSelectionUI shown");
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
} 