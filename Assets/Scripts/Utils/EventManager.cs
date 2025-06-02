using System;
using System.Collections.Generic;

public static class EventManager
{
    private static Dictionary<EventType, Action<object, EventArgs>> eventTable = new();

    public static void Subscribe(EventType eventType, Action<object, EventArgs> listener)
    {
        if (!eventTable.ContainsKey(eventType))
            eventTable[eventType] = delegate { };
        eventTable[eventType] += listener;
    }

    public static void Unsubscribe(EventType eventType, Action<object, EventArgs> listener)
    {
        if (eventTable.ContainsKey(eventType))
            eventTable[eventType] -= listener;
    }

    public static void TriggerEvent(EventType eventType, object sender, EventArgs args)
    {
        if (eventTable.ContainsKey(eventType))
            eventTable[eventType]?.Invoke(sender, args);
    }
}

// 定义事件类型枚举
public enum EventType
{
    OnAttackHit,
    OnEnemyKilled,
    // 其他事件
}