using System;
using System.Collections.Generic;

public delegate bool TriggerCondition(object sender, EventArgs args);

public static class BuffTriggerConditions
{
    private static Dictionary<string, TriggerCondition> conditions = new Dictionary<string, TriggerCondition>();

    public static void Register(string eventName, TriggerCondition condition)
    {
        conditions[eventName] = condition;
    }

    public static bool CheckCondition(string eventName, object sender, EventArgs args)
    {
        if (conditions.TryGetValue(eventName, out var cond))
        {
            return cond(sender, args);
        }
        return false;
    }
}
