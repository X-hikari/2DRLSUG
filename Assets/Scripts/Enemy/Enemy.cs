using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

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
    private bool expGiven = false;

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
            Debug.LogError($"EnemyData with id {enemyId} not found!");
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

        gameObject.layer = LayerMask.NameToLayer("Enemy");

        if (GetComponent<Collider2D>() == null)
        {
            var collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }

         if (GetComponent<Rigidbody2D>() == null)
        {
            Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.gravityScale = 0f;
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

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 1;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            playerScript = player.GetComponent<Player>()
                            ?? player.GetComponentInParent<Player>()
                            ?? player.GetComponentInChildren<Player>();
        }

        PlayAnimation("idle");
    }

    private float lastAttackTime = -999f;

    void Update()
    {
        if (enemyData == null) return;

        if (isDead)
        {
            AnimateCurrentAnimation();

            // 播放死亡动画到最后一帧后1秒销毁物体
            if (currentFrame == animationClips[currentAnimation].Length - 1)
            {
                Destroy(gameObject, 1f);
            }
            return; // 死亡时不执行其他逻辑
        }

        if (player != null && !playerScript.stateManager.IsInvisible)
            spriteRenderer.flipX = player.position.x < transform.position.x;

        float distance = Vector2.Distance(transform.position, player.position);

        if (currentHp <= 0)
        {
            Die();
            return;
        }

        if (playerScript != null && playerScript.stateManager.IsInvisible)
        {
            ChangeState(EnemyState.Idle);
            AnimateCurrentAnimation();
            return;
        }

        if (distance <= attackRange)
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

        switch (currentState)
        {
            case EnemyState.Walk:
                Vector2 dir = (player.position - transform.position).normalized;
                transform.Translate(dir * moveSpeed * Time.deltaTime);
                break;

            case EnemyState.Attack:
                if (enemyData.attackType == "ranged")
                {
                    if (Time.time - lastAttackTime >= attackCooldown)
                    {
                        FireBulletAtPlayer();
                        lastAttackTime = Time.time;
                    }
                }
                else if (enemyData.attackType == "melee")
                {
                    if (Time.time - lastAttackTime >= attackCooldown && distance <= attackRange)
                    {
                        playerScript?.TakeDamage(enemyData.attack);
                        lastAttackTime = Time.time;
                    }
                }
                break;
        }

        AnimateCurrentAnimation();
    }

    private void AnimateCurrentAnimation()
    {
        if (animationClips.ContainsKey(currentAnimation))
        {
            frameTimer += Time.deltaTime;
            if (frameTimer >= frameRate)
            {
                frameTimer = 0f;
                currentFrame++;
                if (currentFrame >= animationClips[currentAnimation].Length)
                {
                    if (currentState == EnemyState.Die)
                    {
                        currentFrame = animationClips[currentAnimation].Length - 1; // 死亡动画停留在最后一帧
                    }
                    else
                    {
                        currentFrame = 0; // 其他动画循环
                    }
                }
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
            Vector2 dir = (player.position - transform.position).normalized;

            List<Sprite> bulletSprites = new();
            if (enemyData.bulletSprites != null && enemyData.bulletSprites.Count > 0)
            {
                Sprite[] allBulletSprites = Resources.LoadAll<Sprite>(enemyData.spriteSheet);
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

    public void TakeDamage(float dmg)
    {
        currentHp -= (int)dmg;
        EventManager.TriggerEvent(EventType.OnEnemyKilled, this, new AttackHitEventArgs(this, (int)dmg));
        if (currentHp < 0) currentHp = 0;
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        ChangeState(EnemyState.Die);

        if (!expGiven)
        {
            playerScript?.GainExp(enemyData.exp);
            expGiven = true;
        }

        currentFrame = 0;
        frameTimer = 0f;
        EventManager.TriggerEvent(EventType.OnEnemyKilled, this, new EnemyKilledEventArgs(this));
    }
}
