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
    public GameObject newGamePanel;      // 新游戏面板（包含主选择和子面板）
    public Button backToMenuButton;      // 新游戏面板里 “返回主菜单” 按钮

    [Header("主选择面板 (MainSelect)")]
    public GameObject mainSelectPanel;   // 包含 “选择角色”、“选择武器”、“开始游戏”
    public Button selectCharacterButton;
    public Button selectWeaponButton;
    public Button startGameButton;

    // 主选择面板里展示已选角色和武器的 Image + Text
    public Image mainSelectCharacterImage;
    public TextMeshProUGUI mainSelectCharacterName;
    public Image mainSelectWeaponImage;
    public TextMeshProUGUI mainSelectWeaponName;

    [Header("武器选择面板 (WeaponSelect)")]
    public GameObject weaponSelectPanel; // 包含 左、右 切换按钮 和 “确认” 按钮
    public Button weaponLeftButton;
    public Button weaponRightButton;
    public Button weaponConfirmButton;

    // 武器选择面板里显示武器预览
    public Image weaponPreviewImage;
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI weaponStatsText;

    [Header("角色选择面板 (CharacterSelect)")]
    public GameObject charaSelectPanel;  // 包含 左、右 切换按钮 和 “确认” 按钮
    public Button charaLeftButton;
    public Button charaRightButton;
    public Button charaConfirmButton;

    // 角色选择面板里显示角色预览
    public Image charaPreviewImage;
    public TextMeshProUGUI charaNameText;
    public TextMeshProUGUI charaStatsText;

    // 已加载的所有 WeaponData
    private List<WeaponData> weaponList = new List<WeaponData>();
    private int currentWeaponIndex = 0;
    private WeaponData selectedWeapon;

    // 已加载的所有 PlayerStatsData
    private List<PlayerStatsData> playerList = new List<PlayerStatsData>();
    private int currentPlayerIndex = 0;
    private PlayerStatsData selectedPlayer;

    void Start()
    {
        // —— 主菜单按钮绑定 —— 
        newGameButton.onClick.AddListener(OnNewGame);
        continueButton.onClick.AddListener(OnContinue);
        settingsButton.onClick.AddListener(OnSettings);
        exitButton.onClick.AddListener(OnExit);

        // —— 新游戏子面板 按钮绑定 —— 
        backToMenuButton.onClick.AddListener(OnBackToMainMenu);
        selectCharacterButton.onClick.AddListener(OnEnterCharacterSelect);
        selectWeaponButton.onClick.AddListener(OnEnterWeaponSelect);
        startGameButton.onClick.AddListener(OnStartGame);

        // —— 武器选择面板 按钮绑定 —— 
        weaponLeftButton.onClick.AddListener(() => ChangeWeaponSelection(-1));
        weaponRightButton.onClick.AddListener(() => ChangeWeaponSelection(1));
        weaponConfirmButton.onClick.AddListener(OnConfirmWeaponSelect);

        // —— 角色选择面板 按钮绑定 —— 
        charaLeftButton.onClick.AddListener(() => ChangePlayerSelection(-1));
        charaRightButton.onClick.AddListener(() => ChangePlayerSelection(1));
        charaConfirmButton.onClick.AddListener(OnConfirmCharacterSelect);

        // 默认只显示主菜单
        ShowMainMenu();

        // 加载 Resources/Data/Weapons 下的武器数据
        LoadAllWeapons();

        // 加载 Resources/Data/Players 下的角色数据
        LoadAllPlayers();
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
        charaSelectPanel.SetActive(false);
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
        charaSelectPanel.SetActive(false);

        // 刷新主选择面板里的 角色 & 武器 预览
        UpdateMainSelectCharacterPreview();
        UpdateMainSelectWeaponPreview();
    }

    /// <summary>
    /// 点击 “继续”——此处示例为日志，按需补充存档逻辑
    /// </summary>
    private void OnContinue()
    {
        GameManager.Instance.LoadGame();
        SceneManager.LoadScene("Gameplay");
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
    /// 点击 主选择面板 的 “选择角色”
    /// </summary>
    private void OnEnterCharacterSelect()
    {
        mainSelectPanel.SetActive(false);
        charaSelectPanel.SetActive(true);

        // 进入角色选择时刷新显示
        UpdateCharacterSelectUI();
    }

    /// <summary>
    /// 点击 主选择面板 的 “选择武器”
    /// </summary>
    private void OnEnterWeaponSelect()
    {
        mainSelectPanel.SetActive(false);
        weaponSelectPanel.SetActive(true);

        // 进入武器选择时刷新显示
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
        GameManager.Instance.SelectedWeapon = selectedWeapon;
        weaponSelectPanel.SetActive(false);
        mainSelectPanel.SetActive(true);

        // 更新主选择面板里的武器预览
        UpdateMainSelectWeaponPreview();
    }

    /// <summary>
    /// 点击 角色选择面板 的 “确认” 按钮
    /// </summary>
    private void OnConfirmCharacterSelect()
    {
        if (playerList.Count > 0)
        {
            selectedPlayer = playerList[currentPlayerIndex];
        }
        GameManager.Instance.SelectedPlayer = selectedPlayer;
        charaSelectPanel.SetActive(false);
        mainSelectPanel.SetActive(true);

        // 更新主选择面板里的角色预览
        UpdateMainSelectCharacterPreview();
    }

    /// <summary>
    /// 点击 主选择面板 的 “开始游戏”
    /// </summary>
    private void OnStartGame()
    {
        if (selectedPlayer == null)
        {
            Debug.LogWarning("请先选择一个角色！");
            return;
        }
        if (selectedWeapon == null)
        {
            Debug.LogWarning("请先选择一把武器！");
            return;
        }
        Debug.Log($"开始游戏，角色：{selectedPlayer.playerName}，武器：{selectedWeapon.weaponName}");
         SceneManager.LoadScene("Gameplay"); // 根据项目改为实际场景名
    }

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
    /// 刷新 武器选择面板 上的预览图、名称、属性
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
    /// 刷新 主选择面板 上 已确认 武器 的预览 (图标+名称)
    /// </summary>
    private void UpdateMainSelectWeaponPreview()
    {
        if (selectedWeapon == null) return;
        RenderWeaponPreview(
            selectedWeapon,
            mainSelectWeaponImage,
            mainSelectWeaponName,
            null    // 主选择面板 无需显示属性文本
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
            previewImage.sprite = weaponData.idleSprite != null ? weaponData.idleSprite : weaponData.icon;

        // 2. 名称
        if (nameText != null)
            nameText.text = weaponData.weaponName;

        // 3. 属性（可选）
        if (statsText != null)
        {
            statsText.text =
                $"伤害: {weaponData.damage}\n" +
                $"攻速: {weaponData.attackInterval:F2}秒\n" +
                $"射程: {weaponData.attackRange:F1}";
        }
    }

    // ===============================
    // === PlayerStatsData 加载与渲染 ===
    // ===============================

    /// <summary>
    /// 从 Resources/Data/Players 中加载所有 PlayerStatsData
    /// </summary>
    private void LoadAllPlayers()
    {
        playerList.Clear();
        PlayerStatsData[] loaded = Resources.LoadAll<PlayerStatsData>("Data/Players");
        if (loaded != null && loaded.Length > 0)
        {
            playerList.AddRange(loaded);
            currentPlayerIndex = 0;
            selectedPlayer = playerList[0];
        }
        else
        {
            Debug.LogWarning("未在 Resources/Data/Players 中找到任何 PlayerStatsData！");
        }
    }

    /// <summary>
    /// 切换当前待确认角色索引，并刷新角色选择面板 UI
    /// </summary>
    /// <param name="delta">-1 表示上一位，+1 表示下一位</param>
    private void ChangePlayerSelection(int delta)
    {
        if (playerList.Count == 0) return;
        currentPlayerIndex = (currentPlayerIndex + delta + playerList.Count) % playerList.Count;
        UpdateCharacterSelectUI();
    }

    /// <summary>
    /// 刷新 角色选择面板 上的预览图、名称、属性
    /// </summary>
    private void UpdateCharacterSelectUI()
    {
        if (playerList.Count == 0) return;
        RenderCharacterPreview(
            playerList[currentPlayerIndex],
            charaPreviewImage,
            charaNameText,
            charaStatsText
        );
    }

    /// <summary>
    /// 刷新 主选择面板 上 已确认 角色 的预览 (图标+名称)
    /// </summary>
    private void UpdateMainSelectCharacterPreview()
    {
        if (selectedPlayer == null) return;
        RenderCharacterPreview(
            selectedPlayer,
            mainSelectCharacterImage,
            mainSelectCharacterName,
            null    // 主选择面板 仅显示图标+名称
        );
    }

    /// <summary>
    /// 公用方法：将 PlayerStatsData 渲染到指定的 Image+Text 组件上
    ///   参数顺序与页面上 UI 出现顺序一致：预览图 → 名称 → 属性
    /// </summary>
    private void RenderCharacterPreview(
        PlayerStatsData playerData,
        Image previewImage,
        TextMeshProUGUI nameText,
        TextMeshProUGUI statsText
    )
    {
        if (playerData == null) return;

        // 1. 预览图：优先使用 idleSprite，否则用 icon
        if (previewImage != null)
            previewImage.sprite = playerData.idleSprite != null ? playerData.idleSprite : playerData.icon;

        // 2. 名称
        if (nameText != null)
            nameText.text = playerData.playerName;

        // 3. 属性（可选）
        if (statsText != null)
        {
            statsText.text =
                $"HP: {playerData.baseMaxHp}\n" +
                $"攻击: {playerData.baseAttack}\n" +
                $"移动速度: {playerData.baseMoveSpeed:F1}";
        }
    }
}
