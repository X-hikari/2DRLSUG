using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Weapon : MonoBehaviour
{
    [Header("���˼��")]
    [Tooltip("ÿ�����Ѱ��һ��������ˣ���Ϊ0��ʾÿ֡Ѱ��")]
    public float searchInterval = 0.1f;
    [Tooltip("���� Tag")]
    public string enemyTag = "Enemy";

    [Header("��ת����")]
    [Tooltip("���һ�� 360�� ��ת����ʱ�䣨�룩")]
    public float rotationDuration = 0.2f;

    [Header("��������")]
    [Tooltip("ÿ�ι���֮���ʱ����")]
    public float attackInterval = 0.6f;
    [Tooltip("�̻�����")]
    public float attackRange = 1.2f;
    [Tooltip("�̻���������ʱ��")]
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
        // ���ٶȼ���
        rotationSpeed = rotationDuration > 0f ? 360f / rotationDuration : 360f;
        searchTimer = 0f;
        attackTimer = 0f;

        weaponCollider = GetComponent<Collider2D>();
        if (weaponCollider == null)
            Debug.LogError("��Ϊ�������һ�� Collider2D��");

        weaponCollider.enabled = false;
        originalLocalPosition = transform.localPosition;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // �Զ���׼
        searchTimer -= dt;
        if (searchTimer <= 0f)
        {
            AimAtNearestEnemy();
            searchTimer = searchInterval;
        }

        // ���ƹ�������
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
        // ���� ƽ���̳� ����
        while (timer < half)
        {
            float t = timer / half;                                 // 0��1
            transform.localPosition = Vector3.Lerp(orig, orig + offset, t);
            timer += Time.deltaTime;
            yield return null;
        }
        // ȷ����λ
        transform.localPosition = orig + offset;
        // ������ײ���
        weaponCollider.enabled = true;

        // ���� ƽ���ջ� ����
        timer = 0f;
        while (timer < half)
        {
            float t = timer / half;                                 // 0��1
            transform.localPosition = Vector3.Lerp(orig + offset, orig, t);
            timer += Time.deltaTime;
            yield return null;
        }
        // ȷ����λ
        transform.localPosition = orig;
        weaponCollider.enabled = false;

        isStabbing = false;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(enemyTag)) return;

        // ���Ի�ȡ EnemyBase ����������
        Enemy enemy = other.GetComponent<Enemy>();
        // if (enemy != null)
        // {
        //     enemy.TakeDamage(damage);
        // }
    }
}
