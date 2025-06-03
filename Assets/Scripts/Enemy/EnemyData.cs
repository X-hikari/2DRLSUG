using System;
using System.Collections.Generic;

[Serializable]
public class EnemySkillData
{
    public string name;
    public List<string> commands;
}

[Serializable]
public class EnemyData
{
    public string id;
    public string name;
    public int exp;
    public int hp;
    public int attack;
    public float moveSpeed;
    public float attackCooldown;
    public string spriteSheet;
    public Dictionary<string, List<string>> animations;
    public List<EnemySkillData> skills;
    public string attackType; // "melee" 或 "ranged"
    public float attackRange; // 攻击距离
    public List<string> bulletSprites; // 子弹动画帧名（仅远程敌人需要）
}

[Serializable]
public class EnemyDataList
{
    public List<EnemyData> enemies;
}