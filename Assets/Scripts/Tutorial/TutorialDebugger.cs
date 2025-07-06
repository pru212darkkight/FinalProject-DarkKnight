using UnityEngine;
using UnityEngine.UI;

public class TutorialDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool debugOnStart = true;
    public KeyCode forceStartKey = KeyCode.T;
    public KeyCode resetTutorialKey = KeyCode.R;
    public KeyCode resetPositionKey = KeyCode.P;
    public KeyCode forceCompleteKey = KeyCode.C;
    
    void Start()
    {
        if (debugOnStart)
        {
            DebugTutorialSetup();
        }
    }
    
    void Update()
    {
        // Force start tutorial
        if (Input.GetKeyDown(forceStartKey))
        {
            ForceStartTutorial();
        }
        
        // Reset tutorial
        if (Input.GetKeyDown(resetTutorialKey))
        {
            ResetTutorial();
        }
        
        // Reset panel position
        if (Input.GetKeyDown(resetPositionKey))
        {
            ResetTutorialPanelPosition();
        }
        
        // Debug range info (press I key)
        if (Input.GetKeyDown(KeyCode.I))
        {
            DebugRangeInfo();
        }
        
        // Force complete tutorial (press C key)
        if (Input.GetKeyDown(forceCompleteKey))
        {
            ForceCompleteTutorial();
        }
    }
    
    [ContextMenu("Debug Tutorial Setup")]
    public void DebugTutorialSetup()
    {
        Debug.Log("=== TUTORIAL DEBUG START ===");
        
        // 1. Kiểm tra TutorialManager
        TutorialManager tutorialManager = FindObjectOfType<TutorialManager>();
        if (tutorialManager == null)
        {
            Debug.LogError("❌ TutorialManager không tìm thấy trong scene!");
            Debug.Log("💡 Hãy tạo TutorialManager bằng cách:");
            Debug.Log("   1. Tạo empty GameObject");
            Debug.Log("   2. Thêm component TutorialUISetup");
            Debug.Log("   3. Click 'Create Tutorial UI'");
        }
        else
        {
            Debug.Log("✅ TutorialManager tìm thấy: " + tutorialManager.name);
            Debug.Log("   - GameObject active: " + tutorialManager.gameObject.activeInHierarchy);
            Debug.Log("   - Tutorial active: " + tutorialManager.IsTutorialActive);
            Debug.Log("   - Can start tutorial: " + tutorialManager.CanStartTutorial());
            Debug.Log("   - Total steps: " + tutorialManager.TotalSteps);
            Debug.Log("   - Current step: " + tutorialManager.CurrentStepIndex);
            
            // Kiểm tra UI references
            CheckUIReferences(tutorialManager);
        }
        
        // 2. Kiểm tra Map1TutorialSetup
        Map1TutorialSetup tutorialSetup = FindObjectOfType<Map1TutorialSetup>();
        if (tutorialSetup == null)
        {
            Debug.LogError("❌ Map1TutorialSetup không tìm thấy trong scene!");
        }
        else
        {
            Debug.Log("✅ Map1TutorialSetup tìm thấy: " + tutorialSetup.name);
            CheckInputActions(tutorialSetup);
        }
        
        // 3. Kiểm tra Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ Canvas không tìm thấy trong scene!");
            Debug.Log("💡 Tạo Canvas: GameObject → UI → Canvas");
        }
        else
        {
            Debug.Log("✅ Canvas tìm thấy: " + canvas.name);
            Debug.Log("   - Render Mode: " + canvas.renderMode);
            Debug.Log("   - Sort Order: " + canvas.sortingOrder);
        }
        
        // 4. Kiểm tra Tutorial Panel
        GameObject tutorialPanel = GameObject.Find("TutorialPanel");
        if (tutorialPanel == null)
        {
            Debug.LogError("❌ TutorialPanel không tìm thấy!");
        }
        else
        {
            Debug.Log("✅ TutorialPanel tìm thấy: " + tutorialPanel.name);
            Debug.Log("   - Active: " + tutorialPanel.activeInHierarchy);
            
            // Kiểm tra RectTransform
            RectTransform panelRect = tutorialPanel.GetComponent<RectTransform>();
            if (panelRect != null)
            {
                Debug.Log("   - Anchored Position: " + panelRect.anchoredPosition);
                Debug.Log("   - Local Position: " + panelRect.localPosition);
                Debug.Log("   - Anchors: " + panelRect.anchorMin + " to " + panelRect.anchorMax);
                Debug.Log("   - Pivot: " + panelRect.pivot);
                Debug.Log("   - Size Delta: " + panelRect.sizeDelta);
            }
            
            // Kiểm tra CanvasGroup
            CanvasGroup canvasGroup = tutorialPanel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                Debug.Log("   - CanvasGroup Alpha: " + canvasGroup.alpha);
                Debug.Log("   - Interactable: " + canvasGroup.interactable);
                Debug.Log("   - Blocks Raycasts: " + canvasGroup.blocksRaycasts);
            }
        }
        
        // 5. Kiểm tra Player
        PlayerController1 player = FindObjectOfType<PlayerController1>();
        if (player == null)
        {
            Debug.LogError("❌ PlayerController1 không tìm thấy!");
        }
        else
        {
            Debug.Log("✅ PlayerController1 tìm thấy: " + player.name);
        }
        
        Debug.Log("=== TUTORIAL DEBUG END ===");
    }
    
    private void CheckUIReferences(TutorialManager tutorialManager)
    {
        Debug.Log("--- UI References Check ---");
        
        if (tutorialManager.tutorialPanel == null)
            Debug.LogError("❌ Tutorial Panel reference missing!");
        else
            Debug.Log("✅ Tutorial Panel: " + tutorialManager.tutorialPanel.name);
            
        if (tutorialManager.instructionText == null)
            Debug.LogError("❌ Instruction Text reference missing!");
        else
            Debug.Log("✅ Instruction Text: " + tutorialManager.instructionText.name);
            
        if (tutorialManager.inputPromptText == null)
            Debug.LogError("❌ Input Prompt Text reference missing!");
        else
            Debug.Log("✅ Input Prompt Text: " + tutorialManager.inputPromptText.name);
            
        if (tutorialManager.skipButton == null)
            Debug.LogWarning("⚠️ Skip Button reference missing!");
        else
            Debug.Log("✅ Skip Button: " + tutorialManager.skipButton.name);
    }
    
    private void CheckInputActions(Map1TutorialSetup tutorialSetup)
    {
        Debug.Log("--- Input Actions Check ---");
        
        if (tutorialSetup.moveAction == null)
            Debug.LogError("❌ Move Action missing!");
        else
            Debug.Log("✅ Move Action: " + tutorialSetup.moveAction.name);
            
        if (tutorialSetup.jumpAction == null)
            Debug.LogError("❌ Jump Action missing!");
        else
            Debug.Log("✅ Jump Action: " + tutorialSetup.jumpAction.name);
            
        if (tutorialSetup.attackAction == null)
            Debug.LogError("❌ Attack Action missing!");
        else
            Debug.Log("✅ Attack Action: " + tutorialSetup.attackAction.name);
            
        if (tutorialSetup.attack2Action == null)
            Debug.LogError("❌ Attack2 Action missing!");
        else
            Debug.Log("✅ Attack2 Action: " + tutorialSetup.attack2Action.name);
            
        if (tutorialSetup.attack3Action == null)
            Debug.LogError("❌ Attack3 Action missing!");
        else
            Debug.Log("✅ Attack3 Action: " + tutorialSetup.attack3Action.name);
            
        if (tutorialSetup.spell1Action == null)
            Debug.LogError("❌ Spell1 Action missing!");
        else
            Debug.Log("✅ Spell1 Action: " + tutorialSetup.spell1Action.name);
            
        if (tutorialSetup.spell2Action == null)
            Debug.LogError("❌ Spell2 Action missing!");
        else
            Debug.Log("✅ Spell2 Action: " + tutorialSetup.spell2Action.name);
            
        if (tutorialSetup.defendAction == null)
            Debug.LogError("❌ Defend Action missing!");
        else
            Debug.Log("✅ Defend Action: " + tutorialSetup.defendAction.name);
            
        if (tutorialSetup.dashAction == null)
            Debug.LogError("❌ Dash Action missing!");
        else
            Debug.Log("✅ Dash Action: " + tutorialSetup.dashAction.name);
            
        if (tutorialSetup.spell3Action == null)
            Debug.LogError("❌ Spell3 Action missing!");
        else
            Debug.Log("✅ Spell3 Action: " + tutorialSetup.spell3Action.name);
    }
    
    [ContextMenu("Force Start Tutorial")]
    public void ForceStartTutorial()
    {
        TutorialManager tutorialManager = FindObjectOfType<TutorialManager>();
        if (tutorialManager != null)
        {
            Debug.Log("🚀 Force starting tutorial...");
            tutorialManager.StartTutorial();
        }
        else
        {
            Debug.LogError("❌ TutorialManager not found!");
        }
    }
    
    [ContextMenu("Reset Tutorial")]
    public void ResetTutorial()
    {
        TutorialManager tutorialManager = FindObjectOfType<TutorialManager>();
        if (tutorialManager != null)
        {
            Debug.Log("🔄 Resetting tutorial...");
            tutorialManager.ResetTutorial();
        }
        else
        {
            Debug.LogError("❌ TutorialManager not found!");
        }
    }
    
    [ContextMenu("Setup Tutorial UI")]
    public void SetupTutorialUI()
    {
        TutorialUISetup uiSetup = FindObjectOfType<TutorialUISetup>();
        if (uiSetup != null)
        {
            Debug.Log("🔧 Setting up tutorial UI...");
            uiSetup.SetupTutorialInScene();
        }
        else
        {
            Debug.LogError("❌ TutorialUISetup not found!");
            Debug.Log("💡 Create empty GameObject and add TutorialUISetup component");
        }
    }
    
    [ContextMenu("Force Activate Tutorial GameObject")]
    public void ForceActivateTutorial()
    {
        TutorialManager tutorialManager = FindObjectOfType<TutorialManager>();
        if (tutorialManager != null)
        {
            if (!tutorialManager.gameObject.activeInHierarchy)
            {
                Debug.Log("🔧 Activating Tutorial GameObject...");
                tutorialManager.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("✅ Tutorial GameObject is already active");
            }
        }
        else
        {
            Debug.LogError("❌ TutorialManager not found!");
        }
    }
    
    [ContextMenu("Reset Tutorial Panel Position")]
    public void ResetTutorialPanelPosition()
    {
        TutorialManager tutorialManager = FindObjectOfType<TutorialManager>();
        if (tutorialManager != null)
        {
            tutorialManager.ResetPanelPosition();
        }
        else
        {
            Debug.LogError("❌ TutorialManager not found!");
        }
    }
    
    [ContextMenu("Center Tutorial Panel")]
    public void CenterTutorialPanel()
    {
        TutorialManager tutorialManager = FindObjectOfType<TutorialManager>();
        if (tutorialManager != null)
        {
            tutorialManager.SetPanelPosition(new Vector2(0, 200));
        }
        else
        {
            Debug.LogError("❌ TutorialManager not found!");
        }
    }
    
    [ContextMenu("Debug Range Info")]
    public void DebugRangeInfo()
    {
        TutorialManager tutorialManager = FindObjectOfType<TutorialManager>();
        if (tutorialManager != null)
        {
            Debug.Log("=== RANGE DEBUG INFO ===");
            Debug.Log("Current step conditions met: " + tutorialManager.AreCurrentStepConditionsMet());
            Debug.Log("Range info: " + tutorialManager.GetCurrentStepRangeInfo());
            Debug.Log("Is on last step: " + tutorialManager.IsOnLastStep());
            
            // Show current step details
            if (tutorialManager.CurrentStepIndex >= 0 && tutorialManager.CurrentStepIndex < tutorialManager.TotalSteps)
            {
                var currentStep = tutorialManager.tutorialSteps[tutorialManager.CurrentStepIndex];
                Debug.Log($"Current step: {currentStep.stepName}");
                Debug.Log($"Require player in range: {currentStep.requirePlayerInRange}");
                Debug.Log($"Target object: {(currentStep.targetObject != null ? currentStep.targetObject.name : "None")}");
                Debug.Log($"Range distance: {currentStep.rangeDistance}");
            }
            
            // Show player position
            PlayerController1 player = FindObjectOfType<PlayerController1>();
            if (player != null)
            {
                Debug.Log($"Player position: {player.transform.position}");
            }
            
            Debug.Log("=== END RANGE DEBUG ===");
        }
        else
        {
            Debug.LogError("❌ TutorialManager not found!");
        }
    }
    
    [ContextMenu("Force Complete Tutorial")]
    public void ForceCompleteTutorial()
    {
        TutorialManager tutorialManager = FindObjectOfType<TutorialManager>();
        if (tutorialManager != null)
        {
            if (tutorialManager.IsTutorialActive)
            {
                Debug.Log("🚀 Force completing tutorial...");
                tutorialManager.ForceCompleteTutorial();
            }
            else
            {
                Debug.Log("⚠️ Tutorial is not active, cannot force complete");
            }
        }
        else
        {
            Debug.LogError("❌ TutorialManager not found!");
        }
    }
} 