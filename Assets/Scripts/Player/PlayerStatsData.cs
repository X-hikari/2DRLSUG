using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsData", menuName = "Player/Player Stats Data")]
public class PlayerStatsData : ScriptableObject
{
    public float baseMoveSpeed = 3f;
    public int baseMaxHp = 100;
    public int baseAttack = 0;
    public int baseLevel = 1;
    public int baseExp = 0;
}
