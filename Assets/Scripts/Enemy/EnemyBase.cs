using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyBase : MonoBehaviour
{
    [Header("敌人属性")]
    [Tooltip("最大生命值")]
    public int maxHP = 3;
    private int currentHP;

    [Tooltip("是否死亡后播放特效")]
    public bool playDeathEffect = false;
    [Tooltip("死亡特效预制体（可选）")]
    public GameObject deathEffectPrefab;

    void Awake()
    {
        // 初始化当前血量
        currentHP = maxHP;
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
            Die();
    }

    /// <summary>
    /// 死亡处理
    /// </summary>
    private void Die()
    {
        if (playDeathEffect && deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
        // TODO: 可以在这里发送事件通知 GameManager 或加分
        Destroy(gameObject);
    }
}
