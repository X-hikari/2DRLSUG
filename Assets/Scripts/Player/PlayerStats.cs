using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerStats
{
    private BuffManager buffManager;

    private PlayerStatsData data;

    public int currentHp;
    public int level;
    public int currentExp;

    public PlayerStats(BuffManager buffManager, PlayerStatsData data)
    {
        this.buffManager = buffManager;
        this.data = data;

        level = data.baseLevel;
        currentExp = data.baseExp;
        currentHp = MaxHp;
    }

    public float MoveSpeed => Mathf.Max(0, data.baseMoveSpeed + buffManager.GetNumericBuffValue(PlayerAttribute.MoveSpeed));
    public int MaxHp => Mathf.Max(1, data.baseMaxHp + (int)buffManager.GetNumericBuffValue(PlayerAttribute.MaxHealth));
    public int Attack => Mathf.Max(0, data.baseAttack + (int)buffManager.GetNumericBuffValue(PlayerAttribute.Attack));
    public int ExpToNextLevel => 100 * level;

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

    public float GetCurrentMana()
    {
        return data.currentMana;
    }

    public bool UseMana(float amount)
    {
        if (data.currentMana < amount) return false;
        data.currentMana -= amount;
        return true;
    }
}

