using UnityEngine;

public class TutorialDataManager : MonoBehaviour
{
    private static TutorialDataManager instance;
    public static TutorialDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TutorialDataManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("TutorialDataManager");
                    instance = go.AddComponent<TutorialDataManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    // Keys for PlayerPrefs
    private const string TUTORIAL_COMPLETED_KEY = "TutorialCompleted";
    private const string TUTORIAL_STEP_KEY = "TutorialStep";
    private const string TUTORIAL_VERSION_KEY = "TutorialVersion";

    // Current tutorial version - change this when you update the tutorial
    private const int CURRENT_TUTORIAL_VERSION = 1;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Kiểm tra xem tutorial đã được hoàn thành chưa
    /// </summary>
    /// <returns>True nếu tutorial đã hoàn thành</returns>
    public bool IsTutorialCompleted()
    {
        // Kiểm tra version để reset tutorial nếu có update
        int savedVersion = PlayerPrefs.GetInt(TUTORIAL_VERSION_KEY, 0);
        if (savedVersion < CURRENT_TUTORIAL_VERSION)
        {
            ResetTutorialData();
            return false;
        }

        return PlayerPrefs.GetInt(TUTORIAL_COMPLETED_KEY, 0) == 1;
    }

    /// <summary>
    /// Đánh dấu tutorial đã hoàn thành
    /// </summary>
    public void MarkTutorialCompleted()
    {
        PlayerPrefs.SetInt(TUTORIAL_COMPLETED_KEY, 1);
        PlayerPrefs.SetInt(TUTORIAL_VERSION_KEY, CURRENT_TUTORIAL_VERSION);
        PlayerPrefs.Save();
        Debug.Log("Tutorial marked as completed");
    }

    /// <summary>
    /// Lưu step hiện tại của tutorial
    /// </summary>
    /// <param name="stepIndex">Index của step hiện tại</param>
    public void SaveTutorialStep(int stepIndex)
    {
        PlayerPrefs.SetInt(TUTORIAL_STEP_KEY, stepIndex);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Lấy step cuối cùng đã lưu
    /// </summary>
    /// <returns>Index của step cuối cùng, -1 nếu chưa có</returns>
    public int GetLastTutorialStep()
    {
        return PlayerPrefs.GetInt(TUTORIAL_STEP_KEY, -1);
    }

    /// <summary>
    /// Reset tất cả dữ liệu tutorial
    /// </summary>
    public void ResetTutorialData()
    {
        PlayerPrefs.DeleteKey(TUTORIAL_COMPLETED_KEY);
        PlayerPrefs.DeleteKey(TUTORIAL_STEP_KEY);
        PlayerPrefs.SetInt(TUTORIAL_VERSION_KEY, CURRENT_TUTORIAL_VERSION);
        PlayerPrefs.Save();
        Debug.Log("Tutorial data reset");
    }

    /// <summary>
    /// Kiểm tra xem có cần hiển thị tutorial không
    /// </summary>
    /// <returns>True nếu cần hiển thị tutorial</returns>
    public bool ShouldShowTutorial()
    {
        return !IsTutorialCompleted();
    }

    /// <summary>
    /// Force hiển thị tutorial (bỏ qua trạng thái đã hoàn thành)
    /// </summary>
    public void ForceShowTutorial()
    {
        ResetTutorialData();
    }

    /// <summary>
    /// Lấy thông tin debug về trạng thái tutorial
    /// </summary>
    /// <returns>String chứa thông tin debug</returns>
    public string GetDebugInfo()
    {
        return $"Tutorial Completed: {IsTutorialCompleted()}\n" +
               $"Last Step: {GetLastTutorialStep()}\n" +
               $"Version: {PlayerPrefs.GetInt(TUTORIAL_VERSION_KEY, 0)}/{CURRENT_TUTORIAL_VERSION}\n" +
               $"Should Show: {ShouldShowTutorial()}";
    }
} 