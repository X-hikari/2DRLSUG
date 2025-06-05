using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameplayUI : MonoBehaviour
{
    [Header("UI References")]
    public Slider hpSlider;
    public Slider manaSlider;
    public TextMeshProUGUI levelText;
    public Button pauseButton;

    [Header("Pause Menu")]
    public GameObject pausePanel;
    public Button continueButton;
    public Button mainMenuButton;

    private void Start()
    {
        pauseButton.onClick.AddListener(OnPauseClicked);

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);

        pausePanel.SetActive(false);
        UpdateAll();
    }

    private void Update()
    {
        UpdateHPBar();
        UpdateLevel();
        UpdateMana();
    }

    private void UpdateHPBar()
    {
        var gm = GameManager.Instance;
        hpSlider.maxValue = gm.MaxHP;
        hpSlider.value = gm.CurrentHP;
    }


    private void UpdateLevel()
    {
        levelText.text = $"Lv {GameManager.Instance.CurrentLevel}";
    }
    private void UpdateMana()
    {
        var gm = GameManager.Instance;
        manaSlider.maxValue = gm.MaxMana;
        manaSlider.value = gm.CurrentMana;
    }

    private void OnPauseClicked()
    {
        GameManager.Instance.TogglePause();
        pausePanel.SetActive(GameManager.Instance.IsPaused);
    }

    private void OnContinueClicked()
    {
        GameManager.Instance.TogglePause();
        pausePanel.SetActive(false);
    }

    private void OnMainMenuClicked()
    {
        Time.timeScale = 1f; // 确保主菜单是正常速度
        GameManager.Instance.SaveGame(); // 自动保存
        GameManager.Instance.TogglePause();
        
        SceneManager.LoadScene("MainMenu");
        
    }

    private void UpdateAll()
    {
        UpdateHPBar();
        UpdateLevel();
    }
}
