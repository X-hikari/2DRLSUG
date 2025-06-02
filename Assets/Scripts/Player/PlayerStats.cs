using System;
using UnityEngine;

public class PlayerStats
{
    public float baseMoveSpeed = 3f;
    public int baseMaxHp = 100;
    public int baseAttack = 0;
    public int currentHp;

    public int level = 1;
    public int currentExp = 0;

    public int ExpToNextLevel => 100 * level;

    private BuffManager buffManager;

    public PlayerStats(BuffManager buffManager)
    {
        this.buffManager = buffManager;
        currentHp = MaxHp;
    }

    public float MoveSpeed => Mathf.Max(0, baseMoveSpeed + buffManager.GetNumericBuffValue(PlayerAttribute.MoveSpeed));
    public int MaxHp => Mathf.Max(1, baseMaxHp + (int)buffManager.GetNumericBuffValue(PlayerAttribute.MaxHealth));
    public int Attack => Mathf.Max(0, baseAttack + (int)buffManager.GetNumericBuffValue(PlayerAttribute.Attack));
    
    public void Heal(int amount)
    {
        currentHp = Mathf.Min(currentHp + amount, MaxHp);
    }

    public bool TakeDamage(float damage)
    {
        currentHp -= (int)damage;
        return currentHp <= 0;
    }

    public bool GainExp(int amount)
    {
        currentExp += amount;
        if (currentExp >= ExpToNextLevel)
        {
            currentExp -= ExpToNextLevel;
            level++;
            return true; // Leveled up
        }
        return false;
    }
}
