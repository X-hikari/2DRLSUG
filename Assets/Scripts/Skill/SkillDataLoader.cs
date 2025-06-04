using System.Collections.Generic;
using UnityEngine;

public static class SkillDataLoader
{
    public static List<SkillData> LoadSkillData()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("Data/Skills/SkillData");
        if (jsonText == null)
        {
            Debug.LogError("无法读取技能数据！");
            return null;
        }

        // 无法直接反序列化一个json数组，所以包装为 { "skills" : ... }
        SkillWrapper wrapper = JsonUtility.FromJson<SkillWrapper>("{\"skills\":" + jsonText.text + "}");
        return wrapper.skills;
    }

    [System.Serializable]
    private class SkillWrapper
    {
        public List<SkillData> skills;
    }
}
