using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("Ҫ�����Ŀ�꣨�� Player��")]
    public Transform target;
    [Tooltip("�������Ŀ���ƫ�ƣ�ͨ��Ϊ (0,0,-10)��")]
    public Vector3 offset = new Vector3(0, 0, -10);
    [Tooltip("����ƽ��ϵ����0-1��ԽСԽƽ����")]
    [Range(0f, 1f)]
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
    }
}
