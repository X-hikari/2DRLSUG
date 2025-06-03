using System;
using System.Collections.Generic;

[Serializable]
public class SkillData
{
    public string id;
    public string name;
    public float cooldown;
    public float manaCost;

    public SkillCategory category;                // 枚举类型：Buff、Direct、Summon
    public List<SkillInstruction> instructions;   // 指令集，类型由 category 决定
}

[Serializable]
public class SkillInstruction
{
    public string type;   // 指令类型
    public string param;  // 参数，类型因 type 而异，一般为简单字符串或嵌套 JSON 字符串
}

public enum SkillCategory
{
    ownSkill,       // 直接调用成员自带方法的技能，例如player.Heal
    Buff,           // 添加Buff类技能
    DirectDamage,   // 直接伤害类技能（没有新的实体创建，例如直接对附近最近的敌人造成）
    Summon,         // 会生成实体的伤害类技能，例如火球术等
    beckon          // 召唤类技能
}
