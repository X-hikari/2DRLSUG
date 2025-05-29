using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("Ҫ�����Ŀ�꣨�� Player��")]
    public Transform target;

    [Tooltip("�������Ŀ���ƫ�ƣ�ͨ��Ϊ (0,0,-10)��")]
    public Vector3 offset = new Vector3(0, 0, -10);

    [Tooltip("����ƽ��ϵ����0-1��ԽСԽƽ����")]
    [Range(0f, 1f)]
    public float smoothSpeed = 0.125f;

    [Header("��ͼ�߽磨�������ǯ�ƣ�")]
    [Tooltip("���ڼ����ͼ�߽�� Tilemap")]
    public Tilemap mapTilemap;

    // ��ͼ����߽�
    private Bounds worldBounds;
    // �������� & ���
    private float halfHeight;
    private float halfWidth;

    void Start()
    {
        if (target == null)
            Debug.LogError("CameraFollow: û�����ø���Ŀ�꣨target����");

        if (mapTilemap == null)
            Debug.LogError("CameraFollow: û�й��� Tilemap�����ڼ���߽磡");

        // 1. �����ͼ����߽�
        Bounds local = mapTilemap.localBounds;
        Vector3 pos = mapTilemap.transform.position;
        worldBounds = new Bounds(local.center + pos, local.size);

        // 2. �����������߰�����������������
        Camera cam = GetComponent<Camera>();
        if (!cam.orthographic)
            Debug.LogWarning("CameraFollow: ����ʹ�� Orthographic ���������ǯ�ƿ��ܲ�׼ȷ��");

        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;
    }

    void LateUpdate()
    {
        if (target == null || mapTilemap == null) return;

        // 1. Ŀ��λ�� + ƫ��
        Vector3 desired = target.position + offset;

        // 2. ƽ����ֵ
        Vector3 smoothed = Vector3.Lerp(transform.position, desired, smoothSpeed);

        // 3. ǯ�Ƶ���ͼ��Χ��
        float minX = worldBounds.min.x + halfWidth;
        float maxX = worldBounds.max.x - halfWidth;
        float minY = worldBounds.min.y + halfHeight;
        float maxY = worldBounds.max.y - halfHeight;

        smoothed.x = Mathf.Clamp(smoothed.x, minX, maxX);
        smoothed.y = Mathf.Clamp(smoothed.y, minY, maxY);

        // 4. Ӧ��λ��
        transform.position = smoothed;
    }
}
