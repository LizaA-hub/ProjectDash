using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillTreeManager : MonoBehaviour
{
    public List<Skill> allSkills;  // All skills in the tree
    public int availablePoints = 10;  // Total points for the player to spend
    public TextMeshProUGUI skillPointsText;  // UI text for displaying skill points

    private void Start()
    {
        availablePoints = (int)GameManager.GetSkillPoint();
        UpdateSkillPointsUI();  // Initial display of skill points
    }

    // Function to unlock a skill
    public bool UnlockSkill(Skill skill)
    {
        // Check if skill is already unlocked
        if (skill.isUnlocked)
        {
            Debug.Log($"{skill.skillName} is already unlocked!");
            return false;
        }

        // Check if player has enough points
        if (availablePoints < skill.cost)
        {
            Debug.Log("Not enough points to unlock this skill.");
            return false;
        }

        // Check if prerequisites are met
        foreach (Skill prereq in skill.requiredSkills)
        {
            if (!prereq.isUnlocked)
            {
                Debug.Log($"Cannot unlock {skill.skillName}; prerequisite {prereq.skillName} not unlocked.");
                return false;
            }
        }

        // Unlock the skill
        skill.isUnlocked = true;
        availablePoints -= skill.cost;
        Debug.Log($"{skill.skillName} unlocked!");
        //update static variables//
        GameManager.ModifySkillPoint(-skill.cost);

        UpdateSkillPointsUI();  // Update the UI with the new skill points total
        return true;
    }

    // Reset all skills (for testing or respec feature)
    public void ResetSkills()
    {
        foreach (Skill skill in allSkills)
        {
            skill.isUnlocked = false;
        }
        availablePoints = 10;  // Reset points as desired
        UpdateSkillPointsUI();  // Update the UI after resetting
    }

    // Method to update the skill points text UI
    private void UpdateSkillPointsUI()
    {
        if (skillPointsText != null)
        {
            skillPointsText.text = $"Skill Points: {availablePoints}";
        }
    }
}
