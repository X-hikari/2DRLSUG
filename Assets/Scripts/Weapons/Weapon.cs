using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Tooltip("每秒最多寻找一次最近敌人，设0则每帧寻找")]
    public float searchInterval = 0.1f;
    [Tooltip("打上此Tag的物体才视为敌人")]
    public string enemyTag = "Enemy";

    [Header("自动计算旋转速度")]
    [Tooltip("完成一次 360° 旋转所需时间（秒），脚本内部自动算角速度")]
    public float rotationDuration = 0.2f;

    private float rotationSpeed;   // 自动计算得到，单位：度/秒
    private float searchTimer;

    void Awake()
    {
        // 计算角速度
        if (rotationDuration > 0f)
            rotationSpeed = 360f / rotationDuration;
        else
            rotationSpeed = 360f;  // 如果配置不合理，退回默认

        searchTimer = 0f;
    }

    void Update()
    {
        searchTimer -= Time.deltaTime;
        if (searchTimer <= 0f)
        {
            AimAtNearestEnemy();
            searchTimer = searchInterval;
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

        // 目标角度
        Vector3 dir = (closest.position - myPos).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion currentRot = transform.rotation;
        Quaternion targetRot = Quaternion.Euler(0, 0, targetAngle);

        // 平滑旋转
        transform.rotation = Quaternion.RotateTowards(
            currentRot,
            targetRot,
            rotationSpeed * Time.deltaTime
        );
    }
}
