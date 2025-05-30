using System;
using System.Collections.Generic;
using UnityEngine;

public static class SkillParser
{
    public static void Execute(List<string> commands)
    {
        foreach (string cmd in commands)
        {
            if (cmd.StartsWith("damage:"))
            {
                int dmg = int.Parse(cmd.Split(':')[1]);
                Debug.Log($"造成 {dmg} 点伤害");
            }
            else if (cmd.StartsWith("dot:"))
            {
                string[] args = cmd.Split(':')[1].Split(',');
                int dmgPerTick = int.Parse(args[0]);
                int ticks = int.Parse(args[1]);
                Debug.Log($"造成 {dmgPerTick} x {ticks} 点持续伤害");
            }
            else if (cmd.StartsWith("regenerate:"))
            {
                int blood = int.Parse(cmd.Split(':')[1]);
                // Debug.Log($"回复 {blood} 点血量");
            }
            else
            {
                Debug.LogWarning($"无法识别技能指令：{cmd}");
            }
        }
    }
}
