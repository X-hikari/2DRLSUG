public class StatusBuff : BuffBase {
    public PlayerStatus Status { get; private set; }
    public bool Value { get; private set; } // true表示加状态，false表示移除状态

    public StatusBuff(string name, PlayerStatus status, bool value, float duration)
        : base(name, BuffEffectType.Status, duration) {
        Status = status;
        Value = value;
    }

    public override void OnApply(Player player) {
        player.SetStatus(Status, Value);
    }

    public override void OnUpdate(float deltaTime, Player player) {
        base.OnUpdate(deltaTime, player);
        // 状态Buff不需要每帧操作
    }

    public override void OnRemove(Player player) {
        player.SetStatus(Status, !Value);  // 解除状态
    }
}
