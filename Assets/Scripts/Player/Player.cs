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

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        playerRender = GetComponent<PlayerRenderer>();
        currentHp = maxHp;
    }

    private void Update()
    {
        controller.HandleInput();
        InputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        playerRender.UpdateAnimation(InputDir);
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
