using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("主菜单面板")]
    public GameObject mainMenuPanel;     // 包含 “新游戏”、“继续”、“设置”、“退出” 按钮
    public Button newGameButton;
    public Button continueButton;
    public Button settingsButton;
    public Button exitButton;

    [Header("新游戏总面板")]
    public GameObject newGamePanel;      // 新游戏面板（包含主选择和武器选择子面板）
    public Button backToMenuButton;      // 新游戏面板里 返回 主菜单 按钮

    [Header("主选择面板 (MainSelect)")]
    public GameObject mainSelectPanel;   // 包含“选择角色”、“选择武器”、“开始游戏”、“返回主菜单”
    public Button selectCharacterButton;
    public Button selectWeaponButton;
    public Button startGameButton;

    // 主选择面板里展示已选武器的 Image + Text
    public Image mainSelectWeaponImage;
    public TextMeshProUGUI mainSelectWeaponName;

    [Header("武器选择面板 (WeaponSelect)")]
    public GameObject weaponSelectPanel; // 包含 左、右 切换按钮 和 确认按钮
    public Button weaponLeftButton;
    public Button weaponRightButton;
    public Button weaponConfirmButton;

    // 武器选择面板里展示武器详情的 UI
    public Image weaponPreviewImage;
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI weaponStatsText;

    // 所有从 Resources/Data/Weapons 加载到的 WeaponData
    private List<WeaponData> weaponList = new List<WeaponData>();
    private int currentWeaponIndex = 0;
    private WeaponData selectedWeapon;   // 确认后的武器

    void Start()
    {
        // —— 主菜单按钮绑定 —— 
        newGameButton.onClick.AddListener(OnNewGame);
        continueButton.onClick.AddListener(OnContinue);
        settingsButton.onClick.AddListener(OnSettings);
        exitButton.onClick.AddListener(OnExit);

        // —— 新游戏子面板 按钮绑定 —— 
        backToMenuButton.onClick.AddListener(OnBackToMainMenu);
        selectCharacterButton.onClick.AddListener(OnEnterCharacterSelect); // 若无角色选，可留空
        selectWeaponButton.onClick.AddListener(OnEnterWeaponSelect);
        startGameButton.onClick.AddListener(OnStartGame);

        // —— 武器选择面板 按钮绑定 —— 
        weaponLeftButton.onClick.AddListener(() => ChangeWeaponSelection(-1));
        weaponRightButton.onClick.AddListener(() => ChangeWeaponSelection(1));
        weaponConfirmButton.onClick.AddListener(OnConfirmWeaponSelect);

        // 默认只显示主菜单
        ShowMainMenu();

        // 加载 Resources/Data/Weapons 下的所有武器数据
        LoadAllWeapons();
    }

    /// <summary>
    /// 显示主菜单，隐藏其他所有子面板
    /// </summary>
    private void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);

        newGamePanel.SetActive(false);
        mainSelectPanel.SetActive(false);
        weaponSelectPanel.SetActive(false);
       // charaSelectPanel?.SetActive(false); // 如果实现角色选择，可保留
    }

    /// <summary>
    /// 点击 “新游戏”：隐藏主菜单，显示 新游戏 + 主选择面板
    /// </summary>
    private void OnNewGame()
    {
        mainMenuPanel.SetActive(false);
        newGamePanel.SetActive(true);

        mainSelectPanel.SetActive(true);
        weaponSelectPanel.SetActive(false);
       // charaSelectPanel?.SetActive(false);

        // 刷新主选择面板里的武器预览（若已有 selectedWeapon）
        UpdateMainSelectWeaponPreview();
    }

    /// <summary>
    /// 点击 “继续”——此处示例为日志，按需补充存档逻辑
    /// </summary>
    private void OnContinue()
    {
        Debug.Log("Continue: 尚未实现存档加载");
    }

    /// <summary>
    /// 点击 “设置”——示例为日志，按需弹出设置界面
    /// </summary>
    private void OnSettings()
    {
        Debug.Log("Settings: 尚未实现设置面板");
    }

    /// <summary>
    /// 点击 “退出”——编辑器模式停止，打包后退出
    /// </summary>
    private void OnExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// 点击 新游戏面板里的 “返回主菜单”
    /// </summary>
    private void OnBackToMainMenu()
    {
        ShowMainMenu();
    }

    /// <summary>
    /// 点击 主选择面板 的 “选择角色”（此示例不实现角色选择，仅作占位）
    /// </summary>
    private void OnEnterCharacterSelect()
    {
        Debug.Log("Enter Character Select: 尚未实现");
        // mainSelectPanel.SetActive(false);
        // charaSelectPanel.SetActive(true);
    }

    /// <summary>
    /// 点击 主选择面板 的 “选择武器”
    /// </summary>
    private void OnEnterWeaponSelect()
    {
        mainSelectPanel.SetActive(false);
        weaponSelectPanel.SetActive(true);

        // 刷新武器选择面板显示当前待确认的武器
        UpdateWeaponSelectUI();
    }

    /// <summary>
    /// 点击 武器选择面板 的 “确认” 按钮
    /// </summary>
    private void OnConfirmWeaponSelect()
    {
        if (weaponList.Count > 0)
        {
            selectedWeapon = weaponList[currentWeaponIndex];
        }
        weaponSelectPanel.SetActive(false);
        mainSelectPanel.SetActive(true);

        // 更新主选择面板里的武器预览
        UpdateMainSelectWeaponPreview();
    }

    /// <summary>
    /// 点击 主选择面板 的 “开始游戏”
    /// </summary>
    private void OnStartGame()
    {
        if (selectedWeapon == null)
        {
            Debug.LogWarning("请先选择一把武器！");
            return;
        }
        Debug.Log($"开始游戏，使用武器：{selectedWeapon.weaponName}");
        // SceneManager.LoadScene("Gameplay"); // 根据项目改为实际场景名
    }

    // （若实现角色选择，这里可补充 OnConfirmCharacterSelect 等）

    // ============================
    // === WeaponData 加载与渲染 ===
    // ============================

    /// <summary>
    /// 从 Resources/Data/Weapons 中加载所有 WeaponData
    /// </summary>
    private void LoadAllWeapons()
    {
        weaponList.Clear();
        WeaponData[] loaded = Resources.LoadAll<WeaponData>("Data/Weapons");
        if (loaded != null && loaded.Length > 0)
        {
            weaponList.AddRange(loaded);
            currentWeaponIndex = 0;
            selectedWeapon = weaponList[0];
        }
        else
        {
            Debug.LogWarning("未在 Resources/Data/Weapons 中找到任何 WeaponData！");
        }
    }

    /// <summary>
    /// 切换当前待确认武器索引，并刷新选择面板 UI
    /// </summary>
    /// <param name="delta">-1 表示上一把，+1 表示下一把</param>
    private void ChangeWeaponSelection(int delta)
    {
        if (weaponList.Count == 0) return;
        currentWeaponIndex = (currentWeaponIndex + delta + weaponList.Count) % weaponList.Count;
        UpdateWeaponSelectUI();
    }

    /// <summary>
    /// 更新武器选择面板上的预览图、名称、属性
    /// </summary>
    private void UpdateWeaponSelectUI()
    {
        if (weaponList.Count == 0) return;
        RenderWeaponPreview(
            weaponList[currentWeaponIndex],
            weaponPreviewImage,
            weaponNameText,
            weaponStatsText
        );
    }

    /// <summary>
    /// 更新主选择面板上已确认武器的预览图+名称
    /// </summary>
    private void UpdateMainSelectWeaponPreview()
    {
        if (selectedWeapon == null) return;
        RenderWeaponPreview(
            selectedWeapon,
            mainSelectWeaponImage,
            mainSelectWeaponName,
            null    // 主选择面板仅显示图标+名称，无需属性文本
        );
    }

    /// <summary>
    /// 公用方法：将 WeaponData 渲染到指定的 Image+Text 组件上
    ///   参数顺序与页面上 UI 出现顺序一致：预览图 → 名称 → 属性
    /// </summary>
    private void RenderWeaponPreview(
        WeaponData weaponData,
        Image previewImage,
        TextMeshProUGUI nameText,
        TextMeshProUGUI statsText
    )
    {
        if (weaponData == null) return;

        // 1. 预览图：优先使用 idleSprite，否则用 icon
        if (previewImage != null)
        {
            previewImage.sprite = (weaponData.idleSprite != null) ? weaponData.idleSprite : weaponData.icon;
        }

        // 2. 名称
        if (nameText != null)
        {
            nameText.text = weaponData.weaponName;
        }

        // 3. 属性（可选）
        if (statsText != null)
        {
            statsText.text =
                $"伤害: {weaponData.damage}\n" +
                $"攻速: {weaponData.attackInterval:F2}秒\n" +
                $"射程: {weaponData.attackRange:F1}";
        }
    }
}
