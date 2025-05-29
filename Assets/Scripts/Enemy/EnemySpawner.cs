using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class EnemySpawner : MonoBehaviour
{
    [Header("地图与刷怪点")]
    [Tooltip("用于刷怪的 Tilemap")]
    public Tilemap spawnTilemap;

    [Header("敌人设置")]
    [Tooltip("要随机生成的敌人预制体列表")]
    public List<GameObject> enemyPrefabs;

    [Tooltip("两次生成间隔（秒）")]
    public float spawnInterval = 2f;

    [Tooltip("生成时与玩家的最小距离")]
    public float minDistanceFromPlayer = 0f;

    // 缓存
    private float timer;
    private Transform playerTransform;
    private Bounds worldBounds;

    void Awake()
    {
        // 确保关联 Tilemap
        if (spawnTilemap == null)
            spawnTilemap = GetComponent<Tilemap>();

        // 找到玩家
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
        else
            Debug.LogWarning("EnemySpawner: 场景中未找到标签为 Player 的对象，距离过滤将失效。");

        // 计算 Tilemap 在世界坐标下的包围盒
        // localBounds 是相对于 Tilemap 本地空间的 Bounds，需加上 Transform.position
        worldBounds = spawnTilemap.localBounds;
        worldBounds.center += (Vector3)spawnTilemap.transform.position;
    }

    void Update()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
            return; // 没有预制体，跳过

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        // 随机选择一个敌人类型
        var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        if (prefab == null)
            return;

        const int maxAttempts = 20;
        for (int i = 0; i < maxAttempts; i++)
        {
            // 在世界包围盒内做连续随机采样
            float x = Random.Range(worldBounds.min.x, worldBounds.max.x);
            float y = Random.Range(worldBounds.min.y, worldBounds.max.y);
            Vector3 worldPos = new Vector3(x, y, 0f);

            // 转成 Tilemap 的格子坐标并判断是否有 Tile
            Vector3Int cell = spawnTilemap.WorldToCell(worldPos);
            if (!spawnTilemap.HasTile(cell))
                continue;

            // 确保与玩家保持最小距离
            if (playerTransform != null &&
                Vector2.Distance(worldPos, playerTransform.position) < minDistanceFromPlayer)
                continue;

            // 通过所有检查，生成敌人
            Instantiate(prefab, worldPos, Quaternion.identity);
            return;
        }

        // 如果多次尝试未找到合法点，则在玩家最远可行的格子中心生成（或者直接跳过/做回退逻辑）
        // 这里示例简单跳过本次生成：
        Debug.LogWarning("EnemySpawner: 多次尝试后未找到合适刷怪点，本次跳过生成。");
    }

    private void OnDrawGizmosSelected()
    {
        // 可视化 Tilemap 包围盒
        if (spawnTilemap != null)
        {
            Gizmos.color = Color.cyan;
            Bounds b = spawnTilemap.localBounds;
            Vector3 center = b.center + (Vector3)spawnTilemap.transform.position;
            Gizmos.DrawWireCube(center, b.size);
        }
        // 可视化玩家安全范围
        if (Application.isPlaying && playerTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerTransform.position, minDistanceFromPlayer);
        }
    }
}
