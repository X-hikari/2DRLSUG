using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Tooltip("ÿ�����Ѱ��һ��������ˣ���0��ÿ֡Ѱ��")]
    public float searchInterval = 0.1f;
    [Tooltip("���ϴ�Tag���������Ϊ����")]
    public string enemyTag = "Enemy";

    [Header("�Զ�������ת�ٶ�")]
    [Tooltip("���һ�� 360�� ��ת����ʱ�䣨�룩���ű��ڲ��Զ�����ٶ�")]
    public float rotationDuration = 0.2f;

    private float rotationSpeed;   // �Զ�����õ�����λ����/��
    private float searchTimer;

    void Awake()
    {
        // ������ٶ�
        if (rotationDuration > 0f)
            rotationSpeed = 360f / rotationDuration;
        else
            rotationSpeed = 360f;  // ������ò������˻�Ĭ��

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

        // Ŀ��Ƕ�
        Vector3 dir = (closest.position - myPos).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion currentRot = transform.rotation;
        Quaternion targetRot = Quaternion.Euler(0, 0, targetAngle);

        // ƽ����ת
        transform.rotation = Quaternion.RotateTowards(
            currentRot,
            targetRot,
            rotationSpeed * Time.deltaTime
        );
    }
}
