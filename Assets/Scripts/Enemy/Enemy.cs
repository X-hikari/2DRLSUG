using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor.Build;

public enum EnemyState
{
    Idle,
    Walk,
    Attack,
    Die
}

public class Enemy : MonoBehaviour
{
    public string enemyId;
    // 公开子弹预制体引用，在编辑器拖入
    public GameObject bulletPrefab;

    private EnemyData enemyData;
    private Dictionary<string, Sprite[]> animationClips;
    private SpriteRenderer spriteRenderer;

    private int currentFrame = 0;
    private float frameTimer = 0f;
    private readonly float frameRate = 0.1f;
    private string currentAnimation = "idle";

    private EnemyState currentState = EnemyState.Idle;

    public float moveSpeed = 2f;
    public float detectRange = 5f;
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    private Transform player;
    private Player playerScript;
    private int currentHp;
    private bool isDead = false;

    void Start()
    {
        // 1. 加载 JSON 数据
        TextAsset json = Resources.Load<TextAsset>("Data/EnemyData");
        if (json == null)
        {
            Debug.LogError("EnemyData JSON not found in Resources/Data/");
            return;
        }
        List<EnemyData> allEnemies = JsonConvert.DeserializeObject<List<EnemyData>>(json.text);
        enemyData = allEnemies.FirstOrDefault(e => e.id == enemyId);
        if (enemyData == null)
        {
            return;
        }

        currentHp = enemyData.hp;
        attackRange = enemyData.attackRange;
        if (enemyData.attackType == "melee")
        {
            detectRange = enemyData.attackRange * 4.0f;
        }
        else if (enemyData.attackType == "ranged")
        {
            detectRange = enemyData.attackRange * 2.0f;
        }
        moveSpeed = enemyData.moveSpeed;
        attackCooldown = enemyData.attackCooldown;

        // 设置 Enemy 图层
        gameObject.layer = LayerMask.NameToLayer("Enemy");

        // 添加碰撞器（如果没有）
        if (GetComponent<Collider2D>() == null)
        {
            var collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }

        // 2. 加载精灵图和动画帧
        Sprite[] allSprites = Resources.LoadAll<Sprite>(enemyData.spriteSheet);
        animationClips = new Dictionary<string, Sprite[]>();

        foreach (var anim in enemyData.animations)
        {
            List<Sprite> clip = new();
            foreach (string frame in anim.Value)
            {
                if (frame.Contains("~"))
                {
                    string[] parts = frame.Split('~');
                    string baseName = parts[0].Split('_')[0];
                    int start = int.Parse(parts[0].Split('_')[1]);
                    int end = int.Parse(parts[1].Split('_')[1]);

                    for (int i = start; i <= end; i++)
                    {
                        string name = $"{baseName}_{i}";
                        var sprite = allSprites.FirstOrDefault(s => s.name == name);
                        if (sprite != null) clip.Add(sprite);
                    }
                }
                else
                {
                    var sprite = allSprites.FirstOrDefault(s => s.name == frame);
                    if (sprite != null) clip.Add(sprite);
                }
            }
            animationClips[anim.Key] = clip.ToArray();
        }

        // 3. 添加 SpriteRenderer（防止重复添加）
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 1;

        // 4. 获取玩家对象
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = player.GetComponent<Player>()
                        ?? player.GetComponentInParent<Player>()
                        ?? player.GetComponentInChildren<Player>();

        PlayAnimation("idle");
    }

    // 记录上一次攻击时间
    private float lastAttackTime = -999f;

    void Update()
    {
        if (enemyData == null) return;

        // 状态机控制
        float distance = Vector2.Distance(transform.position, player.position);

        if (currentHp <= 0)
        {
            ChangeState(EnemyState.Die);
        }
        else if (distance <= attackRange)
        {
            ChangeState(EnemyState.Attack);
        }
        else if (distance <= detectRange)
        {
            ChangeState(EnemyState.Walk);
        }
        else
        {
            ChangeState(EnemyState.Idle);
        }

        // 状态执行逻辑
        switch (currentState)
        {
            case EnemyState.Walk:
                Vector2 dir = (player.position - transform.position).normalized;

                // 朝向翻转
                spriteRenderer.flipX = dir.x < 0;

                transform.Translate(dir * moveSpeed * Time.deltaTime);
                break;

            case EnemyState.Attack:
                if (enemyData.attackType == "ranged")
                {
                    // 判断是否达到冷却时间
                    if (Time.time - lastAttackTime >= attackCooldown)
                    {
                        FireBulletAtPlayer();
                        lastAttackTime = Time.time;
                    }
                } else if (enemyData.attackType == "melee")
                {
                    if (Time.time - lastAttackTime >= attackCooldown)
                    {
                        // 近战攻击直接检测玩家距离和造成伤害
                        if (Vector2.Distance(transform.position, player.position) <= attackRange)
                        {
                            // 播放攻击动画（已有PlayAnimation("attack")）
                            // PlayAnimation("attack");

                            // var playerScript = player.GetComponent<Player>()
                            //     ?? player.GetComponentInParent<Player>()
                            //     ?? player.GetComponentInChildren<Player>();

                            playerScript?.TakeDamage(enemyData.attack);

                            lastAttackTime = Time.time;
                        }
                    }
                }
                break;

            case EnemyState.Die:
                // 播放死亡动画 + 延迟销毁
                Destroy(gameObject, 1f);
                // var playerScript = player.GetComponent<Player>();
                if (isDead == false)
                    playerScript.GainExp(enemyData.exp);
                isDead = true;
                break;
        }

        // 动画播放
        if (animationClips.ContainsKey(currentAnimation))
        {
            frameTimer += Time.deltaTime;
            if (frameTimer >= frameRate)
            {
                frameTimer = 0f;
                currentFrame = (currentFrame + 1) % animationClips[currentAnimation].Length;

                spriteRenderer.sprite = animationClips[currentAnimation][currentFrame];
            }
        }
    }

    public void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (newState)
        {
            case EnemyState.Idle:
                PlayAnimation("idle");
                break;
            case EnemyState.Walk:
                PlayAnimation("walk");
                break;
            case EnemyState.Attack:
                PlayAnimation("attack");
                break;
            case EnemyState.Die:
                PlayAnimation("die");
                break;
        }
    }

    public void PlayAnimation(string animName)
    {
        if (animationClips.ContainsKey(animName))
        {
            currentAnimation = animName;
            currentFrame = 0;
            frameTimer = 0f;
        }
    }

    private void FireBulletAtPlayer()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("bulletPrefab 没有设置！");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            // 计算方向
            Vector2 dir = (player.position - transform.position).normalized;

            // 加载子弹帧图
            List<Sprite> bulletSprites = new();
            if (enemyData.bulletSprites != null && enemyData.bulletSprites.Count > 0)
            {
                Sprite[] allBulletSprites = Resources.LoadAll<Sprite>(enemyData.spriteSheet); // 同一图集
                foreach (var frame in enemyData.bulletSprites)
                {
                    if (frame.Contains("~"))
                    {
                        string[] parts = frame.Split('~');
                        string baseName = parts[0].Split('_')[0];
                        int start = int.Parse(parts[0].Split('_')[1]);
                        int end = int.Parse(parts[1].Split('_')[1]);

                        for (int i = start; i <= end; i++)
                        {
                            string name = $"{baseName}_{i}";
                            var sprite = allBulletSprites.FirstOrDefault(s => s.name == name);
                            if (sprite != null)
                                bulletSprites.Add(sprite);
                        }
                    }
                    else
                    {
                        var sprite = allBulletSprites.FirstOrDefault(s => s.name == frame);
                        if (sprite != null)
                            bulletSprites.Add(sprite);
                    }
                }
            }

            bulletScript.Init(
                bulletSprites,
                enemyData.attack,
                dir,
                BulletFaction.Enemy
            );
        }
    }

    // 示例受伤函数
    public void TakeDamage(float dmg)
    {
        // Debug.Log($"Damage: {dmg}, current hp: {currentHp}");
        currentHp -= (int)dmg;
        if (currentHp < 0) currentHp = 0;
    }
}
