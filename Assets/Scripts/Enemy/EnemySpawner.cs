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
        // ���ѡ��һ����������
        var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        if (prefab == null)
            return;

        const int maxAttempts = 20;
        for (int i = 0; i < maxAttempts; i++)
        {
            // �������Χ�����������������
            float x = Random.Range(worldBounds.min.x, worldBounds.max.x);
            float y = Random.Range(worldBounds.min.y, worldBounds.max.y);
            Vector3 worldPos = new Vector3(x, y, 0f);

            // ת�� Tilemap �ĸ������겢�ж��Ƿ��� Tile
            Vector3Int cell = spawnTilemap.WorldToCell(worldPos);
            if (!spawnTilemap.HasTile(cell))
                continue;

            // ȷ������ұ�����С����
            if (playerTransform != null &&
                Vector2.Distance(worldPos, playerTransform.position) < minDistanceFromPlayer)
                continue;

            // ͨ�����м�飬���ɵ���
            Instantiate(prefab, worldPos, Quaternion.identity);
            return;
        }

        // �����γ���δ�ҵ��Ϸ��㣬���������Զ���еĸ����������ɣ�����ֱ������/�������߼���
        // ����ʾ���������������ɣ�
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
