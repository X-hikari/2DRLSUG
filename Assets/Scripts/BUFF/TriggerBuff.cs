using System;
using System.Diagnostics;

public class TriggerBuff : BuffBase
{
    private int maxTriggerCount;
    private int currentCount = 0;
    private Player player;

    private string eventName;
    private EventType eventType;
    private ISkillAction triggerEffect; // 原来是 Action<Player, object, EventArgs>

    public TriggerBuff(string name, float duration,
                       string eventName, EventType eventType,
                       ISkillAction effect,
                       int maxCount = -1)
        : base(name, BuffEffectType.Trigger, duration)
    {
        this.eventName = eventName;
        this.eventType = eventType;
        this.triggerEffect = effect;
        this.maxTriggerCount = maxCount;
    }

    public override void OnApply(Player player)
    {
        this.player = player;
        EventManager.Subscribe(eventType, OnEvent);
    }

    public override void OnRemove(Player player)
    {
        EventManager.Unsubscribe(eventType, OnEvent);
        this.player = null;
    }

    private void OnEvent(object sender, EventArgs args)
    {
        if (triggerEffect == null || player == null)
            return;

        if (BuffTriggerConditions.CheckCondition(eventName, sender, args))
        {
            triggerEffect.ExecuteEvent(player, sender, args); // 统一接口调用
            currentCount++;
            if (maxTriggerCount > 0 && currentCount >= maxTriggerCount)
            {
                player.buffManager.RemoveBuff(this);
            }
        }
    }
}
