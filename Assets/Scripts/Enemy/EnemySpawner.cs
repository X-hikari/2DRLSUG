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

    [Tooltip("通用敌人预制体")]
    public GameObject enemyPrefab;

    [Tooltip("可能生成的敌人 ID 列表")]
    public List<string> enemyIdsToSpawn;

    private List<GameObject> spawnedEnemies = new();

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
        // 清理已经被销毁的敌人引用
        spawnedEnemies.RemoveAll(e => e == null);

        // 限制最多10个敌人
        if (spawnedEnemies.Count >= 10)
        {
            Debug.Log("敌人数量已达到上限，暂不生成");
            return;
        }

        if (enemyPrefab == null || enemyIdsToSpawn == null || enemyIdsToSpawn.Count == 0)
            return;

        const int maxAttempts = 20;
        for (int i = 0; i < maxAttempts; i++)
        {
            float x = Random.Range(worldBounds.min.x, worldBounds.max.x);
            float y = Random.Range(worldBounds.min.y, worldBounds.max.y);
            Vector3 worldPos = new(x, y, 0f);
            Vector3Int cell = spawnTilemap.WorldToCell(worldPos);

            if (!spawnTilemap.HasTile(cell))
                continue;

            if (playerTransform != null &&
                Vector2.Distance(worldPos, playerTransform.position) < minDistanceFromPlayer)
                continue;

            // 随机选择 enemyId
            string enemyId = enemyIdsToSpawn[Random.Range(0, enemyIdsToSpawn.Count)];

            // 实例化通用敌人
            GameObject enemy = Instantiate(enemyPrefab, worldPos, Quaternion.identity);

            // 设置 enemyId
            var enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
                enemyScript.enemyId = enemyId;
            else
                Debug.LogWarning("EnemyPrefab 上没有挂载 Enemy 脚本。");
            
            // 记录生成的敌人
            spawnedEnemies.Add(enemy);

            return;
        }

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
