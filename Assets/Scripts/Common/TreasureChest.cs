using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

public class TreasureChest : MonoBehaviour
{
    public Animator animator; // Gán animator của rương
    public string openAnimationName = "crazy"; // Tên animation mở rương
    [Header("Interaction")]
    public KeyCode openKey = KeyCode.E; // Phím mở rương
    public bool canOpenMultipleTimes = false;

    [Header("Reward")]
    public ItemData rewardItem; // Kéo item thưởng vào đây nếu muốn cố định
    public List<ItemData> possibleRewards; // Nếu muốn random, thêm vào đây
    public bool randomReward = false;
    public enum RewardType { Item, Coin, Both }
    [Header("Reward Type")]
    public RewardType rewardType = RewardType.Item;
    [Header("Coin Reward")]
    public int coinReward = 0;
    [Header("Coin Popup Icon")]
    public Sprite coinIcon;

    private bool playerInRange = false;
    private bool isOpened = false;
    private bool isDestroying = false; // Thêm biến để tránh destroy nhiều lần

    // Input System
    private InputAction interactAction;

    void Start()
    {
        // Đảm bảo mỗi chest có Animator riêng
        if (animator == null)
            animator = GetComponent<Animator>();
        
        // Tạo Animator instance riêng cho mỗi chest để tránh shared state
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            // Tạo một copy của Animator Controller để tránh shared state
            RuntimeAnimatorController originalController = animator.runtimeAnimatorController;
            
            // Tạo một instance mới của Animator Controller
            animator.runtimeAnimatorController = Instantiate(originalController);
            
            Debug.Log($"Chest {gameObject.name} - Animator: {animator.name}, Controller: {animator.runtimeAnimatorController?.name}");
        }

        // Lấy action từ PlayerController1
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            var playerController = playerObj.GetComponent<PlayerController1>();
            if (playerController != null)
            {
                interactAction = playerController.interactAction;
            }
        }
        
        // Debug: Kiểm tra xem có bao nhiêu TreasureChest trong scene
        TreasureChest[] allChests = FindObjectsOfType<TreasureChest>();
        Debug.Log($"Có {allChests.Length} TreasureChest trong scene");
        
        // Debug: Kiểm tra xem có bao nhiêu ItemPopupUI trong scene
        ItemPopupUI[] allPopups = FindObjectsOfType<ItemPopupUI>();
        Debug.Log($"Có {allPopups.Length} ItemPopupUI trong scene");
        foreach (var popup in allPopups)
        {
            Debug.Log($"Popup found: {popup.name} at position {popup.transform.position}");
        }
    }

    void Update()
    {
        if (playerInRange && (canOpenMultipleTimes || !isOpened) && !isDestroying && interactAction != null && interactAction.WasPressedThisFrame())
        {
            Debug.Log($"E pressed, opening chest: {gameObject.name}");
            OpenChest();
        }
    }

    void OpenChest()
    {
        if (isDestroying) return; // Tránh mở nhiều lần
        
        Debug.Log($"Opening chest: {gameObject.name} at position {transform.position}");
        isOpened = true;
        isDestroying = true;
        
        // Đảm bảo chỉ chest này chạy animation
        if (animator != null)
        {
            animator.Play(openAnimationName);
            Debug.Log($"Playing animation '{openAnimationName}' on chest: {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"No Animator found on chest: {gameObject.name}");
        }
        
        GiveReward();
        StartCoroutine(FadeOutAndDestroy(1f)); // Tan biến dần trong 1 giây
        // Không Destroy(gameObject) ngay lập tức!
    }

    void GiveReward()
    {
        // Nhận tiền nếu có
        if (rewardType == RewardType.Coin || rewardType == RewardType.Both)
        {
            PlayerMoney money = FindObjectOfType<PlayerMoney>();
            if (money != null && coinReward > 0)
            {
                money.AddCoins(coinReward);
                // Hiển thị popup tiền
                ItemPopupUI popup = GetComponentInChildren<ItemPopupUI>(true);
                if (popup != null)
                {
                    popup.ShowCoin(coinReward);
                }
            }
        }
        // Nhận item nếu có
        if (rewardType == RewardType.Item || rewardType == RewardType.Both)
        {
            ItemData itemToGive = null;
            if (randomReward && possibleRewards != null && possibleRewards.Count > 0)
            {
                int idx = Random.Range(0, possibleRewards.Count);
                itemToGive = possibleRewards[idx];
            }
            else
            {
                itemToGive = rewardItem;
            }
            if (itemToGive != null)
            {
                // Tìm Inventory trong scene
                Inventory inventory = FindObjectOfType<Inventory>();
                if (inventory != null)
                {
                    inventory.AddItem(itemToGive);
                    // inventory.EquipItem(itemToGive);
                    Debug.Log($"Bạn nhận được: {itemToGive.itemName}");
                    // Gọi UI popup là con của rương
                    ItemPopupUI popup = GetComponentInChildren<ItemPopupUI>(true); // true để tìm cả object đang bị tắt
                    if (popup != null)
                    {
                        popup.Show(itemToGive);
                    }
                    else
                    {
                        Debug.LogWarning("Không tìm thấy ItemPopupUI là con của rương!");
                    }
                }
                else
                {
                    Debug.LogWarning("Không tìm thấy Inventory trong scene!");
                }
            }
            else
            {
                Debug.LogWarning("Không có item thưởng được thiết lập cho rương này!");
            }
        }
    }

    IEnumerator FadeOutAndDestroy(float duration = 1f)
    {
        Debug.Log($"Starting fade out for chest: {gameObject.name}");
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.Log($"Destroying chest {gameObject.name} - no SpriteRenderer");
            Destroy(gameObject);
            yield break;
        }
        Color originalColor = sr.color;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / duration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        Debug.Log($"Destroying chest: {gameObject.name}");
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log($"Player entered range of chest: {gameObject.name}");
            // TODO: Hiện UI "Nhấn E để mở rương" nếu muốn
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log($"Player exited range of chest: {gameObject.name}");
            // TODO: Ẩn UI "Nhấn E để mở rương" nếu có
        }
    }
    
    // Tìm popup chính xác
    private ItemPopupUI FindClosestPopup()
    {
        // Thử tìm bằng tag trước
        GameObject popupObj = GameObject.FindGameObjectWithTag("ItemPopup");
        if (popupObj != null)
        {
            ItemPopupUI popup = popupObj.GetComponent<ItemPopupUI>();
            if (popup != null)
            {
                Debug.Log($"Found popup by tag: {popup.name}");
                return popup;
            }
        }
        
        // Thử tìm bằng tên cụ thể
        popupObj = GameObject.Find("ItemPopupUI");
        if (popupObj != null)
        {
            ItemPopupUI popup = popupObj.GetComponent<ItemPopupUI>();
            if (popup != null)
            {
                Debug.Log($"Found popup by name: {popup.name}");
                return popup;
            }
        }
        
        // Fallback: tìm popup đầu tiên
        ItemPopupUI[] allPopups = FindObjectsOfType<ItemPopupUI>();
        if (allPopups.Length == 0) return null;
        
        Debug.Log($"Using first popup found: {allPopups[0].name}");
        return allPopups[0];
    }
}