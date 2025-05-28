using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("要跟随的目标（拖 Player）")]
    public Transform target;
    [Tooltip("摄像机与目标的偏移（通常为 (0,0,-10)）")]
    public Vector3 offset = new Vector3(0, 0, -10);
    [Tooltip("跟随平滑系数（0-1，越小越平滑）")]
    [Range(0f, 1f)]
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
    }
}
