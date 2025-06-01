using UnityEngine;
using System.Collections.Generic;

public abstract class Weapon : MonoBehaviour
{
    [Header("基础属性")]
    public WeaponData data;

    [Header("旋转设置")]
    public float rotateDuration = 2f; // 360° 所需时间

    [Header("检测设置")]
    public float detectionInterval = 1f;
    public float detectionRange = 3f;
    public string enemyTag = "Enemy";

    [Header("攻击设置")]
    public float attackCooldown = 0.5f;

    protected List<Transform> detectedEnemies = new();
    protected float detectionTimer = 0f;
    protected float rotationSpeed; // 角速度 (°/s)

    protected float attackCooldownTimer = 0f;
    protected WeaponRenderer weaponRenderer;

    protected Transform currentTarget = null;

    protected virtual void Awake()
    {
        weaponRenderer = GetComponent<WeaponRenderer>();

        if (rotateDuration > 0)
            rotationSpeed = 360f / rotateDuration;
    }

    protected virtual void Start()
    {
        InitializeRenderer();
    }

    protected virtual void InitializeRenderer()
    {
        if (weaponRenderer == null)
            weaponRenderer = GetComponent<WeaponRenderer>();

        if (data != null && weaponRenderer != null)
        {
            if (data.idleSprite != null)
                weaponRenderer.SetSprite(data.idleSprite);

            if (data.animationController != null)
                weaponRenderer.SetAnimatorOverride(
                    data.animationController,
                    data.idleClip,
                    data.attackClip
                );
        }
    }

    protected virtual void Update()
    {
        attackCooldownTimer -= Time.deltaTime;

        detectionTimer += Time.deltaTime;
        if (detectionTimer >= detectionInterval)
        {
            detectionTimer = 0f;
            DetectEnemies();
        }

        if (detectedEnemies.Count > 0)
        {
            currentTarget = FindNearestEnemy();
        }
        else
        {
            currentTarget = null;
        }

        if (currentTarget != null)
        {
            Vector2 dir = (currentTarget.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            float angleDiff = Quaternion.Angle(transform.rotation, targetRotation);

            if (angleDiff < 1f && attackCooldownTimer <= 0f)
            {
                Attack(dir);
                attackCooldownTimer = attackCooldown;
            }
        }
        else
        {
            // 没有目标，原地旋转
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }

    protected virtual void DetectEnemies()
    {
        detectedEnemies.Clear();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        foreach (var col in colliders)
        {
            if (col.CompareTag(enemyTag))
            {
                detectedEnemies.Add(col.transform);
            }
        }
    }

    protected Transform FindNearestEnemy()
    {
        Transform nearest = null;
        float minDistSqr = Mathf.Infinity;
        Vector3 pos = transform.position;

        // 过滤掉已被销毁的对象
        detectedEnemies.RemoveAll(enemy => enemy == null);

        foreach (var enemy in detectedEnemies)
        {
            if (enemy == null) continue;

            float distSqr = (enemy.position - pos).sqrMagnitude;
            if (distSqr < minDistSqr)
            {
                minDistSqr = distSqr;
                nearest = enemy;
            }
        }
        return nearest;
    }

    public abstract void Attack(Vector2 direction);

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    public virtual void Initialize(WeaponData weaponData)
    {
        data = weaponData;
        InitializeRenderer();
    }
}
