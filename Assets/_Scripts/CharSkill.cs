using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;


[System.Serializable]
public class CharSkill
{
    public string skillId { get; set; }
    public string skillName { get; set; }
    public int skillCost { get; set; }
    public int skillLevel { get; set; }
    public SkillType skillType { get; set; }

    public bool skillActive;
    public int effectDuration;

    public Dictionary<Attributes, int> skillEffects = new Dictionary<Attributes, int>();

    public enum SkillType { AttribBonus, Self, Target, Group}

    public CharSkill(int id, string name, int cost, int level, int type, Dictionary<Attributes, int> effects)
    {
        skillId = skillId;
        skillName = name;
        skillCost = cost;
        skillLevel = level;
        skillType = (SkillType)type;

        foreach (KeyValuePair<Attributes, int> a in effects)
        {
            skillEffects.Add(a.Key, a.Value);
        }
    }
}
