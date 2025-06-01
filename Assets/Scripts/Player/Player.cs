using System.Reflection;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerRenderer))]
public class Player : MonoBehaviour
{
    public float moveSpeed = 3f;
    public int maxHp = 100;
    public int currentHp;

    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public Vector2 lastMoveDir;

    private PlayerController controller;
    private PlayerRenderer playerRender;

    public Vector2 InputDir { get; private set; }

    // 新增经验值和等级
    public int level = 1;
    public int currentExp = 0;
    // 升级所需经验
    public int ExpToNextLevel => 100 * level;

    // 武器相关
    [Header("武器")]
    public string defaultWeaponName = "Weapon_001"; // 默认武器名
    public Transform weaponHoldPoint; // 武器生成位置（建议空物体）

    public Weapon[] weapons = new Weapon[2];
    private int currentWeaponIndex = 0;
    private Weapon CurrentWeapon => weapons[currentWeaponIndex];

    private WeaponFactory weaponFactory;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        playerRender = GetComponent<PlayerRenderer>();
        weaponFactory = FindObjectOfType<WeaponFactory>(); // 场景中有一个 WeaponFactory 单例

        currentHp = maxHp;

        // 初始化默认武器
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
        controller.HandleInput();
        InputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        playerRender.UpdateAnimation(InputDir);

        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchWeapon(1);
    }

    public void SwitchWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;
        if (weapons[index] == null) return;

        currentWeaponIndex = index;
        Debug.Log($"切换到武器 {index + 1}: {weapons[index].name}");
        // 可以触发换武器动画或UI更新
    }

    // 拾取武器，返回是否成功（比如槽满了就失败）
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

    // 丢弃当前武器
    public void DropCurrentWeapon()
    {
        if (CurrentWeapon == null) return;

        Debug.Log($"丢弃武器：{CurrentWeapon.name}");
        // 实际游戏中可以实例化武器掉落物，或做隐藏处理

        weapons[currentWeaponIndex] = null;
        // 自动切换到另一把武器或空手
        int otherIndex = (currentWeaponIndex == 0) ? 1 : 0;
        if (weapons[otherIndex] != null)
            SwitchWeapon(otherIndex);
    }

    public void TakeDamage(float damage)
    {
        currentHp -= (int)damage;
        Debug.Log("Player HP: " + currentHp);

        if (currentHp <= 0)
        {
            Die();
        }
    }

    public void GainExp(int amount)
    {
        currentExp += amount;
        Debug.Log($"获得经验 {amount}，当前经验 {currentExp} / {ExpToNextLevel}， 当前等级 {level}");

        while (currentExp >= ExpToNextLevel)
        {
            currentExp -= ExpToNextLevel;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        Debug.Log($"升级啦！当前等级: {level}");
        // 这里可以扩展升级时增加属性等逻辑
    }

    void Die()
    {
        Debug.Log("Player Died");
        currentHp = 100;
    }
}
