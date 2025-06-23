using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class MapBoundsCollider : MonoBehaviour
{
    [Header("要围挡的 Tilemap")]
    public Tilemap mapTilemap;

    [Header("碰撞体厚度")]
    public float wallThickness = 1f;

    [Header("碰撞体所属图层 (可选)")]
    public int physicsLayer = 0; // 默认 Layer 0

    void Start()
    {
        if (mapTilemap == null) mapTilemap = GetComponent<Tilemap>();

        // 1. 计算地图世界边界
        Bounds localBounds = mapTilemap.localBounds;
        Vector3 worldPos = mapTilemap.transform.position;
        // localBounds.center is relative; 转到世界坐标
        Vector2 center = new Vector2(localBounds.center.x + worldPos.x,
                                     localBounds.center.y + worldPos.y);
        Vector2 size = new Vector2(localBounds.size.x,
                                     localBounds.size.y);

        // 2. 创建一个空容器
        GameObject wallParent = new GameObject("MapBoundsWalls");
        wallParent.transform.SetParent(transform, false);

        // 3. 上/下/左/右 四个 BoxCollider2D
        CreateWall("Wall_Top", center + new Vector2(0, size.y / 2f + wallThickness / 2f), new Vector2(size.x + wallThickness * 2, wallThickness), wallParent);
        CreateWall("Wall_Bottom", center + new Vector2(0, -size.y / 2f - wallThickness / 2f), new Vector2(size.x + wallThickness * 2, wallThickness), wallParent);
        CreateWall("Wall_Left", center + new Vector2(-size.x / 2f - wallThickness / 2f, 0), new Vector2(wallThickness, size.y), wallParent);
        CreateWall("Wall_Right", center + new Vector2(size.x / 2f + wallThickness / 2f, 0), new Vector2(wallThickness, size.y), wallParent);
    }

    // 辅助方法：生成单个墙体
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
