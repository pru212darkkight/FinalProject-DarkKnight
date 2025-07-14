using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenuController : MonoBehaviour
{
    [Header("Main Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button introButton;
    [SerializeField] private Button exitButton;

    [Header("Panels")]
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject introPanel;
    [SerializeField] private GameObject menuContainer;

    [Header("Option Panel Controls")]
    [SerializeField] private Button optionOkButton;
    [SerializeField] private Button optionLeftButton;
    [SerializeField] private Button optionRightButton;

    [Header("Intro Panel Controls")]
    [SerializeField] private Button introOkButton;
    [SerializeField] private Button intro1Button;
    [SerializeField] private Button intro2Button;
    [SerializeField] private Button intro3Button;

    [Header("Intro Contents")]
    [SerializeField] private GameObject introContent1;
    [SerializeField] private GameObject introContent2;
    [SerializeField] private GameObject introContent3;
    private void Start()
    {
        // Main Menu buttons
        playButton.onClick.AddListener(OnPlayClick);
        optionButton.onClick.AddListener(OnOptionClick);
        introButton.onClick.AddListener(OnIntroClick);
        exitButton.onClick.AddListener(OnExitClick);

        // Option Panel
        optionOkButton.onClick.AddListener(() => ClosePanel(optionPanel));
        optionLeftButton.onClick.AddListener(() => Debug.Log("Option Left"));
        optionRightButton.onClick.AddListener(() => Debug.Log("Option Right"));

        // Introduction Panel
        introOkButton.onClick.AddListener(() => ClosePanel(introPanel));
        intro1Button.onClick.AddListener(() => ShowIntroContent(1));
        intro2Button.onClick.AddListener(() => ShowIntroContent(2));
        intro2Button.onClick.AddListener(() => ShowIntroContent(3));

        // Start state
        optionPanel?.SetActive(false);
        introPanel?.SetActive(false);
        menuContainer?.SetActive(true);
        ShowIntroContent(1); // Mặc định hiển thị content 1
    }

    private void OnPlayClick()
    {
        SceneManager.LoadScene("Home Village");
    }

    private void OnOptionClick()
    {
        optionPanel?.SetActive(true);
        menuContainer?.SetActive(false);
    }

    private void OnIntroClick()
    {
        introPanel?.SetActive(true);
        menuContainer?.SetActive(false);
    }

    private void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ClosePanel(GameObject panel)
    {
        panel?.SetActive(false);
        menuContainer?.SetActive(true);
    }

    private void ShowIntroContent(int contentIndex)
    {
        if (introContent1 != null) introContent1.SetActive(contentIndex == 1);
        if (introContent2 != null) introContent2.SetActive(contentIndex == 2);
        if (introContent3 != null) introContent3.SetActive(contentIndex == 3);
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveAllListeners();
        optionButton.onClick.RemoveAllListeners();
        introButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();

        optionOkButton.onClick.RemoveAllListeners();
        optionLeftButton.onClick.RemoveAllListeners();
        optionRightButton.onClick.RemoveAllListeners();

        introOkButton.onClick.RemoveAllListeners();
        intro1Button.onClick.RemoveAllListeners();
        intro2Button.onClick.RemoveAllListeners();
    }
    public void ShowIntro1()
    {
        ShowIntroContent(1);
    }

    public void ShowIntro2()
    {
        ShowIntroContent(2);
    }
    public void ShowIntro3()
    {
        ShowIntroContent(3);
    }
    public void CloseIntroPanel()
    {
        ClosePanel(introPanel);
    }

}
