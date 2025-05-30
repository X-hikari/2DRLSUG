using UnityEngine;
using System.Collections.Generic;

public enum BulletFaction
{
    Friendly,
    Enemy
}

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class Bullet : MonoBehaviour
{
    public float speed = 1f;
    public float damage = 1f;
    public Vector2 direction = Vector2.right;
    public BulletFaction faction;

    public List<Sprite> animationSprites;
    public float frameRate = 0.1f;
    public float maxTravelDistance = 20f;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private int currentFrame = 0;
    private float frameTimer = 0f;
    private Vector2 startPosition;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 2;
    }

    private void Start()
    {
        startPosition = transform.position;
        // 设置 Enemy 图层
        gameObject.layer = LayerMask.NameToLayer("Bullet");
    }

    private void Update()
    {
        Vector2 movement = direction.normalized * speed * Time.deltaTime;
        transform.position += (Vector3)movement;

        // 超出飞行距离销毁
        float traveled = Vector2.Distance(startPosition, transform.position);
        if (traveled >= maxTravelDistance)
        {
            Destroy(gameObject);
            return;
        }

        // 播放动画
        if (animationSprites != null && animationSprites.Count > 0)
        {
            frameTimer += Time.deltaTime;
            if (frameTimer >= frameRate)
            {
                frameTimer = 0f;
                currentFrame = (currentFrame + 1) % animationSprites.Count;
                spriteRenderer.sprite = animationSprites[currentFrame];
            }
        }
    }

    public void Init(List<Sprite> sprites, float dmg, Vector2 dir, BulletFaction bulletFaction, float maxDistance = 20f)
    {
        animationSprites = sprites;
        damage = dmg;
        direction = dir;
        faction = bulletFaction;
        maxTravelDistance = maxDistance;

        // 设置旋转朝向
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (sprites != null && sprites.Count > 0)
        {
            spriteRenderer.sprite = sprites[0];
        }

        startPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (faction == BulletFaction.Enemy && other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            player?.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (faction == BulletFaction.Friendly && other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<Enemy>();
            enemy?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
