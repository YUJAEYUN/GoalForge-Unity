using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BtnCtrl : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button manualButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button titleButton;

    void Start()
    {
        // Connect buttons if assigned
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnClickStartGame);
            Debug.Log("[BtnCtrl] StartButton connected");
        }
        
        if (manualButton != null)
        {
            manualButton.onClick.AddListener(OnClickManual);
            Debug.Log("[BtnCtrl] ManualButton connected");
        }
        
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnClickExit);
            Debug.Log("[BtnCtrl] ExitButton connected");
        }
        
        if (titleButton != null)
        {
            titleButton.onClick.AddListener(OnClickTitle);
            Debug.Log("[BtnCtrl] TitleButton connected");
        }
    }

    // Start Game: Load MainScene
    public void OnClickStartGame()
    {
        Debug.Log("[BtnCtrl] OnClickStartGame called");
        SceneManager.LoadScene("MainScene");
    }

    // How To Play: Load ManualScene to explain rules
    public void OnClickManual()
    {
        Debug.Log("[BtnCtrl] OnClickManual called");
        SceneManager.LoadScene("ManualScene");
    }

    // Back to Title: Load TitleScene (used in ManualScene)
    public void OnClickTitle()
    {
        Debug.Log("[BtnCtrl] OnClickTitle called");
        SceneManager.LoadScene("TitleScene");
    }

    // Exit Game
    public void OnClickExit()
    {
        Debug.Log("[BtnCtrl] OnClickExit called - Quitting application");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
