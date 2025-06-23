using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using System.Linq;

public class SkillInstructionParser
{
    readonly static List<SkillData> allSkills = SkillDataLoader.LoadSkillData();

    public static SkillData GetSkillById(string skillId)
    {
        if (string.IsNullOrEmpty(skillId) || allSkills == null)
            return null;

        return allSkills.FirstOrDefault(skill => skill.id == skillId);
    }

    public static (string id, string[] args) ParseSkillCall(string input)
    {
        if (string.IsNullOrEmpty(input))
            return (null, Array.Empty<string>());

        string[] parts = input.Split('-');

        if (parts.Length == 0)
            return (null, Array.Empty<string>());

        string id = parts[0];
        string[] args = new string[parts.Length - 1];
        Array.Copy(parts, 1, args, 0, args.Length);

        return (id, args);
    }

    public static void ParseAndExecute(SkillInstruction instruction, GameObject caster, params string[] messages)
    {
        switch (instruction.type)
        {
            case "ownSkill":
                ExcuteOwnSkill(instruction.param, caster, messages);
                break;
            case "Buff":
                ExcuteBuffSkill(instruction.param, caster);
                break;
            case "DirectDamage":
                ExcuteSkillDirectDamage(instruction.param, caster);
                break;
            case "Summon":
                break;
            case "beckon":
                break;
            default:
                Debug.Log("无效的分类");
                break;
        }
    }

    static void ExcuteBuffSkill(string json, GameObject caster)
    {
        var obj = JObject.Parse(json);
        string type = (string)obj["type"];
        string name = (string)obj["name"];
        float duration = (float)obj["duration"];
        BuffBase buff = null;
        SkillData SkillData;
        ISkillAction effect = null;
        string[] messages;
        string id;
        string action;
        switch (type)
        {
            case "Numeric":
                float value = (float)obj["value"];
                string stat = (string)obj["stat"];
                PlayerAttribute attribute = (PlayerAttribute)System.Enum.Parse(typeof(PlayerAttribute), stat);
                float interval = (float)(obj["periodicInterval"] ?? 0);
                action = (string)(obj["periodicAction"] ?? null);
                if (action != null)
                {
                    (id, messages) = ParseSkillCall(action);
                    SkillData = SkillInstructionParser.GetSkillById(id);
                    effect = SkillActionFactory.ParseActionFromInstruction2Periodic(SkillData, messages);
                }
                buff = new NumericBuff(name, attribute, value, duration, interval, effect);
                break;
            case "Status":
                bool boolvalue = (bool)obj["value"];
                string status = (string)obj["status"];
                PlayerStatus STDstatus = (PlayerStatus)System.Enum.Parse(typeof(PlayerStatus), status);
                buff = new StatusBuff(name, STDstatus, boolvalue, duration);
                break;
            case "Trigger":
                string eventName = (string)obj["eventName"];
                string eventType = (string)obj["eventType"];
                EventType STDeventType = (EventType)System.Enum.Parse(typeof(EventType), eventType);
                int maxCount = (int)obj["maxCount"];
                action = (string)obj["effect"];
                (id, messages) = ParseSkillCall(action);
                SkillData = SkillInstructionParser.GetSkillById(id);
                effect = SkillActionFactory.ParseActionFromInstruction2Event(SkillData, messages);
                buff = new TriggerBuff(name, duration, eventName, STDeventType, effect, maxCount);
                break;
            default:
                Debug.Log("无效的Buff类技能分类");
                break;
        }
        caster.GetComponent<Player>().buffManager.AddBuff(buff);
    }

    static void ExcuteOwnSkill(string json, GameObject caster, params string[] messages)
    {
        var obj = JObject.Parse(json);
        string funcName = (string)obj["type"];
        int value = (int)obj["value"];
        foreach (var msg in messages)
        {
            // 现在只考虑最简单的情况，更复杂的以后再说吧
            if (int.TryParse(msg, out int number))
            {
                value = number;
            }
        }
        var player = caster.GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("caster 上没有 Player 组件！");
            return;
        }

        // 用字典映射函数名到具体方法
        var actions = new Dictionary<string, Action<int>> {
            { "Heal", player.Heal },
            // { "TakeDamage", player.TakeDamage }
            // 可扩展更多方法
        };

        // 尝试调用对应方法
        if (actions.TryGetValue(funcName, out var action))
        {
            action(value);
            Debug.Log($"成功调用 {funcName}({value})");
        }
    }

    static void ExcuteSkillDirectDamage(string json, GameObject caster)
    {
        var obj = JObject.Parse(json);
        Player player = caster.GetComponent<Player>();
        string type = (string)obj["type"];
        string targets = (string)obj["targets"];
        string condition = (string)obj["condition"];
        int value = (int)obj["value"];

        GameObject[] candidates = GameObject.FindGameObjectsWithTag(targets);
        GameObject target = null;

        switch (condition)
        {
            case "nearest":
                float minDistance = float.MaxValue;
                Vector3 playerPos = player.transform.position;

                foreach (var c in candidates)
                {
                    float dist = Vector3.Distance(playerPos, c.transform.position);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        target = c;
                    }
                }
                break;
            case "minHP":
                float minHP = float.MaxValue;
                foreach (var c in candidates)
                {
                    var unit = c.GetComponent<Enemy>();  
                    if (unit != null && unit.GetCurrentHp() < minHP)
                    {
                        minHP = unit.GetCurrentHp();
                        target = c;
                    }
                }
                break;
            default:
                Debug.LogWarning("Unknown condition: " + condition);
                break;
        }

        if (target)
        {
            switch (type)
            {
                case "Damage":
                    target.GetComponent<Enemy>().TakeDamage(value);     // 默认为Enemy，其他物品的解决方案同上
                    Debug.Log($"对 {condition} 敌人造成 {value} 的伤害");
                    break;
                default:
                    break;
            }
        }
    }
}
