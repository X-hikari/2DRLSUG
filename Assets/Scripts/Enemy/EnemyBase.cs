using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyBase : MonoBehaviour
{
    [Header("��������")]
    [Tooltip("�������ֵ")]
    public int maxHP = 3;
    private int currentHP;

    [Tooltip("�Ƿ������󲥷���Ч")]
    public bool playDeathEffect = false;
    [Tooltip("������ЧԤ���壨��ѡ��")]
    public GameObject deathEffectPrefab;

    void Awake()
    {
        // ��ʼ����ǰѪ��
        currentHP = maxHP;
    }

    /// <summary>
    /// �ܵ��˺�
    /// </summary>
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
            Die();
    }

    /// <summary>
    /// ��������
    /// </summary>
    private void Die()
    {
        if (playDeathEffect && deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
        // TODO: ���������﷢���¼�֪ͨ GameManager ��ӷ�
        Destroy(gameObject);
    }
}
