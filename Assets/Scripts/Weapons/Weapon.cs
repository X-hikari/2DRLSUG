using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Weapon : MonoBehaviour
{
    [Header("敌人检测")]
    [Tooltip("每秒最多寻找一次最近敌人，设为0表示每帧寻找")]
    public float searchInterval = 0.1f;
    [Tooltip("敌人 Tag")]
    public string enemyTag = "Enemy";

    [Header("旋转设置")]
    [Tooltip("完成一次 360° 旋转所需时间（秒）")]
    public float rotationDuration = 0.2f;

    [Header("攻击设置")]
    [Tooltip("每次攻击之间的时间间隔")]
    public float attackInterval = 0.6f;
    [Tooltip("刺击距离")]
    public float attackRange = 1.2f;
    [Tooltip("刺击动作持续时间")]
    public float stabDuration = 0.2f;

    public float damage = 1;

    private float rotationSpeed;
    private float searchTimer;
    private float attackTimer;
    private bool isStabbing = false;

    private Collider2D weaponCollider;
    private Vector3 originalLocalPosition;

    void Awake()
    {
        // 角速度计算
        rotationSpeed = rotationDuration > 0f ? 360f / rotationDuration : 360f;
        searchTimer = 0f;
        attackTimer = 0f;

        weaponCollider = GetComponent<Collider2D>();
        if (weaponCollider == null)
            Debug.LogError("请为武器添加一个 Collider2D！");

        weaponCollider.enabled = false;
        originalLocalPosition = transform.localPosition;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // 自动瞄准
        searchTimer -= dt;
        if (searchTimer <= 0f)
        {
            AimAtNearestEnemy();
            searchTimer = searchInterval;
        }

        // 控制攻击节奏
        attackTimer -= dt;
        if (attackTimer <= 0f)
        {
            StartCoroutine(Stab());
            attackTimer = attackInterval;
        }
    }

    void AimAtNearestEnemy()
    {
        var enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        if (enemies.Length == 0) return;

        Transform closest = null;
        float minDist = float.MaxValue;
        Vector3 myPos = transform.position;

        foreach (var go in enemies)
        {
            float d = (go.transform.position - myPos).sqrMagnitude;
            if (d < minDist)
            {
                minDist = d;
                closest = go.transform;
            }
        }

        if (closest == null) return;

        Vector3 dir = (closest.position - myPos).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Quaternion targetRot = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );
    }

    IEnumerator Stab()
    {
        if (isStabbing) yield break;
        isStabbing = true;

        float half = stabDuration * 0.5f;
        Vector3 orig = originalLocalPosition;
        Vector3 offset = transform.right * attackRange;

        float timer = 0f;
        // —— 平滑刺出 ——
        while (timer < half)
        {
            float t = timer / half;                                 // 0→1
            transform.localPosition = Vector3.Lerp(orig, orig + offset, t);
            timer += Time.deltaTime;
            yield return null;
        }
        // 确保到位
        transform.localPosition = orig + offset;
        // 开启碰撞检测
        weaponCollider.enabled = true;

        // —— 平滑收回 ——
        timer = 0f;
        while (timer < half)
        {
            float t = timer / half;                                 // 0→1
            transform.localPosition = Vector3.Lerp(orig + offset, orig, t);
            timer += Time.deltaTime;
            yield return null;
        }
        // 确保复位
        transform.localPosition = orig;
        weaponCollider.enabled = false;

        isStabbing = false;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(enemyTag)) return;

        // 尝试获取 EnemyBase 并调用受伤
        Enemy enemy = other.GetComponent<Enemy>();
        // if (enemy != null)
        // {
        //     enemy.TakeDamage(damage);
        // }
    }
}
