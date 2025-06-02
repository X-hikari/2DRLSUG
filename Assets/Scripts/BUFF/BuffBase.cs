using System;

public abstract class BuffBase {
    public string BuffName { get; protected set; }
    public BuffEffectType BuffType { get; protected set; }
    public float Duration { get; protected set; }   // 持续时间（秒）
    protected float elapsedTime = 0f;               // 已经过时间

    public bool IsExpired => elapsedTime >= Duration;

    public BuffBase(string buffName, BuffEffectType buffType, float duration) {
        BuffName = buffName;
        BuffType = buffType;
        Duration = duration;
    }

    // 作用于玩家，初始生效
    public abstract void OnApply(Player player);

    // 每帧更新，deltaTime单位秒
    public virtual void OnUpdate(float deltaTime, Player player) {
        elapsedTime += deltaTime;
    }

    // Buff到期或被移除时调用
    public abstract void OnRemove(Player player);
}
