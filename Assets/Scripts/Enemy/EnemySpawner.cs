using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class EnemySpawner : MonoBehaviour
{
    [Header("��ͼ��ˢ�ֵ�")]
    [Tooltip("����ˢ�ֵ� Tilemap")]
    public Tilemap spawnTilemap;

    [Header("��������")]
    [Tooltip("Ҫ������ɵĵ���Ԥ�����б�")]
    public List<GameObject> enemyPrefabs;

    [Tooltip("�������ɼ�����룩")]
    public float spawnInterval = 2f;

    [Tooltip("����ʱ����ҵ���С����")]
    public float minDistanceFromPlayer = 0f;

    [Tooltip("ͨ�õ���Ԥ����")]
    public GameObject enemyPrefab;

    [Tooltip("�������ɵĵ��� ID �б�")]
    public List<string> enemyIdsToSpawn;

    private List<GameObject> spawnedEnemies = new();

    // ����
    private float timer;
    private Transform playerTransform;
    private Bounds worldBounds;

    void Awake()
    {
        // ȷ������ Tilemap
        if (spawnTilemap == null)
            spawnTilemap = GetComponent<Tilemap>();

        // �ҵ����
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
        else
            Debug.LogWarning("EnemySpawner: ������δ�ҵ���ǩΪ Player �Ķ��󣬾�����˽�ʧЧ��");

        // ���� Tilemap �����������µİ�Χ��
        // localBounds ������� Tilemap ���ؿռ�� Bounds������� Transform.position
        worldBounds = spawnTilemap.localBounds;
        worldBounds.center += (Vector3)spawnTilemap.transform.position;
    }

    void Update()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
            return; // û��Ԥ���壬����

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        // �����Ѿ������ٵĵ�������
        spawnedEnemies.RemoveAll(e => e == null);

        // �������10������
        if (spawnedEnemies.Count >= 10)
        {
            Debug.Log("���������Ѵﵽ���ޣ��ݲ�����");
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

            // ���ѡ�� enemyId
            string enemyId = enemyIdsToSpawn[Random.Range(0, enemyIdsToSpawn.Count)];

            // ʵ����ͨ�õ���
            GameObject enemy = Instantiate(enemyPrefab, worldPos, Quaternion.identity);

            // ���� enemyId
            var enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
                enemyScript.enemyId = enemyId;
            else
                Debug.LogWarning("EnemyPrefab ��û�й��� Enemy �ű���");
            
            // ��¼���ɵĵ���
            spawnedEnemies.Add(enemy);

            return;
        }

        Debug.LogWarning("EnemySpawner: ��γ��Ժ�δ�ҵ�����ˢ�ֵ㣬�����������ɡ�");
    }

    private void OnDrawGizmosSelected()
    {
        // ���ӻ� Tilemap ��Χ��
        if (spawnTilemap != null)
        {
            Gizmos.color = Color.cyan;
            Bounds b = spawnTilemap.localBounds;
            Vector3 center = b.center + (Vector3)spawnTilemap.transform.position;
            Gizmos.DrawWireCube(center, b.size);
        }
        // ���ӻ���Ұ�ȫ��Χ
        if (Application.isPlaying && playerTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerTransform.position, minDistanceFromPlayer);
        }
    }
}
