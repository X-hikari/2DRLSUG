using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsData", menuName = "Player/Player Stats Data")]
public class PlayerStatsData : ScriptableObject
{
    public float baseMoveSpeed = 3f;
    public int baseMaxHp = 100;
    public int baseAttack = 0;
    public int baseLevel = 1;
    public int baseExp = 0;
    public float maxMana = 100f;
    public float currentMana = 100f;
    public string playerName;
    public Sprite icon;
    public Sprite idleSprite;

    public void Init()
    {
        baseMoveSpeed = 3f;
        baseMaxHp = 100;
        baseAttack = 0;
        baseLevel = 1;
        baseExp = 0;
        maxMana = 100f;
        currentMana = 100f;
    }
}
