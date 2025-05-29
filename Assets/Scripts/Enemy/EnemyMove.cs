using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMove : MonoBehaviour
{
    [Header("追踪设置")]
    [Tooltip("玩家进入此半径后开始追踪")]
    public float chaseRadius = 8f;
    [Tooltip("追踪时的移动速度")]
    public float moveSpeed = 2f;
    [Tooltip("离玩家小于此距离时停止移动")]
    public float stopDistance = 1f;

    private Transform player;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        else Debug.LogError("EnemyFollowPlayer：场景中找不到 Tag 为 Player 的对象！");
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // 计算与玩家的距离
        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= chaseRadius && dist > stopDistance)
        {
            // 方向向量（从敌人指向玩家）
            Vector2 dir = (player.position - transform.position).normalized;
            // 移动
            rb.velocity = dir * moveSpeed;
        }
        else
        {
            // 超出追踪半径或过近时停止
            rb.velocity = Vector2.zero;
        }
    }

    // 在 Scene 视图中可视化追踪和停止半径
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
