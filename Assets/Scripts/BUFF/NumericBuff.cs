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
        // 不再直接修改 PlayerStats，而是通过 BuffManager 实时叠加
        player.buffManager.AddNumericBuff(this);
    }

    public override void OnUpdate(float deltaTime, Player player)
    {
        base.OnUpdate(deltaTime, player);
        // NumericBuff 无需每帧更新
    }

    public override void OnRemove(Player player)
    {
        player.buffManager.RemoveNumericBuff(this);
    }
}
