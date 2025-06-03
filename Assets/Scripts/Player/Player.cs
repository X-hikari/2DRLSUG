using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerRenderer))]
public class Player : MonoBehaviour
{
    public PlayerStatsData playerStatsData;
    public BuffManager buffManager;
    public PlayerStats stats;

    public PlayerStateManager stateManager;

    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public Vector2 lastMoveDir;

    private PlayerController controller;
    private PlayerRenderer playerRender;

    public Vector2 InputDir { get; private set; }

    [Header("武器")]
    public string defaultWeaponName = "Weapon_001";
    public Transform weaponHoldPoint;

    public Weapon[] weapons = new Weapon[2];
    private int currentWeaponIndex = 0;
    private Weapon CurrentWeapon => weapons[currentWeaponIndex];
    private WeaponFactory weaponFactory;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        playerRender = GetComponent<PlayerRenderer>();
        weaponFactory = FindObjectOfType<WeaponFactory>();

        buffManager = new BuffManager(this);
        stats = new PlayerStats(buffManager, playerStatsData);

        stateManager = GetComponent<PlayerStateManager>();

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
        else
        {
            Debug.LogWarning("未能初始化默认武器：WeaponFactory 或默认武器名缺失！");
        }
    }

    private void Update()
    {
        buffManager.Update(Time.deltaTime);
        controller.HandleInput();
        InputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        playerRender.UpdateAnimation(InputDir);

        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchWeapon(1);

        // 测试技能
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
        if (CurrentWeapon == null) return;

        Debug.Log($"丢弃武器：{CurrentWeapon.name}");
        weapons[currentWeaponIndex] = null;

        int otherIndex = (currentWeaponIndex == 0) ? 1 : 0;
        if (weapons[otherIndex] != null)
            SwitchWeapon(otherIndex);
    }

    public void TakeDamage(float damage)
    {
        if (stateManager.IsInvincible) return ;
        bool isDead = stats.TakeDamage(damage);
        Debug.Log("Player HP: " + stats.currentHp);

        if (isDead)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        stats.Heal(amount);
        Debug.Log($"回血: {amount}");
    }

    public void GainExp(int amount)
    {
        bool leveledUp = stats.GainExp(amount);
        Debug.Log($"获得经验 {amount}，当前经验 {stats.currentExp} / {stats.ExpToNextLevel}， 当前等级 {stats.level}");
        if (leveledUp)
        {
            Debug.Log($"升级啦！当前等级: {stats.level}");
        }
    }

    void Die()
    {
        Debug.Log("Player Died");
        stats.currentHp = stats.MaxHp; // 可以复活或其他处理
    }
}
