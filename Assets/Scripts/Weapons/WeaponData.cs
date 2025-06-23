using System.Collections.Generic;
using UnityEngine;

// 可复用的武器配置文件（用于所有武器）
[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("基础信息")]
    public string weaponName;
    public Sprite icon;
    public Sprite idleSprite; // 默认显示的 sprite

    [Header("战斗属性")]
    public int damage = 10;
    public float attackInterval = 1f;
    public float attackRange = 2f;
    public float stabDuration = 1f;


    [Header("动画片段（用于覆盖默认 Animator）")]
    public RuntimeAnimatorController animationController; // 可选动画控制器
    public AnimationClip idleClip;
    public AnimationClip attackClip;

    [Header("子弹")]
    public GameObject projectilePrefab; // 子弹、回旋镖等投射物预制体
    public List<Sprite> bulletSprites;
}
