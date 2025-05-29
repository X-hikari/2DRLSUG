using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyBase : MonoBehaviour
{
    [Header("��������")]
    [Tooltip("�������ֵ")]
    public float maxHP = 3;
    private float currentHP;

    [Tooltip("�Ƿ������󲥷���Ч")]
    public bool playDeathEffect = false;
    [Tooltip("������ЧԤ���壨��ѡ��")]
    public GameObject deathEffectPrefab;

    void Awake()
    {
        currentHP = maxHP;
    }

    /// <summary>
    /// �ܵ��˺�
    /// </summary>
    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"{gameObject.name} �ܵ� {damage} ���˺���ʣ������ {currentHP}");
        if (currentHP <= 0)
            Die();
    }

    private void Die()
    {
        if (playDeathEffect && deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
