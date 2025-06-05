using System.IO;
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
    public int CurrentHP { get;  set; }
    public int MaxHP { get;  set; }
    public int CurrentLevel { get;  set; }
    public int CurrentExp { get;  set; }
    public float MaxMana { get; set; }
    public float CurrentMana { get; set; }

    // —— 游戏运行状态 ——
    public bool IsPaused { get; private set; }
    private string savePath => Application.persistentDataPath + "/save.json";

    public void SaveGame()
    {

        
        SaveData data = new SaveData
        {
            playerId = SelectedPlayer.name,   // 或唯一ID
            weaponId = SelectedWeapon.name,   // 或唯一ID
            hp = CurrentHP,
            maxHp = MaxHP,
            level = CurrentLevel
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
        Debug.Log($"游戏已保存到 {savePath}");
    }

    public bool HasSave() => File.Exists(savePath);

    public void LoadGame()
    {
        if (!HasSave()) return;

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // 加载数据到 GameManager
        SelectedPlayer = LoadPlayerById(data.playerId);
        SelectedWeapon = LoadWeaponById(data.weaponId);
        CurrentHP = data.hp;
        MaxHP = data.maxHp;
        CurrentLevel = data.level;
    }

    private PlayerStatsData LoadPlayerById(string id)
    {
        // 可从 Resources、ScriptableObject 管理器等加载
        return Resources.Load<PlayerStatsData>("Data/Players/" + id);
    }

    private WeaponData LoadWeaponById(string id)
    {
        return Resources.Load<WeaponData>("Data/Weapons/" + id);
    }

    public void ClearSave()
    {
        if (HasSave()) File.Delete(savePath);
    }
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
        BuffConditionRegister.RegisterAll();
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
            // InitGameData();
        }
    }

    // /// <summary>
    // /// 初始化游戏内状态（从角色数据读取初始值）
    // /// </summary>
    // private void InitGameData()
    // {
    //     if (SelectedPlayer != null)
    //     {
    //         MaxHP = SelectedPlayer.baseMaxHp;
    //         CurrentHP = MaxHP;
    //         CurrentLevel = SelectedPlayer.baseLevel;
    //         CurrentExp = SelectedPlayer.baseExp;
    //     }
    //     else
    //     {
    //         Debug.LogWarning("GameManager: 未设置角色数据！");
    //     }

    //     IsPaused = false;
    // }

    /// <summary>
    /// 暂停或恢复游戏（UI按钮调用）
    /// </summary>
    public void TogglePause()
    {
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
    }
}
