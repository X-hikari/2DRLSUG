using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerRenderer))]
public class Player : MonoBehaviour
{
    public PlayerStatsData playerStatsData;
    public BuffManager buffManager;
    public PlayerStats stats;
    public PlayerStateManager stateManager;

    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public Vector2 lastMoveDir;

    [Header("武器")]
    public string defaultWeaponName = "Weapon_001";
    public Transform weaponHoldPoint;

    public Weapon[] weapons = new Weapon[2];
    private int currentWeaponIndex = 0;
    private WeaponFactory weaponFactory;

    [Header("技能")]
    public SkillData[] ownSkills = new SkillData[6];

    private void Awake()
    {
        weaponFactory = FindObjectOfType<WeaponFactory>();
        buffManager = new BuffManager(this);
        stats = new PlayerStats(buffManager, playerStatsData);
        stateManager = GetComponent<PlayerStateManager>();
    }

    public void Init(string defaultWeaponName)
    {
        // 暂时无法选择初始技能
        if (weaponFactory != null && !string.IsNullOrEmpty(defaultWeaponName))
        {
            Weapon weapon = weaponFactory.CreateWeapon(defaultWeaponName, weaponHoldPoint);
            if (weapon != null)
            {
                weapons[0] = weapon;
                currentWeaponIndex = 0;
                Debug.Log($"默认武器已创建：{weapon.name}");
            }
        }
        stats.Init();
        Debug.Log("玩家初始化完成");
    }

    public void SetStatus(PlayerStatus status, bool value)
    {
        stateManager?.SetStatus(status, value);
    }

    public bool HasStatus(PlayerStatus status)
    {
        return stateManager != null && stateManager.HasStatus(status);
    }

    public void SwitchWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length || weapons[index] == null) return;
        currentWeaponIndex = index;
        Debug.Log($"切换到武器 {index + 1}: {weapons[index].name}");
    }

    public bool PickUpWeapon(Weapon newWeapon)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == null)
            {
                weapons[i] = newWeapon;
                Debug.Log($"拾取武器：{newWeapon.name}");
                return true;
            }
        }
        Debug.Log("武器栏已满，无法拾取！");
        return false;
    }

    public void DropCurrentWeapon()
    {
        if (weapons[currentWeaponIndex] == null) return;

        Debug.Log($"丢弃武器：{weapons[currentWeaponIndex].name}");
        weapons[currentWeaponIndex] = null;

        int otherIndex = (currentWeaponIndex == 0) ? 1 : 0;
        if (weapons[otherIndex] != null)
            SwitchWeapon(otherIndex);
    }

    public void TakeDamage(float damage)
    {
        if (stateManager.IsInvincible) return;
        bool isDead = stats.TakeDamage(damage);
        Debug.Log("Player HP: " + stats.currentHp);

        if (isDead) Die();
    }

    public void Heal(int amount)
    {
        stats.Heal(amount);
        Debug.Log($"回血: {amount}");
    }

    public void GainExp(int amount)
    {
        bool leveledUp = stats.GainExp(amount);
        Debug.Log($"获得经验 {amount}，当前经验 {stats.currentExp} / {stats.ExpToNextLevel}，当前等级 {stats.level}");
        if (leveledUp)
        {
            Debug.Log($"升级啦！当前等级: {stats.level}");
            stats.LevelUP();
        }
    }

    void Die()
    {
        Debug.Log("Player Died");
        stats.currentHp = stats.MaxHp; // 复活或其他处理
    }
}
