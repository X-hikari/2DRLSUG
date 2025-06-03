using UnityEngine;

public class SkillExecutor
{
    private readonly GameObject caster;

    public SkillExecutor(GameObject caster)
    {
        this.caster = caster;
    }

    public void ExecuteSkill(SkillData skill)
    {
        foreach (var instruction in skill.instructions)
        {
            SkillInstructionParser.ParseAndExecute(instruction, caster);
        }
    }
}
