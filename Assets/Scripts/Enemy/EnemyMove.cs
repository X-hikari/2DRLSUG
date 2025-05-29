using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMove : MonoBehaviour
{
    [Header("׷������")]
    [Tooltip("��ҽ���˰뾶��ʼ׷��")]
    public float chaseRadius = 8f;
    [Tooltip("׷��ʱ���ƶ��ٶ�")]
    public float moveSpeed = 2f;
    [Tooltip("�����С�ڴ˾���ʱֹͣ�ƶ�")]
    public float stopDistance = 1f;

    private Transform player;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        else Debug.LogError("EnemyFollowPlayer���������Ҳ��� Tag Ϊ Player �Ķ���");
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // ��������ҵľ���
        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= chaseRadius && dist > stopDistance)
        {
            // �����������ӵ���ָ����ң�
            Vector2 dir = (player.position - transform.position).normalized;
            // �ƶ�
            rb.velocity = dir * moveSpeed;
        }
        else
        {
            // ����׷�ٰ뾶�����ʱֹͣ
            rb.velocity = Vector2.zero;
        }
    }

    // �� Scene ��ͼ�п��ӻ�׷�ٺ�ֹͣ�뾶
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
