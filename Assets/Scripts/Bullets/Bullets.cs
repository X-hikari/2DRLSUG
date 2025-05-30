using UnityEngine;

public enum BulletFaction
{
    Friendly,
    Enemy
}

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class Bullet : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 1;
    public Vector2 direction = Vector2.right;
    public BulletFaction faction;

    private void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime);
    }

    public void Init(Sprite sprite, int dmg, Vector2 dir, BulletFaction bulletFaction)
    {
        damage = dmg;
        direction = dir;
        faction = bulletFaction;

        GetComponent<SpriteRenderer>().sprite = sprite;
        GetComponent<SpriteRenderer>().flipX = dir.x < 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // // 敌方子弹碰到玩家
        // if (faction == BulletFaction.Enemy && other.CompareTag("Player"))
        // {
        //     var player = other.GetComponent<Player>();
        //     if (player != null) player.TakeDamage(damage);
        //     Destroy(gameObject);
        // }
        // // 我方子弹碰到敌人
        // else if (faction == BulletFaction.Friendly && other.CompareTag("Enemy"))
        // {
        //     var enemy = other.GetComponent<Enemy>();
        //     if (enemy != null) enemy.TakeDamage(damage);
        //     Destroy(gameObject);
        // }
    }
}
