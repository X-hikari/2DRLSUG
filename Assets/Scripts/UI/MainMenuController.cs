using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject newGamePanel;

    void Start()
    {
        ShowMainMenu();
    }

    // 点击 “New Game”
    public void OnNewGame()
    {
        mainMenuPanel.SetActive(false);
        newGamePanel.SetActive(true);
    }

    // 点击 “Back”（在 NewGamePanel）
    public void OnBackFromNewGame()
    {
        newGamePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // 点击 “Continue”
    public void OnContinue()
    {
        // TODO: 加载存档逻辑
    }

    // 点击 “Settings”
    public void OnSettings()
    {
        // TODO: 打开设置面板
    }

    // 点击 “Exit Game”
    public void OnExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    private void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        newGamePanel.SetActive(false);
    }
}
