using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayUI : MonoBehaviour
{
    [Header("UI References")]
    public Slider hpSlider;
    public TextMeshProUGUI levelText;
    public Button pauseButton;
    public GameObject pausePanel; // 可选：暂停界面

    private void Start()
    {
        pauseButton.onClick.AddListener(OnPauseClicked);
        UpdateAll();
    }

    private void Update()
    {
        UpdateHPBar();
        UpdateLevel();
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

    private void OnPauseClicked()
    {
        GameManager.Instance.TogglePause();
        if (pausePanel != null)
            pausePanel.SetActive(GameManager.Instance.IsPaused);
    }

    private void UpdateAll()
    {
        UpdateHPBar();
        UpdateLevel();
    }
}
