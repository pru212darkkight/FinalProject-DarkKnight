using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TutorialStep
{
    [Header("Tutorial Step Info")]
    public string stepName;
    [TextArea(3, 6)]
    public string instructionText;
    public Sprite instructionImage; // Optional image to show
    
    [Header("Input Requirements")]
    public InputAction requiredInput; // The input that must be pressed
    public string inputDisplayName; // Display name for the input (e.g., "WASD", "SPACE", "LEFT CLICK")
    public bool requireHold = false; // Whether the input needs to be held
    public float holdDuration = 1f; // How long to hold if requireHold is true
    
    [Header("Visual Settings")]
    public Vector2 tutorialPanelPosition = Vector2.zero; // Position of tutorial panel
    public bool showArrow = false; // Whether to show an arrow pointing to something
    public Vector2 arrowPosition = Vector2.zero; // Position of the arrow
    public Vector3 arrowRotation = Vector3.zero; // Rotation of the arrow
    
    [Header("Conditions")]
    public bool requirePlayerGrounded = false; // Whether player must be grounded
    public bool requirePlayerInRange = false; // Whether player must be in specific range
    public Transform targetObject; // Object player must be near
    public float rangeDistance = 2f; // Distance for range check
}

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial Settings")]
    public List<TutorialStep> tutorialSteps = new List<TutorialStep>();
    public bool startTutorialOnStart = true;
    public bool canSkipTutorial = true;
    public bool hideTutorialOnComplete = true; // Whether to hide tutorial panel when completed
    public bool disableTutorialOnComplete = true; // Whether to disable tutorial GameObject when completed
    public bool checkTutorialCompletion = true; // Whether to check if tutorial was already completed
    public bool forceShowTutorial = false; // Force show tutorial even if completed (for testing)
    
    [Header("UI References")]
    public GameObject tutorialPanel;
    public Text instructionText;
    public Image instructionImage;
    public Text inputPromptText;
    public GameObject arrowObject;
    public Button skipButton;
    public Button nextButton;
    
    [Header("Visual Settings")]
    public Color panelBackgroundColor = new Color(0, 0, 0, 0.8f);
    public Color textColor = Color.white;
    public float panelFadeInDuration = 0.5f;
    public float panelFadeOutDuration = 0.3f;
    
    [Header("Audio")]
    public AudioClip stepCompleteSound;
    public AudioClip tutorialStartSound;
    public AudioClip tutorialEndSound;
    
    // Private variables
    private int currentStepIndex = -1;
    private bool isTutorialActive = false;
    private bool isWaitingForInput = false;
    private float holdTimer = 0f;
    private AudioSource audioSource;
    private CanvasGroup panelCanvasGroup;
    private PlayerController1 playerController;
    
    // Events
    public System.Action OnTutorialStart;
    public System.Action OnTutorialComplete;
    public System.Action OnTutorialStepComplete;
    
    void Start()
    {
        InitializeTutorial();
        
        if (startTutorialOnStart)
        {
            // Check if tutorial should be shown
            if (ShouldShowTutorial())
            {
                // Ensure GameObject is active before starting tutorial
                if (gameObject != null && !gameObject.activeInHierarchy)
                {
                    gameObject.SetActive(true);
                }
                StartTutorial();
            }
            else
            {
                // Tutorial already completed, disable this GameObject
                if (disableTutorialOnComplete)
                {
                    gameObject.SetActive(false);
                }
                Debug.Log("Tutorial already completed, skipping...");
            }
        }
    }
    
    void InitializeTutorial()
    {
        // Get components
        audioSource = GetComponent<AudioSource>();
        if (tutorialPanel != null)
        {
            panelCanvasGroup = tutorialPanel.GetComponent<CanvasGroup>();
            if (panelCanvasGroup == null)
            {
                panelCanvasGroup = tutorialPanel.AddComponent<CanvasGroup>();
            }
        }
        
        // Find player
        playerController = FindObjectOfType<PlayerController1>();
        
        // Setup UI
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
        
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipTutorial);
        }
        
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(NextStep);
            nextButton.gameObject.SetActive(false);
        }
        
        // Setup input callbacks for all tutorial steps
        foreach (var step in tutorialSteps)
        {
            if (step.requiredInput != null)
            {
                step.requiredInput.performed += (context) => OnRequiredInputPressed(context, step);
                if (step.requireHold)
                {
                    step.requiredInput.canceled += (context) => OnRequiredInputReleased(context, step);
                }
            }
        }
    }
    
    public void StartTutorial()
    {
        if (isTutorialActive) return;
        
        // Check if tutorial should be shown (unless force show is enabled)
        if (checkTutorialCompletion && !forceShowTutorial && !ShouldShowTutorial())
        {
            Debug.Log("Tutorial already completed, cannot start again");
            return;
        }
        
        // Ensure the GameObject is active
        if (gameObject != null && !gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
        
        isTutorialActive = true;
        currentStepIndex = -1;
        
        // Play start sound
        if (audioSource != null && tutorialStartSound != null)
        {
            audioSource.PlayOneShot(tutorialStartSound);
        }
        
        // Invoke event
        OnTutorialStart?.Invoke();
        
        // Start first step
        NextStep();
    }
    
    public void NextStep()
    {
        currentStepIndex++;
        
        if (currentStepIndex >= tutorialSteps.Count)
        {
            CompleteTutorial();
            return;
        }
        
        ShowCurrentStep();
    }
    
    private void ShowCurrentStep()
    {
        // Ensure the GameObject is active before starting coroutines
        if (gameObject != null && !gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
        
        TutorialStep currentStep = tutorialSteps[currentStepIndex];
        
        // Update UI
        if (instructionText != null)
        {
            instructionText.text = currentStep.instructionText;
        }
        
        if (instructionImage != null && currentStep.instructionImage != null)
        {
            instructionImage.sprite = currentStep.instructionImage;
            instructionImage.gameObject.SetActive(true);
        }
        else if (instructionImage != null)
        {
            instructionImage.gameObject.SetActive(false);
        }
        
        // Update input prompt
        if (inputPromptText != null)
        {
            string promptText = $"Press: {currentStep.inputDisplayName}";
            if (currentStep.requireHold)
            {
                promptText += $" (Hold for {currentStep.holdDuration}s)";
            }
            inputPromptText.text = promptText;
        }
        
        // Position tutorial panel
        if (tutorialPanel != null)
        {
            // Use anchoredPosition for UI elements instead of localPosition
            RectTransform panelRect = tutorialPanel.GetComponent<RectTransform>();
            if (panelRect != null)
            {
                panelRect.anchoredPosition = currentStep.tutorialPanelPosition;
            }
            else
            {
                tutorialPanel.transform.localPosition = currentStep.tutorialPanelPosition;
            }
        }
        
        // Show/hide arrow
        if (arrowObject != null)
        {
            arrowObject.SetActive(currentStep.showArrow);
            if (currentStep.showArrow)
            {
                // Use anchoredPosition for UI elements
                RectTransform arrowRect = arrowObject.GetComponent<RectTransform>();
                if (arrowRect != null)
                {
                    arrowRect.anchoredPosition = currentStep.arrowPosition;
                    arrowRect.localRotation = Quaternion.Euler(currentStep.arrowRotation);
                }
                else
                {
                    arrowObject.transform.localPosition = currentStep.arrowPosition;
                    arrowObject.transform.localRotation = Quaternion.Euler(currentStep.arrowRotation);
                }
            }
        }
        
        // Show tutorial panel
        StartCoroutine(ShowTutorialPanel());
        
        // Setup input waiting
        SetupInputWaiting(currentStep);
    }
    
    private void SetupInputWaiting(TutorialStep step)
    {
        isWaitingForInput = true;
        holdTimer = 0f;
        
        // Enable the required input
        if (step.requiredInput != null)
        {
            step.requiredInput.Enable();
        }
        
        // Show next button if no input is required
        if (nextButton != null && step.requiredInput == null)
        {
            nextButton.gameObject.SetActive(true);
        }
    }
    
    private void OnRequiredInputPressed(InputAction.CallbackContext context, TutorialStep step)
    {
        if (!isWaitingForInput || step != tutorialSteps[currentStepIndex]) return;
        
        // Check conditions before proceeding
        if (!CheckStepConditions(step)) return;
        
        if (step.requireHold)
        {
            // Start hold timer
            holdTimer = 0f;
        }
        else
        {
            // Complete step immediately
            CompleteCurrentStep();
        }
    }
    
    private void OnRequiredInputReleased(InputAction.CallbackContext context, TutorialStep step)
    {
        if (!isWaitingForInput || step != tutorialSteps[currentStepIndex]) return;
        
        // Check conditions before proceeding
        if (!CheckStepConditions(step)) return;
        
        if (step.requireHold)
        {
            // Reset hold timer if released too early
            holdTimer = 0f;
        }
    }
    
    void Update()
    {
        if (!isWaitingForInput || currentStepIndex < 0 || currentStepIndex >= tutorialSteps.Count) return;
        
        TutorialStep currentStep = tutorialSteps[currentStepIndex];
        
        // Check conditions
        if (!CheckStepConditions(currentStep)) return;
        
        // Handle hold input
        if (currentStep.requireHold && currentStep.requiredInput != null && currentStep.requiredInput.IsPressed())
        {
            holdTimer += Time.deltaTime;
            
            // Update progress text
            if (inputPromptText != null)
            {
                float progress = Mathf.Clamp01(holdTimer / currentStep.holdDuration);
                string promptText = $"Hold: {currentStep.inputDisplayName} ({progress:P0})";
                inputPromptText.text = promptText;
            }
            
            if (holdTimer >= currentStep.holdDuration)
            {
                CompleteCurrentStep();
            }
        }
    }
    
    private bool CheckStepConditions(TutorialStep step)
    {
        if (playerController == null) return true;
        
        // Check if player must be grounded
        if (step.requirePlayerGrounded)
        {
            // You'll need to expose isGrounded from PlayerController1 or use a different method
            // For now, we'll assume it's accessible
            // if (!playerController.IsGrounded) return false;
        }
        
        // Check if player must be in range
        if (step.requirePlayerInRange && step.targetObject != null)
        {
            float distance = Vector2.Distance(playerController.transform.position, step.targetObject.position);
            if (distance > step.rangeDistance) 
            {
                return false;
            }
        }
        
        return true;
    }
    
    private void CompleteCurrentStep()
    {
        if (!isWaitingForInput) return;
        
        isWaitingForInput = false;
        
        // Disable current input
        if (currentStepIndex < tutorialSteps.Count)
        {
            TutorialStep currentStep = tutorialSteps[currentStepIndex];
            if (currentStep.requiredInput != null)
            {
                currentStep.requiredInput.Disable();
            }
        }
        
        // Play completion sound
        if (audioSource != null && stepCompleteSound != null)
        {
            audioSource.PlayOneShot(stepCompleteSound);
        }
        
        // Invoke event
        OnTutorialStepComplete?.Invoke();
        
        // Auto-advance to next step after a short delay
        if (gameObject != null && gameObject.activeInHierarchy)
        {
            StartCoroutine(AutoAdvanceStep());
        }
    }
    
    private System.Collections.IEnumerator AutoAdvanceStep()
    {
        yield return new WaitForSeconds(0.5f);
        NextStep();
    }
    
    private System.Collections.IEnumerator ShowTutorialPanel()
    {
        if (tutorialPanel == null) yield break;
        
        tutorialPanel.SetActive(true);
        
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0f;
            float elapsed = 0f;
            
            while (elapsed < panelFadeInDuration)
            {
                elapsed += Time.deltaTime;
                panelCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / panelFadeInDuration);
                yield return null;
            }
            
            panelCanvasGroup.alpha = 1f;
        }
    }
    
    private System.Collections.IEnumerator HideTutorialPanel()
    {
        if (tutorialPanel == null) yield break;
        
        if (panelCanvasGroup != null)
        {
            float elapsed = 0f;
            
            while (elapsed < panelFadeOutDuration)
            {
                elapsed += Time.deltaTime;
                panelCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / panelFadeOutDuration);
                yield return null;
            }
            
            panelCanvasGroup.alpha = 0f;
        }
        
        tutorialPanel.SetActive(false);
    }
    
    private System.Collections.IEnumerator DisableTutorialAfterDelay()
    {
        // Wait for panel fade out to complete
        yield return new WaitForSeconds(panelFadeOutDuration + 0.5f);
        
        // Disable the entire tutorial GameObject
        if (gameObject != null)
        {
            gameObject.SetActive(false);
        }
    }
    
    public void SkipTutorial()
    {
        if (!canSkipTutorial) return;
        
        CompleteTutorial();
    }
    
    private void CompleteTutorial()
    {
        isTutorialActive = false;
        isWaitingForInput = false;
        
        // Mark tutorial as completed in data manager
        if (checkTutorialCompletion)
        {
            TutorialDataManager.Instance.MarkTutorialCompleted();
        }
        
        // Disable all inputs
        foreach (var step in tutorialSteps)
        {
            if (step.requiredInput != null)
            {
                step.requiredInput.Disable();
            }
        }
        
        // Play completion sound
        if (audioSource != null && tutorialEndSound != null)
        {
            audioSource.PlayOneShot(tutorialEndSound);
        }
        
        // Hide tutorial panel if enabled
        if (hideTutorialOnComplete)
        {
            StartCoroutine(HideTutorialPanel());
        }
        
        // Disable tutorial GameObject if enabled
        if (disableTutorialOnComplete)
        {
            StartCoroutine(DisableTutorialAfterDelay());
        }
        
        // Invoke event
        OnTutorialComplete?.Invoke();
        
        Debug.Log("Tutorial completed and saved!");
    }
    
    // Public methods for external control
    public bool IsTutorialActive => isTutorialActive;
    public int CurrentStepIndex => currentStepIndex;
    public int TotalSteps => tutorialSteps.Count;
    
    // Method to add tutorial steps programmatically
    public void AddTutorialStep(TutorialStep step)
    {
        tutorialSteps.Add(step);
    }
    
    // Method to reset tutorial
    public void ResetTutorial()
    {
        currentStepIndex = -1;
        isTutorialActive = false;
        isWaitingForInput = false;
        
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }
    
    // Helper method to ensure GameObject is active
    private void EnsureGameObjectActive()
    {
        if (gameObject != null && !gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
    }
    
    // Public method to check if tutorial can run
    public bool CanStartTutorial()
    {
        return gameObject != null && gameObject.activeInHierarchy;
    }
    
    // Method to reset tutorial panel position
    public void ResetPanelPosition()
    {
        if (tutorialPanel != null)
        {
            RectTransform panelRect = tutorialPanel.GetComponent<RectTransform>();
            if (panelRect != null)
            {
                panelRect.anchoredPosition = Vector2.zero;
            }
        }
    }
    
    // Method to set tutorial panel position
    public void SetPanelPosition(Vector2 position)
    {
        if (tutorialPanel != null)
        {
            RectTransform panelRect = tutorialPanel.GetComponent<RectTransform>();
            if (panelRect != null)
            {
                panelRect.anchoredPosition = position;
            }
        }
    }
    
    // Public method to check if current step conditions are met
    public bool AreCurrentStepConditionsMet()
    {
        if (currentStepIndex < 0 || currentStepIndex >= tutorialSteps.Count) return false;
        return CheckStepConditions(tutorialSteps[currentStepIndex]);
    }
    
    // Public method to get current step range info
    public string GetCurrentStepRangeInfo()
    {
        if (currentStepIndex < 0 || currentStepIndex >= tutorialSteps.Count) return "No active step";
        
        TutorialStep step = tutorialSteps[currentStepIndex];
        if (!step.requirePlayerInRange || step.targetObject == null || playerController == null)
            return "No range requirement";
            
        float distance = Vector2.Distance(playerController.transform.position, step.targetObject.position);
        return $"Distance: {distance:F2}, Required: {step.rangeDistance:F2}, In Range: {distance <= step.rangeDistance}";
    }
    
    // Public method to force complete tutorial
    public void ForceCompleteTutorial()
    {
        if (isTutorialActive)
        {
            CompleteTutorial();
        }
    }
    
    // Public method to check if tutorial is on last step
    public bool IsOnLastStep()
    {
        return isTutorialActive && currentStepIndex == tutorialSteps.Count - 1;
    }
    
    // Public method to check if tutorial should be shown
    public bool ShouldShowTutorial()
    {
        if (forceShowTutorial) return true;
        if (!checkTutorialCompletion) return true;
        return TutorialDataManager.Instance.ShouldShowTutorial();
    }
    
    // Public method to force reset tutorial data
    public void ResetTutorialData()
    {
        TutorialDataManager.Instance.ResetTutorialData();
        Debug.Log("Tutorial data reset - will show tutorial again on next start");
    }
    
    // Public method to get tutorial debug info
    public string GetTutorialDebugInfo()
    {
        return TutorialDataManager.Instance.GetDebugInfo();
    }
    
    void OnDestroy()
    {
        // Clean up input callbacks
        foreach (var step in tutorialSteps)
        {
            if (step.requiredInput != null)
            {
                step.requiredInput.performed -= (context) => OnRequiredInputPressed(context, step);
                if (step.requireHold)
                {
                    step.requiredInput.canceled -= (context) => OnRequiredInputReleased(context, step);
                }
            }
        }
    }
} 