using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 全局游戏管理器。记录选中角色与武器、玩家状态数据，并在游戏中管理进度。
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // —— 角色 & 武器（由主菜单设置） ——
    [HideInInspector] public PlayerStatsData SelectedPlayer;
    [HideInInspector] public WeaponData SelectedWeapon;

    // —— 游戏中状态数据（UI可读取） ——
    public int CurrentHP { get; private set; }
    public int MaxHP { get; private set; }
    public int CurrentLevel { get; private set; }
    public int CurrentExp { get; private set; }

    // —— 游戏运行状态 ——
    public bool IsPaused { get; private set; }

    private void Awake()
    {
        // 单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 场景载入后初始化数据（如进入Gameplay）
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Gameplay") // 替换为实际名称
        {
            InitGameData();
        }
    }

    /// <summary>
    /// 初始化游戏内状态（从角色数据读取初始值）
    /// </summary>
    private void InitGameData()
    {
        if (SelectedPlayer != null)
        {
            MaxHP = SelectedPlayer.baseMaxHp;
            CurrentHP = MaxHP;
            CurrentLevel = SelectedPlayer.baseLevel;
            CurrentExp = SelectedPlayer.baseExp;
        }
        else
        {
            Debug.LogWarning("GameManager: 未设置角色数据！");
        }

        IsPaused = false;
    }

    /// <summary>
    /// 伤害处理
    /// </summary>
    public void TakeDamage(int amount)
    {
        CurrentHP = Mathf.Max(CurrentHP - amount, 0);
        if (CurrentHP == 0)
        {
            Debug.Log("Player 死亡");
            // 后续可扩展 Game Over
        }
    }

    /// <summary>
    /// 增加经验
    /// </summary>
    public void GainExp(int amount)
    {
        CurrentExp += amount;
        // 示例升级机制（可换成更复杂公式）
        int expToNext = 100 + 20 * (CurrentLevel - 1);
        while (CurrentExp >= expToNext)
        {
            CurrentExp -= expToNext;
            CurrentLevel++;
            Debug.Log($"升级到等级 {CurrentLevel}！");
            expToNext = 100 + 20 * (CurrentLevel - 1);
        }
    }

    /// <summary>
    /// 暂停或恢复游戏（UI按钮调用）
    /// </summary>
    public void TogglePause()
    {
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
    }
}
