using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class MapBoundsCollider : MonoBehaviour
{
    [Header("ҪΧ���� Tilemap")]
    public Tilemap mapTilemap;

    [Header("��ײ����")]
    public float wallThickness = 1f;

    [Header("��ײ������ͼ�� (��ѡ)")]
    public int physicsLayer = 0; // Ĭ�� Layer 0

    void Start()
    {
        if (mapTilemap == null) mapTilemap = GetComponent<Tilemap>();

        // 1. �����ͼ����߽�
        Bounds localBounds = mapTilemap.localBounds;
        Vector3 worldPos = mapTilemap.transform.position;
        // localBounds.center is relative; ת����������
        Vector2 center = new Vector2(localBounds.center.x + worldPos.x,
                                     localBounds.center.y + worldPos.y);
        Vector2 size = new Vector2(localBounds.size.x,
                                     localBounds.size.y);

        // 2. ����һ��������
        GameObject wallParent = new GameObject("MapBoundsWalls");
        wallParent.transform.SetParent(transform, false);

        // 3. ��/��/��/�� �ĸ� BoxCollider2D
        CreateWall("Wall_Top", center + new Vector2(0, size.y / 2f + wallThickness / 2f), new Vector2(size.x + wallThickness * 2, wallThickness), wallParent);
        CreateWall("Wall_Bottom", center + new Vector2(0, -size.y / 2f - wallThickness / 2f), new Vector2(size.x + wallThickness * 2, wallThickness), wallParent);
        CreateWall("Wall_Left", center + new Vector2(-size.x / 2f - wallThickness / 2f, 0), new Vector2(wallThickness, size.y), wallParent);
        CreateWall("Wall_Right", center + new Vector2(size.x / 2f + wallThickness / 2f, 0), new Vector2(wallThickness, size.y), wallParent);
    }

    // �������������ɵ���ǽ��
    private void CreateWall(string name, Vector2 pos, Vector2 size, GameObject parent)
    {
        GameObject go = new GameObject(name);
        go.transform.parent = parent.transform;
        go.transform.position = pos;

        var bc = go.AddComponent<BoxCollider2D>();
        bc.size = size;
        bc.offset = Vector2.zero;
        bc.usedByComposite = false;

        go.layer = physicsLayer;
    }
}
