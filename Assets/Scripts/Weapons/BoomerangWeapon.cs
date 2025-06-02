using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class BoomerangWeapon : Weapon
{
    private bool isStabbing = false;
    private Collider2D weaponCollider;
    private Vector3 originalLocalPosition;
    private Player player;

    private HashSet<Enemy> hitEnemies = new(); // 当前阶段已命中的敌人

    protected override void Awake()
    {
        base.Awake();
        weaponCollider = GetComponent<Collider2D>();
        if (weaponCollider == null)
        {
            weaponCollider = gameObject.AddComponent<BoxCollider2D>();
            Debug.LogWarning("自动为武器添加了 BoxCollider2D 组件");
        }
        weaponCollider.enabled = false;
        originalLocalPosition = transform.localPosition;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public override void Attack(Vector2 direction)
    {
        if (isStabbing || data == null) return;

        // 朝向敌人方向
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        StartCoroutine(Stab());
    }

    IEnumerator Stab()
    {
        isStabbing = true;

        float half = data.stabDuration * 0.5f;
        Vector3 orig = originalLocalPosition;
        Vector3 offset = transform.right * data.attackRange;

        // 去程
        hitEnemies.Clear();
        weaponCollider.enabled = true;

        float timer = 0f;
        while (timer < half)
        {
            float t = timer / half;
            transform.localPosition = Vector3.Lerp(orig, orig + offset, t);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = orig + offset;
        weaponCollider.enabled = false;

        // 回程
        hitEnemies.Clear(); // 清空，允许重新命中
        weaponCollider.enabled = true;

        timer = 0f;
        while (timer < half)
        {
            float t = timer / half;
            transform.localPosition = Vector3.Lerp(orig + offset, orig, t);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = orig;
        weaponCollider.enabled = false;

        isStabbing = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(enemyTag) || data == null) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;

        if (!hitEnemies.Contains(enemy))
        {
            // Debug.Log($"data.damge: {data.damage}, playerAttack: {player.stats.Attack}");
            float damge = data.damage + (float)player.stats.Attack;
            enemy.TakeDamage(damge);
            hitEnemies.Add(enemy);
        }
    }
}
