using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq; // 推荐引入 Json.NET

public static class SkillInstructionParser
{
    public static void ParseAndExecute(SkillInstruction instruction, GameObject caster)
    {
        // switch (instruction.type)
        // {
        //     case "Numeric":
        //         ExecuteNumeric(instruction.param, caster);
        //         break;
        //     case "Status":
        //         ExecuteStatus(instruction.param, caster);
        //         break;
        //     case "Trigger":
        //         ExecuteTrigger(instruction.param, caster);
        //         break;
        //     case "Damage":
        //         ExecuteDamage(instruction.param, caster);
        //         break;
        //     case "Heal":
        //         ExecuteHeal(instruction.param, caster);
        //         break;
        //     default:
        //         Debug.LogWarning("未知指令类型: " + instruction.type);
        //         break;
        // }
    }
}
