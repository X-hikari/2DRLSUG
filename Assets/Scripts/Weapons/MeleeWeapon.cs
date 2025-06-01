using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class MeleeWeapon : Weapon
{
    public float damageCooldown = 0.5f; // 对同一敌人造成伤害的最小时间间隔

    private Transform player;
    private Collider2D weaponCollider;

    private Dictionary<Enemy, float> lastDamageTime = new(); // 记录每个敌人上次被击中的时间

    protected override void Awake()
    {
        base.Awake();

        weaponCollider = GetComponent<Collider2D>();
        weaponCollider.isTrigger = true;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("未找到带有 Player 标签的玩家对象！");
        }

        // 设置旋转角速度
        if (rotateDuration > 0)
            rotationSpeed = 360f / rotateDuration;
    }

    protected override void Update()
    {
        if (player == null || data == null) return;

        // 围绕玩家旋转
        transform.RotateAround(player.position, Vector3.forward, rotationSpeed * Time.deltaTime);

        // 始终面朝外圈方向
        Vector2 dir = (transform.position - player.position).normalized;
        transform.right = dir;
    }

    public override void Attack(Vector2 direction)
    {
        // 不需要使用主控调用 Attack，这里留空
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(enemyTag) || data == null) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;

        if (CanDamage(enemy))
        {
            enemy.TakeDamage(data.damage);
            lastDamageTime[enemy] = Time.time;
        }
    }

    private bool CanDamage(Enemy enemy)
    {
        if (!lastDamageTime.ContainsKey(enemy)) return true;
        return Time.time - lastDamageTime[enemy] >= damageCooldown;
    }
}
