using System;

public class AttackHitEventArgs : EventArgs
{
    public Enemy TargetEnemy { get; private set; }
    public int Damage { get; private set; }

    public AttackHitEventArgs(Enemy target, int damage)
    {
        TargetEnemy = target;
        Damage = damage;
    }
}

public class EnemyKilledEventArgs : EventArgs
{
    public Enemy KilledEnemy { get; private set; }

    public EnemyKilledEventArgs(Enemy killed)
    {
        KilledEnemy = killed;
    }
}
