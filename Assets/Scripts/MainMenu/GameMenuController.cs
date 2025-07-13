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
    [SerializeField] private Button introLeftButton;
    [SerializeField] private Button introRightButton;

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
        introLeftButton.onClick.AddListener(() => Debug.Log("Intro Left"));
        introRightButton.onClick.AddListener(() => Debug.Log("Intro Right"));

        // Start state
        optionPanel?.SetActive(false);
        introPanel?.SetActive(false);
        menuContainer?.SetActive(true);
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
        introLeftButton.onClick.RemoveAllListeners();
        introRightButton.onClick.RemoveAllListeners();
    }
}
