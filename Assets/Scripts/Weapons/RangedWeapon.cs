using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 玩家用的远程武器（枪械），带攻击动画、发射子弹
/// </summary>
public class RangedWeapon : Weapon
{
    public override void Attack(Vector2 direction)
    {
        // 发射子弹
        FireBullet(direction);

        // 播放攻击动画（如果有动画控制器）
        if (weaponRenderer != null && data.animationController != null)
        {
            weaponRenderer.PlayAnimation("Attack");
        }
    }

    private void FireBullet(Vector2 direction)
    {
        if (data.projectilePrefab == null)
        {
            Debug.LogWarning($"RangedWeapon: {data.weaponName} 缺少 projectilePrefab，无法开火");
            return;
        }

        GameObject bulletGO = Instantiate(data.projectilePrefab, transform.position, Quaternion.identity);

        Bullet bulletScript = bulletGO.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            // 加载动画帧（如果你的 Bullet 支持帧动画渲染）
            List<Sprite> bulletSprites = data.bulletSprites;

            bulletScript.Init(
                bulletSprites,     // 帧动画（可为空）
                data.damage,       // 伤害
                direction.normalized,
                BulletFaction.Friendly // 区分阵营
            );
        }
        else
        {
            Debug.LogWarning("RangedWeapon: projectilePrefab 缺少 Bullet 脚本");
        }
    }
}
