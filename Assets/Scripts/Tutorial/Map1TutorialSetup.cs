using UnityEngine;
using UnityEngine.InputSystem;

public class Map1TutorialSetup : MonoBehaviour
{
    [Header("Tutorial Manager Reference")]
    public TutorialManager tutorialManager;
    
    [Header("Player Input Actions")]
    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction attackAction;
    public InputAction attack2Action;
    public InputAction attack3Action;
    public InputAction spell1Action;
    public InputAction spell2Action;
    public InputAction defendAction;
    public InputAction dashAction;
    public InputAction spell3Action;
    
    [Header("Tutorial Objects")]
    public Transform enemyTarget; // Enemy to attack
    public Transform platformTarget; // Platform to jump on
    public Transform spellTarget; // Target for spell casting
    
    void Start()
    {
        if (tutorialManager == null)
        {
            tutorialManager = FindObjectOfType<TutorialManager>();
        }
        
        if (tutorialManager != null)
        {
            SetupTutorialSteps();
        }
        else
        {
            Debug.LogError("TutorialManager not found!");
        }
    }
    
    void SetupTutorialSteps()
    {
        // Clear existing steps
        // tutorialManager.tutorialSteps.Clear();
        
        // // Step 1: Welcome and Movement
        // TutorialStep step1 = new TutorialStep
        // {
        //     stepName = "Welcome",
        //     instructionText = "Welcome to the game! Use the right arrow key to move the character forward.",
        //     inputDisplayName = "Right Arrow",
        //     requiredInput = moveAction,
        //     tutorialPanelPosition = new Vector2(61.41f, 86.45f),
        //     requireHold = false
        // };
        // tutorialManager.AddTutorialStep(step1);
        
        // // Step 2: Jumping
        // TutorialStep step2 = new TutorialStep
        // {
        //     stepName = "Jumping",
        //     instructionText = "Press the up arrow key to jump. Try jumping onto the stone in front of you!",
        //     inputDisplayName = "Up Arrow",
        //     requiredInput = jumpAction,
        //     tutorialPanelPosition = new Vector2(61.41f, 86.45f),
        //     showArrow = true,
        //     arrowPosition = new Vector2(0, 100),
        //     arrowRotation = new Vector3(0, 0, 45),
        //     requirePlayerInRange = true,
        //     targetObject = platformTarget,
        //     rangeDistance = 1f
        // };
        // tutorialManager.AddTutorialStep(step2);
        
        // // Step 3: Basic Attack
        // TutorialStep step3 = new TutorialStep
        // {
        //     stepName = "Basic Attack",
        //     instructionText = "Press the Z key to perform a basic attack. Attack the enemy in front of you!",
        //     inputDisplayName = "Z",
        //     requiredInput = attackAction,
        //     tutorialPanelPosition = new Vector2(61.41f, 86.45f),
        //     showArrow = true,
        //     arrowPosition = new Vector2(0, 100),
        //     arrowRotation = new Vector3(0, 0, 0),
        //     requirePlayerInRange = true,
        //     targetObject = enemyTarget,
        //     rangeDistance = 0.5f
        // };
        // tutorialManager.AddTutorialStep(step3);
        
        // // Step 4: Attack 2
        // TutorialStep step4 = new TutorialStep
        // {
        //     stepName = "Attack 2",
        //     instructionText = "Press the X key to perform a stronger attack with a longer range.",
        //     inputDisplayName = "X",
        //     requiredInput = attack2Action,
        //     tutorialPanelPosition = new Vector2(61.41f, 86.45f),
        //     requireHold = false
        // };
        // tutorialManager.AddTutorialStep(step4);
        
       
        
        // // Step 5: Defend
        // TutorialStep step5 = new TutorialStep
        // {
        //     stepName = "Defend",
        //     instructionText = "Hold the Space key to defend. While defending, you will not take damage!",
        //     inputDisplayName = "Space",
        //     requiredInput = defendAction,
        //     tutorialPanelPosition = new Vector2(61.41f, 86.45f),
        //     requireHold = true,
        //     holdDuration = 1.5f
        // };
        // tutorialManager.AddTutorialStep(step5);
        
        // // Step 6: Dash
        // TutorialStep step6 = new TutorialStep
        // {
        //     stepName = "Dash",
        //     instructionText = "Press the Shift key to dash forward. Dash consumes stamina but allows you to move faster!",
        //     inputDisplayName = "Shift",
        //     requiredInput = dashAction,
        //     tutorialPanelPosition = new Vector2(61.41f, 86.45f),
        //     requireHold = false
        // };
        // tutorialManager.AddTutorialStep(step6);
        
        // // Step 7: Final Instructions
        // TutorialStep step7 = new TutorialStep
        // {
        //     stepName = "Final Instructions",
        //     instructionText = "Awesome! You have learned all the basic skills. Now, explore the world and defeat the enemies!",
        //     inputDisplayName = "",
        //     requiredInput = null,
        //     tutorialPanelPosition = new Vector2(61.41f, 86.45f),
        //     requireHold = false
        // };
        // tutorialManager.AddTutorialStep(step7);
        
        Debug.Log($"Setup {tutorialManager.tutorialSteps.Count} tutorial steps for Map 1");
    }
    
    // Public method to manually start tutorial
    public void StartTutorial()
    {
        if (tutorialManager != null)
        {
            tutorialManager.StartTutorial();
        }
    }
    
    // Public method to reset tutorial
    public void ResetTutorial()
    {
        if (tutorialManager != null)
        {
            tutorialManager.ResetTutorial();
            SetupTutorialSteps();
        }
    }
    
    // Method to check if tutorial is active
    public bool IsTutorialActive()
    {
        return tutorialManager != null && tutorialManager.IsTutorialActive;
    }
} 