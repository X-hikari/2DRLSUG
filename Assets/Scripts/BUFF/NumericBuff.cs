public class NumericBuff : BuffBase
{
    public PlayerAttribute Attribute { get; private set; }
    public float Value { get; private set; }

    public NumericBuff(string name, PlayerAttribute attribute, float value, float duration)
        : base(name, BuffEffectType.Numeric, duration)
    {
        Attribute = attribute;
        Value = value;
    }

    public override void OnApply(Player player)
    {
        player.buffManager.AddNumericBuff(this);
    }

    public override void OnUpdate(float deltaTime, Player player)
    {
        base.OnUpdate(deltaTime, player);
    }

    public override void OnRemove(Player player)
    {
        player.buffManager.RemoveNumericBuff(this);
    }
}
