using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyBase : MonoBehaviour
{
    [Header("敌人属性")]
    [Tooltip("最大生命值")]
    public float maxHP = 3;
    private float currentHP;

    [Tooltip("是否死亡后播放特效")]
    public bool playDeathEffect = false;
    [Tooltip("死亡特效预制体（可选）")]
    public GameObject deathEffectPrefab;

    void Awake()
    {
        currentHP = maxHP;
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"{gameObject.name} 受到 {damage} 点伤害，剩余生命 {currentHP}");
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
