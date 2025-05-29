using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("要跟随的目标（拖 Player）")]
    public Transform target;

    [Tooltip("摄像机与目标的偏移（通常为 (0,0,-10)）")]
    public Vector3 offset = new Vector3(0, 0, -10);

    [Tooltip("跟随平滑系数（0-1，越小越平滑）")]
    [Range(0f, 1f)]
    public float smoothSpeed = 0.125f;

    [Header("地图边界（用于相机钳制）")]
    [Tooltip("用于计算地图边界的 Tilemap")]
    public Tilemap mapTilemap;

    // 地图世界边界
    private Bounds worldBounds;
    // 摄像机半高 & 半宽
    private float halfHeight;
    private float halfWidth;

    void Start()
    {
        if (target == null)
            Debug.LogError("CameraFollow: 没有设置跟随目标（target）！");

        if (mapTilemap == null)
            Debug.LogError("CameraFollow: 没有关联 Tilemap，用于计算边界！");

        // 1. 计算地图世界边界
        Bounds local = mapTilemap.localBounds;
        Vector3 pos = mapTilemap.transform.position;
        worldBounds = new Bounds(local.center + pos, local.size);

        // 2. 计算摄像机半高半宽（基于正交摄像机）
        Camera cam = GetComponent<Camera>();
        if (!cam.orthographic)
            Debug.LogWarning("CameraFollow: 建议使用 Orthographic 相机，否则钳制可能不准确。");

        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;
    }

    void LateUpdate()
    {
        if (target == null || mapTilemap == null) return;

        // 1. 目标位置 + 偏移
        Vector3 desired = target.position + offset;

        // 2. 平滑插值
        Vector3 smoothed = Vector3.Lerp(transform.position, desired, smoothSpeed);

        // 3. 钳制到地图范围内
        float minX = worldBounds.min.x + halfWidth;
        float maxX = worldBounds.max.x - halfWidth;
        float minY = worldBounds.min.y + halfHeight;
        float maxY = worldBounds.max.y - halfHeight;

        smoothed.x = Mathf.Clamp(smoothed.x, minX, maxX);
        smoothed.y = Mathf.Clamp(smoothed.y, minY, maxY);

        // 4. 应用位置
        transform.position = smoothed;
    }
}
