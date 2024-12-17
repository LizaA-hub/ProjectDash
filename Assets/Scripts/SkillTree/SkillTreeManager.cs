using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Collections.Generic;
using Unity.Collections;
using System;
using static UnityEngine.Rendering.DebugUI;

public enum skillTypes { //WARNING: THIS ELEMENTS NEED TO STAY IN THE RIGHT ORDER FOR THE SYSTEM TO WORK//
    //general skills//
    General_HP,Dash_Cooldown,Trail_Damage,Trail_Duration,Dash_Speed,General_XP,
    //triangle skills//
    Triangle_Damage, Triangle_Gravity, Triangle_DOT, Triangle_Stun, Triangle_Support, Triangle_6,
    //square skills//
    Square_Damage, Square_Slow, Square_Flame, Square_Trap, Square_Heal, Square_6,
    //pentagon skills//
    Pentagon_Damage, Pentagon_Blade, Pentagon_Implosion, Pentagon_Drain, Pentagon_Bomb, Pentagon_6,
    //hexagon skills//
    Hexagon_Damage, Hexagon_Meteor, Hexagon_Lightning, Hexagon_Area, Hexagon_Slow, Hexagon_6,
    //pentagram skills//
    Pentagram_Duration, Pentagram_Damage, Pentagram_Critical, Pentagram_Star, Pentagram_Octagon, Pentagram_6,
    //branch7 skills//
    Branch7_1, Branch7_2, Branch7_3, Branch7_4, Branch7_5, Branch7_6
};

public class SkillTreeManager : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent<skillTypes> skillUnlocked = new UnityEvent<skillTypes>(), unlockAdjacent = new UnityEvent<skillTypes>();
    public UnityEvent resetSkills = new UnityEvent();
    [SerializeField]
    TextMeshProUGUI skillPointsText, skillName, skillDescription, skillStat, skillCost, skillLevel, lockPanelText;
    [SerializeField]
    GameObject infoPanel, costPanel,lockPanel;
    SkillScriptableObject selectedSkill;

    private int availablePoints;
    List<skillTypes> unlockedSkills = new List<skillTypes>();

    #region Unity Functions

    private void Start()
    {
        availablePoints = Mathf.RoundToInt(GameManager.GetSkillPoint());
        
        UpdateSkillPointsUI();  // Initial display of skill points
        UpdateSkillSlots();
    }

    #endregion
    #region Public Functions
    public void ShowPanel(SkillScriptableObject skill)
    {
        if(selectedSkill == skill && infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
            selectedSkill = null;
        }
        else
        {
            selectedSkill = skill;
            infoPanel.SetActive(true);
            SetPanelText();
        }
        
    }

    public void UnlockSkills(List<skillTypes> skills)
    {
        foreach (var skill in skills)
        {
            skillUnlocked.Invoke(skill);
            unlockedSkills.Add(skill);
        }
    }

    public void LevelUPSkill()
    {
        if(selectedSkill.cost > availablePoints)
        {
            Debug.Log("not enough point!");
            return;
        }
        //update points//
        availablePoints -= selectedSkill.cost;
        GameManager.ModifySkillPoint(-selectedSkill.cost);
        UpdateSkillPointsUI();
        //update skill level//
        var level = GameManager.SkillEnhance(selectedSkill.type);
        if(level == 0)
        {
            Debug.Log("skill manager : can't enhance skill");
        }
        skillLevel.text = $"Level {level}";
        //to do : update level up cost
        if(level == 3)
        {
            unlockAdjacent.Invoke(selectedSkill.type);
        }
        else if(level == 5)
        {
            costPanel.SetActive(false);
            lockPanel.SetActive(true);
            lockPanelText.text = "Level Max reached";
            lockPanelText.color = Color.white;
        }

    }

    public void MainMenu()
    {
        GameManager.LoadMenu();
    }

    public void ResetSkillTree()
    {
        infoPanel.SetActive(false);
        selectedSkill = null;
        resetSkills.Invoke();
        GameManager.ResetSkillPoints();
        availablePoints = 0;
        UpdateSkillPointsUI();
        for (int i = 0; i < GameManager.GetSkillLevels().Length; i++)
        {
            GameManager.SetSkillLevel(i, 0);
        }
        unlockedSkills.Clear();
        UpdateSkillSlots();
        GameManager.UpdateAllSkills();
    }

    public void SetPoints(string value)
    {
        if (float.TryParse(value, out float result))
        {
            availablePoints = Mathf.RoundToInt(result);
            GameManager.ResetSkillPoints();
            GameManager.ModifySkillPoint(availablePoints);
            UpdateSkillPointsUI();
        }
        else
        {
            Debug.Log("invalid format");
        }
    }

    #endregion
    #region Private Functions
    private void UpdateSkillSlots()
    {
        for (int i = 0; i < GameManager.GetSkillLevels().Length; i++)
        {
            if (GameManager.GetSkillLevelAt(i) > 0)
            {
                skillUnlocked.Invoke((skillTypes)i);
                unlockedSkills.Add((skillTypes)i);
                if(GameManager.GetSkillLevelAt(i) >= 3)
                {
                    unlockAdjacent.Invoke((skillTypes)i);
                }
            }
        }

        if(unlockedSkills.Count == 0)
        {
            skillUnlocked.Invoke(skillTypes.General_HP);
            unlockedSkills.Add(skillTypes.General_HP);
        }
    }
    // Method to update the skill points text UI
    private void UpdateSkillPointsUI()
    {
        if (skillPointsText != null)
        {
            skillPointsText.text = $"Skill Points: {availablePoints}";
            //Debug.Log(availablePoints);
        }
    }

    private void SetPanelText()
    {
        skillName.text = selectedSkill.skillName;

        if (unlockedSkills.Contains(selectedSkill.type)) { //if the skill is unlocked
            var level = GameManager.GetSkillLevelAt((int)selectedSkill.type); //show the current level
            skillLevel.text = $"Level {level}";
            skillLevel.color = Color.white;

            if(level < 5)
            {
                costPanel.SetActive(true); //show the cost to next level
                lockPanel.SetActive(false);
                skillCost.text = $"{selectedSkill.cost} points to next level";
            }
            else
            {
                costPanel.SetActive(false);
                lockPanel.SetActive(true);
                lockPanelText.text = "Level Max reached";
                lockPanelText.color = Color.white;
            }
            
        }
        else { //skill is locked
            skillLevel.text = "Locked";
            skillLevel.color = Color.red;

            costPanel.SetActive(false);
            lockPanel.SetActive(true);
            lockPanelText.text = "Upgrade adjacent skill to level 3 to unlock this";
            lockPanelText.color = Color.red;
        }

        skillDescription.text = selectedSkill.description;
        skillStat.text = selectedSkill.statInfo;

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
    #endregion
}
