using UnityEngine;
using UnityEngine.UI;

public class TutorialDebugUI : MonoBehaviour
{
    [Header("Debug UI")]
    public GameObject debugPanel;
    public Text debugInfoText;
    public Button showTutorialButton;
    public Button resetTutorialButton;
    public Button hideDebugButton;
    
    [Header("Settings")]
    public KeyCode toggleDebugKey = KeyCode.F1;
    public bool showDebugOnStart = false;
    
    private TutorialManager tutorialManager;
    private bool isDebugVisible = false;
    
    void Start()
    {
        // Find tutorial manager
        tutorialManager = FindObjectOfType<TutorialManager>();
        
        // Setup UI
        if (debugPanel != null)
        {
            debugPanel.SetActive(showDebugOnStart);
            isDebugVisible = showDebugOnStart;
        }
        
        // Setup buttons
        if (showTutorialButton != null)
        {
            showTutorialButton.onClick.AddListener(ForceShowTutorial);
        }
        
        if (resetTutorialButton != null)
        {
            resetTutorialButton.onClick.AddListener(ResetTutorialData);
        }
        
        if (hideDebugButton != null)
        {
            hideDebugButton.onClick.AddListener(ToggleDebugPanel);
        }
        
        // Update debug info
        UpdateDebugInfo();
    }
    
    void Update()
    {
        // Toggle debug panel with key
        if (Input.GetKeyDown(toggleDebugKey))
        {
            ToggleDebugPanel();
        }
        
        // Update debug info every frame
        if (isDebugVisible)
        {
            UpdateDebugInfo();
        }
    }
    
    public void ToggleDebugPanel()
    {
        if (debugPanel != null)
        {
            isDebugVisible = !isDebugVisible;
            debugPanel.SetActive(isDebugVisible);
            
            if (isDebugVisible)
            {
                UpdateDebugInfo();
            }
        }
    }
    
    public void UpdateDebugInfo()
    {
        if (debugInfoText == null) return;
        
        string info = "=== TUTORIAL DEBUG INFO ===\n\n";
        
        // Tutorial Data Manager info
        if (TutorialDataManager.Instance != null)
        {
            info += TutorialDataManager.Instance.GetDebugInfo() + "\n\n";
        }
        else
        {
            info += "TutorialDataManager: NOT FOUND\n\n";
        }
        
        // Tutorial Manager info
        if (tutorialManager != null)
        {
            info += $"Tutorial Manager: FOUND\n";
            info += $"Is Active: {tutorialManager.IsTutorialActive}\n";
            info += $"Current Step: {tutorialManager.CurrentStepIndex + 1}/{tutorialManager.TotalSteps}\n";
            info += $"Should Show: {tutorialManager.ShouldShowTutorial()}\n";
            info += $"Force Show: {tutorialManager.forceShowTutorial}\n";
            info += $"Check Completion: {tutorialManager.checkTutorialCompletion}\n";
        }
        else
        {
            info += "Tutorial Manager: NOT FOUND\n";
        }
        
        debugInfoText.text = info;
    }
    
    public void ForceShowTutorial()
    {
        if (tutorialManager != null)
        {
            tutorialManager.forceShowTutorial = true;
            tutorialManager.StartTutorial();
            UpdateDebugInfo();
        }
    }
    
    public void ResetTutorialData()
    {
        if (tutorialManager != null)
        {
            tutorialManager.ResetTutorialData();
            UpdateDebugInfo();
        }
    }
    
    // Public methods for external access
    public void ShowDebugPanel()
    {
        if (debugPanel != null)
        {
            debugPanel.SetActive(true);
            isDebugVisible = true;
            UpdateDebugInfo();
        }
    }
    
    public void HideDebugPanel()
    {
        if (debugPanel != null)
        {
            debugPanel.SetActive(false);
            isDebugVisible = false;
        }
    }
} 