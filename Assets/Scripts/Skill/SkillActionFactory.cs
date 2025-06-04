using System;
using UnityEngine;

public static class SkillActionFactory
{
    public static ISkillAction ParseActionFromInstruction2Periodic(SkillData skillData, string[] messages = null)
    {
        if (skillData.instructions == null || skillData.instructions.Count == 0)
        {
            Debug.LogWarning($"技能 {skillData.id} 没有指令内容");
            return null;
        }

        var instruction = skillData.instructions[0]; // 只有一个
        return new PeriodicSkillAction((deltaTime, player) =>
            {
                SkillInstructionParser.ParseAndExecute(instruction, player.gameObject, messages);
            });
    }

    public static ISkillAction ParseActionFromInstruction2Event(SkillData skillData, string[] messages = null)
    {
        if (skillData.instructions == null || skillData.instructions.Count == 0)
        {
            Debug.LogWarning($"技能 {skillData.id} 没有指令内容");
            return null;
        }

        var instruction = skillData.instructions[0]; // 只有一个
        return new EventAction((player, sender, args) =>
            {
                SkillInstructionParser.ParseAndExecute(instruction, player.gameObject, messages);
            });
    }
}

