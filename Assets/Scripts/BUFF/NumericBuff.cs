using System;

public class NumericBuff : BuffBase
{
    public PlayerAttribute Attribute { get; private set; }
    public float Value { get; private set; }

    // 可选的周期回调，外部传入，没传就是不周期执行
    private Action<float, Player> onPeriodicUpdate;

    private float timer = 0f;
    private float interval = 0f;

    public NumericBuff(string name, PlayerAttribute attribute, float value, float duration,
                      float periodicInterval = 0f,
                      Action<float, Player> periodicAction = null)
        : base(name, BuffEffectType.Numeric, duration)
    {
        Attribute = attribute;
        Value = value;
        interval = periodicInterval;
        onPeriodicUpdate = periodicAction;
    }

    public override void OnApply(Player player)
    {
        player.buffManager.AddNumericBuff(this);
    }

    public override void OnUpdate(float deltaTime, Player player)
    {
        base.OnUpdate(deltaTime, player);

        if (interval > 0f && onPeriodicUpdate != null)
        {
            timer += deltaTime;
            if (timer >= interval)
            {
                timer -= interval;
                onPeriodicUpdate(timer, player);
            }
        }
    }

    public override void OnRemove(Player player)
    {
        player.buffManager.RemoveNumericBuff(this);
    }
}
