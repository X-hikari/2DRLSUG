public static class BuffConditionRegister
{
    public static void RegisterAll()
    {
        BuffTriggerConditions.Register("OnAttackHit", (sender, args) => {
            if (args is AttackHitEventArgs hitArgs)
                return hitArgs.TargetEnemy != null && hitArgs.Damage > 0;
            return false;
        });

        BuffTriggerConditions.Register("OnEnemyKilled", (sender, args) => {
            if (args is EnemyKilledEventArgs killedArgs)
                return killedArgs.KilledEnemy != null;
            return false;
        });

        // 以后再加其他事件条件...
    }
}
