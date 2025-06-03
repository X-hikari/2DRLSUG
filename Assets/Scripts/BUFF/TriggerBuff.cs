using System;

public class TriggerBuff : BuffBase
{
    private int maxTriggerCount;
    private int currentCount = 0;
    private Player player;

    private string eventName;
    private EventType eventType;
    private Action<Player, object, EventArgs> triggerEffect;

    public TriggerBuff(string name, float duration,
                       string eventName, EventType eventType,
                       Action<Player, object, EventArgs> effect,
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

        // 这里调用你的静态判断函数，传入事件名称
        if (BuffTriggerConditions.CheckCondition(eventName, sender, args))
        {
            triggerEffect(player, sender, args);

            currentCount++;
            if (maxTriggerCount > 0 && currentCount >= maxTriggerCount)
            {
                player.buffManager.RemoveBuff(this);
            }
        }
    }
}
