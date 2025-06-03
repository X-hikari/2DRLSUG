using System.Collections.Generic;
using UnityEngine;

public static class SkillDataLoader
{
    public static List<SkillData> LoadSkillData()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("SkillData");
        if (jsonText == null)
        {
            Debug.LogError("无法读取技能数据！");
            return null;
        }

        SkillWrapper wrapper = JsonUtility.FromJson<SkillWrapper>("{\"skills\":" + jsonText.text + "}");
        return wrapper.skills;
    }

    [System.Serializable]
    private class SkillWrapper
    {
        public List<SkillData> skills;
    }
}
