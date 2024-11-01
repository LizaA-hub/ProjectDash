using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skill Tree/Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public string description;
    public Sprite icon;
    public List<Skill> requiredSkills;  // Skills needed to unlock this one
    public bool isUnlocked = false;
    public int cost = 1;  // Cost of unlocking the skill
}
