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

    private bool playerInRange = false;
    private bool isOpened = false;

    // Input System
    private InputAction interactAction;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

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
    }

    void Update()
    {
        if (playerInRange && (canOpenMultipleTimes || !isOpened) && interactAction != null && interactAction.WasPressedThisFrame())
        {
            Debug.Log("E pressed, opening chest");
            OpenChest();
        }
    }

    void OpenChest()
    {
        isOpened = true;
        animator.Play(openAnimationName);
        GiveReward();
        StartCoroutine(FadeOutAndDestroy(1f)); // Tan biến dần trong 1 giây
        // Không Destroy(gameObject) ngay lập tức!
    }

    void GiveReward()
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
                Debug.Log($"Bạn nhận được: {itemToGive.itemName}");
                // Gọi UI popup nếu có
                var popup = FindObjectOfType<ItemPopupUI>();
                if (popup != null)
                {
                    popup.Show(itemToGive);
                }
                else
                {
                    Debug.LogWarning("Không tìm thấy ItemPopupUI trong scene!");
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

    IEnumerator FadeOutAndDestroy(float duration = 1f)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
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
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // TODO: Hiện UI "Nhấn E để mở rương" nếu muốn
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            // TODO: Ẩn UI "Nhấn E để mở rương" nếu có
        }
    }
}