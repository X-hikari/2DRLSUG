public class NumericBuff : BuffBase
{
    public PlayerAttribute Attribute { get; private set; }
    public float Value { get; private set; }

    private ISkillAction periodicAction; // 修改为统一接口

    private float timer = 0f;
    private float interval = 0f;

    public NumericBuff(string name, PlayerAttribute attribute, float value, float duration,
                      float periodicInterval = 0f,
                      ISkillAction periodicAction = null) // 使用统一接口
        : base(name, BuffEffectType.Numeric, duration)
    {
        Attribute = attribute;
        Value = value;
        interval = periodicInterval;
        this.periodicAction = periodicAction;
    }

    public override void OnApply(Player player)
    {
        player.buffManager.AddNumericBuff(this);
    }

    public override void OnUpdate(float deltaTime, Player player)
    {
        base.OnUpdate(deltaTime, player);

        if (interval > 0f && periodicAction != null)
        {
            timer += deltaTime;
            if (timer >= interval)
            {
                timer -= interval;
                periodicAction.Execute(player, timer); // 统一接口调用
            }
        }
    }

    public override void OnRemove(Player player)
    {
        player.buffManager.RemoveNumericBuff(this);
    }
}
