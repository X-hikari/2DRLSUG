using System.Collections.Generic;
using UnityEngine;

public class SkillExecutor
{
    private readonly GameObject caster;
    private Dictionary<string, float> cooldownDict = new();

    public SkillExecutor(GameObject caster)
    {
        this.caster = caster;
    }

    public void ExecuteSkill(SkillData skill)
    {
        float now = Time.time;
        // 冷却检查
        if (cooldownDict.TryGetValue(skill.id, out float lastCastTime))
        {
            if (now - lastCastTime < skill.cooldown)
            {
                Debug.Log($"技能 {skill.name} 冷却中！");
                return;
            }
        }

        // 法力检查
        Player player = caster.GetComponent<Player>();
        if (player.stats.currentMana < skill.manaCost)
        {
            Debug.Log($"法力不足，无法释放技能 {skill.name}");
            return;
        }

        player.stats.UseMana(skill.manaCost);

        // 设置冷却
        cooldownDict[skill.id] = now;

        foreach (var instruction in skill.instructions)
        {
            SkillInstructionParser.ParseAndExecute(instruction, caster);
        }
    }
}
