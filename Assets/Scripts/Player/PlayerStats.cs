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
        GameManager.Instance.MaxHP = MaxHp;
        GameManager.Instance.CurrentHP = currentHp;
        GameManager.Instance.CurrentExp = currentExp;
        GameManager.Instance.CurrentLevel = level;
        GameManager.Instance.MaxMana = data.maxMana; ;
        GameManager.Instance.CurrentMana = data.currentMana;

    }

    public float MoveSpeed => Mathf.Max(0, data.baseMoveSpeed + buffManager.GetNumericBuffValue(PlayerAttribute.MoveSpeed));
    public int MaxHp => Mathf.Max(1, data.baseMaxHp + (int)buffManager.GetNumericBuffValue(PlayerAttribute.MaxHealth));
    public int Attack => Mathf.Max(0, data.baseAttack + (int)buffManager.GetNumericBuffValue(PlayerAttribute.Attack));
    public int ExpToNextLevel => 100 * level;

    public void Heal(int amount)
    {
        currentHp = Mathf.Min(currentHp + amount, MaxHp);
        GameManager.Instance.CurrentHP = currentHp;
    }

    public bool TakeDamage(float damage)
    {
        currentHp -= (int)damage;
        GameManager.Instance.CurrentHP =currentHp; 
        return currentHp <= 0;
    }

    public bool GainExp(int amount)
    {
        currentExp += amount;
        if (currentExp >= ExpToNextLevel)
        {
            currentExp -= ExpToNextLevel;
            level++;
            GameManager.Instance.CurrentLevel = level;
            return true; // Leveled up
        }
        GameManager.Instance.CurrentExp = currentExp;
        return false;
    }

    public void LevelUP()
    {
        data.baseMaxHp += 20;
        currentHp = data.baseMaxHp;
        data.maxMana += 10;
        data.currentMana = data.maxMana;
    }

    public float GetCurrentMana()
    {
        return data.currentMana;
    }

    public bool UseMana(float amount)
    {
        if (data.currentMana < amount) return false;
        data.currentMana -= amount;
        GameManager.Instance.CurrentMana = data.currentMana;
        return true;
    }
    
}

