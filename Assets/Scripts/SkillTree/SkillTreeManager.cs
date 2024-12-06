using UnityEngine;
using TMPro;
using UnityEngine.Events;
//using UnityEditor;

public enum skillTypes { 
    //general skills//
    General_HP,Dash_Colldown,Trail_Damage,Trail_Duration,Dash_Speed,General_XP,
    //triangle skills//
    Triangle_Damage, Triangle_Gravity, Triangle_DOT, Triangle_Stun, Triangle_Support, Triangle_6,
    //square skills//
    Square_Damage, Square_Slow, Square_Flame, Square_Trap, Square_Heal, Square_6,
    //pentagon skills//
    Pentagon_Damage, Pentagon_Blade, Pentagon_Implosion, Pentagon_Drain, Pentagon_Bomb, Pentagon_6,
    //hexagon skills//
    Hexagon_Damage, Hexagon_Meteor, Hexagon_Lightning, Hexagon_Area, Hexagon_Slow, Hexagon_6,
    //pentagram skills//
    Pentagram_Damage, Pentagram_Duration, Pentagram_Critical, Pentagram_Start, Pentagram_Octagon, Pentagram_6,
    //branch7 skills//
    Branch7_1, Branch7_2, Branch7_3, Branch7_4, Branch7_5, Branch7_6
};

public class SkillTreeManager : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent<skillTypes> skillUnlocked = new UnityEvent<skillTypes>();
    private int availablePoints;  // Total points for the player to spend
    [SerializeField]
    TextMeshProUGUI skillPointsText;  // UI text for displaying skill points


    private void Start()
    {
        availablePoints = (int)GameManager.GetSkillPoint();
        UpdateSkillPointsUI();  // Initial display of skill points
        UpdateSkillSlots();
    }

    // Function to unlock a skill
    /*public bool UnlockSkill(Skill skill)
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
    }*/

    private void UpdateSkillSlots()
    {
        for (int i = 0; i < GameManager.skillLevels.Length; i++)
        {
            if (GameManager.skillLevels[i] > 0)
            {
                skillUnlocked.Invoke((skillTypes)i);
            }
        }
    }
    // Method to update the skill points text UI
    private void UpdateSkillPointsUI()
    {
        if (skillPointsText != null)
        {
            skillPointsText.text = $"Skill Points: {availablePoints}";
        }
    }

    //function used to create all the skills scriptable objects. uncomment "using UnityEditor" to use//
    /*private void CreateSkillObject()
    {
        for (int i = 0; i < 42; i++)
        {
            SkillScriptableObject skillObject = ScriptableObject.CreateInstance<SkillScriptableObject>();
            AssetDatabase.CreateAsset(skillObject, $"Assets/ScriptableObjects/Skill/Skill{i+1}.asset");
        }

    }*/
}
