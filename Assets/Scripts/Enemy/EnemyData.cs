using System;
using System.Collections.Generic;

[Serializable]
public class SkillData
{
    public string name;
    public List<string> commands;
}

[Serializable]
public class EnemyData
{
    public string id;
    public string name;
    public int hp;
    public int attack;
    public string spriteSheet;
    public Dictionary<string, List<string>> animations;
    public List<SkillData> skills;
}