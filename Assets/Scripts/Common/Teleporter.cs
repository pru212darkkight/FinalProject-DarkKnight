using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{
    [Header("Teleporter Settings")]
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private KeyCode openKey = KeyCode.E;
    [SerializeField] private bool canUseMultipleTimes = true;
    
    [Header("Visual Effects")]
    [SerializeField] private Animator teleporterAnimator;
    [SerializeField] private string disappearAnimationTrigger = "Disappear";
    [SerializeField] private float disappearDelay = 1f;
    
    [Header("UI")]
    [SerializeField] private GameObject levelSelectionUI;
    [SerializeField] private string levelSelectionUITag = "LevelSelectionUI";
    [SerializeField] private bool useSimpleUI = false; // Nếu true, sẽ tìm SimpleLevelSelectionUI thay vì LevelSelectionUI
    
    [Header("Level Options")]
    [SerializeField] private string[] availableLevels = {
        "Map 1 - Tin",
        "Map 2 - Han", 
        "Map 3 - Tuyen",
        "Map 4 - Phuong",
        "Map 5 - Hau"
    };
    
    [Header("Level Names for UI")]
    [SerializeField] private string[] levelDisplayNames = {
        "Map 1 - Tin",
        "Map 2 - Han",
        "Map 3 - Tuyen", 
        "Map 4 - Phuong",
        "Map 5 - Hau"
    };
    
    private bool playerInRange = false;
    private bool isUsed = false;
    private bool isDisappearing = false;
    private PlayerController1 playerController;
    private InputAction interactAction;
    
    void Start()
    {
        // Tìm player và gán Input Action
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController1>();
            if (playerController != null)
            {
                interactAction = playerController.interactAction;
            }
        }
        
        // Tìm UI nếu chưa được gán
        if (levelSelectionUI == null)
        {
            levelSelectionUI = GameObject.FindGameObjectWithTag(levelSelectionUITag);
        }
        
        // Nếu không tìm thấy UI với tag, thử tìm với tên
        if (levelSelectionUI == null)
        {
            if (useSimpleUI)
            {
                levelSelectionUI = GameObject.Find("SimpleLevelSelectionUI");
            }
            else
            {
                levelSelectionUI = GameObject.Find("LevelSelectionUI");
            }
        }
        
        // Ẩn UI ban đầu
        if (levelSelectionUI != null)
        {
            levelSelectionUI.SetActive(false);
        }
        
        // Tìm Animator nếu chưa được gán
        if (teleporterAnimator == null)
        {
            teleporterAnimator = GetComponent<Animator>();
        }
        
        Debug.Log($"Teleporter {gameObject.name} initialized - UI found: {levelSelectionUI != null}, Animator found: {teleporterAnimator != null}");
    }
    
    void Update()
    {
        if (playerInRange && (canUseMultipleTimes || !isUsed) && !isDisappearing && interactAction != null && interactAction.WasPressedThisFrame())
        {
            Debug.Log($"E pressed, activating teleporter: {gameObject.name}");
            ActivateTeleporter();
        }
    }
    
    void ActivateTeleporter()
    {
        if (isDisappearing) return;
        
        isUsed = true;
        isDisappearing = true;
        
        Debug.Log($"Activating teleporter: {gameObject.name}");
        
        //// Chạy animation biến mất
        //if (teleporterAnimator != null)
        //{
        //    teleporterAnimator.SetTrigger(disappearAnimationTrigger);
        //    Debug.Log($"Playing disappear animation for teleporter: {gameObject.name}");
        //}
        
        // Hiện UI chọn màn sau một khoảng thời gian
        Invoke(nameof(ShowLevelSelectionUI), disappearDelay);
    }
    
    void ShowLevelSelectionUI()
    {
        if (levelSelectionUI != null)
        {
            levelSelectionUI.SetActive(true);
            
            if (useSimpleUI)
            {
                // Sử dụng SimpleLevelSelectionUI
                SimpleLevelSelectionUI simpleUI = levelSelectionUI.GetComponent<SimpleLevelSelectionUI>();
                if (simpleUI != null)
                {
                    simpleUI.ShowUI();
                }
                else
                {
                    Debug.LogWarning("SimpleLevelSelectionUI component not found on levelSelectionUI GameObject");
                }
            }
            else
            {
                // Sử dụng LevelSelectionUI
                LevelSelectionUI levelUI = levelSelectionUI.GetComponent<LevelSelectionUI>();
                if (levelUI != null)
                {
                    levelUI.SetupLevels(availableLevels, levelDisplayNames);
                    levelUI.ShowUI();
                }
                else
                {
                    Debug.LogWarning("LevelSelectionUI component not found on levelSelectionUI GameObject");
                }
            }
            
            Debug.Log($"Level selection UI activated for teleporter: {gameObject.name}");
        }
        else
        {
            Debug.LogError($"Level selection UI not found for teleporter: {gameObject.name}");
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log($"Player entered teleporter range: {gameObject.name}");
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log($"Player exited teleporter range: {gameObject.name}");
        }
    }
    
    // Public method để ẩn teleporter (có thể gọi từ animation event)
    public void HideTeleporter()
    {
        gameObject.SetActive(false);
        Debug.Log($"Teleporter {gameObject.name} hidden");
    }
    
    // Public method để reset teleporter (nếu cần)
    public void ResetTeleporter()
    {
        isUsed = false;
        isDisappearing = false;
        gameObject.SetActive(true);
        
        if (teleporterAnimator != null)
        {
            teleporterAnimator.ResetTrigger(disappearAnimationTrigger);
        }
        
        Debug.Log($"Teleporter {gameObject.name} reset");
    }
} 