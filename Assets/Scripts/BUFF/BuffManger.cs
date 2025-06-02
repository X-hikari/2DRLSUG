using System.Collections.Generic;
using UnityEngine;

public class BuffManager
{
    private readonly List<BuffBase> activeBuffs = new();
    private readonly Dictionary<PlayerAttribute, float> numericBuffTotals = new();

    private Player player;

    public BuffManager(Player player)
    {
        this.player = player;
    }

    public void AddBuff(BuffBase buff)
    {
        // 可加重复判断，是否叠加等逻辑
        buff.OnApply(player);
        activeBuffs.Add(buff);
    }

    public void RemoveBuff(BuffBase buff)
    {
        buff.OnRemove(player);
        activeBuffs.Remove(buff);
    }

    public void Update(float deltaTime)
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            var buff = activeBuffs[i];
            buff.OnUpdate(deltaTime, player);
            if (buff.IsExpired)
            {
                RemoveBuff(buff);
            }
        }
    }

    public bool HasBuff(string buffName)
    {
        return activeBuffs.Exists(b => b.BuffName == buffName);
    }

    // --- NumericBuff 支持 ---
    public void AddNumericBuff(NumericBuff buff)
    {
        if (!numericBuffTotals.ContainsKey(buff.Attribute))
            numericBuffTotals[buff.Attribute] = 0;

        numericBuffTotals[buff.Attribute] += buff.Value;
    }

    public void RemoveNumericBuff(NumericBuff buff)
    {
        if (!numericBuffTotals.ContainsKey(buff.Attribute)) return;

        numericBuffTotals[buff.Attribute] -= buff.Value;

        // 如果数值接近 0，清理
        if (Mathf.Approximately(numericBuffTotals[buff.Attribute], 0))
            numericBuffTotals.Remove(buff.Attribute);
    }

    public float GetNumericBuffValue(PlayerAttribute attribute)
    {
        return numericBuffTotals.TryGetValue(attribute, out float value) ? value : 0f;
    }
}
