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

    // ��� ��New Game��
    public void OnNewGame()
    {
        mainMenuPanel.SetActive(false);
        newGamePanel.SetActive(true);
    }

    // ��� ��Back������ NewGamePanel��
    public void OnBackFromNewGame()
    {
        newGamePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // ��� ��Continue��
    public void OnContinue()
    {
        // TODO: ���ش浵�߼�
    }

    // ��� ��Settings��
    public void OnSettings()
    {
        // TODO: ���������
    }

    // ��� ��Exit Game��
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
