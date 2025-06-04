using UnityEngine;

/// <summary>
/// 全局游戏管理。负责存储玩家在菜单中选定的角色和武器，并跨场景保存。
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // 菜单中选好的角色/武器
    [HideInInspector] public PlayerStatsData SelectedPlayer;
    [HideInInspector] public WeaponData SelectedWeapon;

    private void Awake()
    {
        // 单例处理
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
