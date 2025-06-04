using System;

public interface ISkillAction
{
    void Execute(Player player, float deltaTime = 0f);
    void ExecuteEvent(Player player, object sender, EventArgs args);
}

public class PeriodicSkillAction : ISkillAction
{
    private readonly Action<float, Player> action;
    public PeriodicSkillAction(Action<float, Player> action) => this.action = action;

    public void Execute(Player player, float deltaTime = 0f) => action?.Invoke(deltaTime, player);
    public void ExecuteEvent(Player player, object sender, EventArgs args) { } // 不做事
}

public class EventAction : ISkillAction
{
    private readonly Action<Player, object, EventArgs> action;
    public EventAction(Action<Player, object, EventArgs> action) => this.action = action;

    public void Execute(Player player, float deltaTime = 0f) { } // 不做事
    public void ExecuteEvent(Player player, object sender, EventArgs args) => action?.Invoke(player, sender, args);
}
