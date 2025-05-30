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
    public string enemyTag = "Enemy";
    private EnemyData enemyData;
    private Dictionary<string, Sprite[]> animationClips;
    private SpriteRenderer spriteRenderer;

    private int currentFrame = 0;
    private float frameTimer = 0f;
    private float frameRate = 0.1f;
    private string currentAnimation = "idle";

    private EnemyState currentState = EnemyState.Idle;

    public float moveSpeed = 2f;
    public float detectRange = 5f;
    public float attackRange = 1f;
    private Transform player;
    private int currentHp;

    void Start()
    {
        // 1. 加载 JSON 数据
        TextAsset json = Resources.Load<TextAsset>("Data/EnemyData");
        enemyData = JsonConvert.DeserializeObject<EnemyData>(json.text);
        currentHp = enemyData.hp;

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

        // 4. 获取玩家对象
        player = GameObject.FindGameObjectWithTag("Player").transform;

        PlayAnimation("idle");
    }

    void Update()
    {
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
                UseSkill(); // 或播放攻击动画等
                break;

            case EnemyState.Die:
                // 播放死亡动画 + 延迟销毁
                Destroy(gameObject, 0.5f);
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

    public void UseSkill()
    {
        foreach (var skill in enemyData.skills)
        {
            // Debug.Log($"Using skill: {skill.name}");
            SkillParser.Execute(skill.commands);
        }
    }

    // 示例受伤函数
    public void TakeDamage(int dmg)
    {
        currentHp -= dmg;
        if (currentHp < 0) currentHp = 0;
    }
}
