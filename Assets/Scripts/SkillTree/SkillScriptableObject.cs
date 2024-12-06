using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "ScriptableObjects/Skill")]
public class SkillScriptableObject : ScriptableObject
{
    public skillTypes type;
    public string skillName;
    public string description;
    public Sprite icon;
    public List<skillTypes> nextSkills;
    public int cost = 1;  // Cost of unlocking the skill
}
