using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;

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
    [HideInInspector]
    public UnityEvent resetSkills = new UnityEvent();
    [SerializeField]
    TextMeshProUGUI skillPointsText, skillName, skillDescription, skillStat, skillCost, skillLevel, lockPanelText;
    [SerializeField]
    GameObject infoPanel, costPanel,lockPanel, skillParent;
    [SerializeField]
    Toggle disableToggle;
    SkillScriptableObject selectedSkill;

    private int availablePoints;
    List<skillTypes> unlockedSkills = new List<skillTypes>();

    //camera movement variables//
    [SerializeField]
    float zoomFactor = 0.1f, smoothFactor = 1f, moveSpeed = 1f;
    Vector3  minPos = Vector3.zero, maxPos = Vector3.zero, targetZoom = Vector3.one;
    Camera cam;
    RectTransform rectTransform, skillTransform;
    Vector2 limits, result, targetPos, mousePos;
    float debug;

    #region Unity Functions

    private void Start()
    {
        availablePoints = Mathf.RoundToInt(GameManagerV2.instance.GetSkillPoint());
        
        UpdateSkillPointsUI();  // Initial display of skill points
        UpdateSkillSlots();

        cam = gameObject.GetComponent<Canvas>().worldCamera;
        rectTransform = GetComponent<RectTransform>();
        limits = new Vector2(rectTransform.sizeDelta.x * 5 / 12, rectTransform.sizeDelta.y * 5 / 12);
        skillTransform = skillParent.GetComponent<RectTransform>();
        targetPos = skillTransform.anchoredPosition;
    }

    private void OnGUI()
    {
        debug += Time.deltaTime;
        if(Input.mouseScrollDelta.y != 0f)
        {
            Zoom(Input.mouseScrollDelta.y);
        }

      
    }

    private void Update()
    {
        Vector3 currentScale = skillParent.transform.localScale;
        if(Vector3.Distance(currentScale, targetZoom) > 0.01f)
        {
            skillParent.transform.localScale = Vector3.Slerp(currentScale, targetZoom, Time.deltaTime * smoothFactor);
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, cam, out result);
        mousePos = result;
        if ((Mathf.Abs(mousePos.x) > limits.x) || (Mathf.Abs(mousePos.y) > limits.y)) { GetPositionTarget(); }
        /* (debug > 1f)
        {
            Debug.Log(mousePos);
            debug = 0;
        }*/

        if (Vector2.Distance(skillTransform.anchoredPosition,targetPos) > 0.01f)
        {
            skillTransform.anchoredPosition = Vector3.Lerp(skillTransform.anchoredPosition, targetPos, Time.deltaTime * smoothFactor);
        }
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
        if (!unlockedSkills.Contains(selectedSkill.type)) return;

        if(selectedSkill.cost > availablePoints)
        {
            Debug.Log("not enough point!");
            return;
        }

        var level = GameManagerV2.instance.GetSkillLevelAt((int)selectedSkill.type);
        if (level >= 5) return;

        //update points//
        availablePoints -= selectedSkill.cost;
        GameManagerV2.instance.ModifySkillPoint(-selectedSkill.cost);
        UpdateSkillPointsUI();
        //update skill level//
        level = GameManagerV2.instance.SkillEnhance(selectedSkill.type);
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
        GameManagerV2.instance.LoadMenu();
    }

    public void ResetSkillTree()
    {
        infoPanel.SetActive(false);
        selectedSkill = null;
        resetSkills.Invoke();
        GameManagerV2.instance.ResetSkillPoints();
        availablePoints = 0;
        UpdateSkillPointsUI();
        for (int i = 0; i < GameManagerV2.instance.GetSkillLevels().Length; i++)
        {
            GameManagerV2.instance.SetSkillLevel(i, 0);
            GameManagerV2.instance.SetSkilState(i, false);
        }
        unlockedSkills.Clear();
        UpdateSkillSlots();
        GameManagerV2.instance.UpdateAllSkills();
    }

    public void SetPoints(string value)
    {
        if (float.TryParse(value, out float result))
        {
            availablePoints = Mathf.RoundToInt(result);
            GameManagerV2.instance.ResetSkillPoints();
            GameManagerV2.instance.ModifySkillPoint(availablePoints);
            UpdateSkillPointsUI();
        }
        else
        {
            Debug.Log("invalid format");
        }
    }

    public void disableSkill(bool value)
    {
        GameManagerV2.instance.disableSkill(selectedSkill.type, value);
    }

    #endregion
    #region Private Functions
    private void UpdateSkillSlots()
    {
        for (int i = 0; i < GameManagerV2.instance.GetSkillLevels().Length; i++)
        {
            if (GameManagerV2.instance.GetSkillLevelAt(i) > 0)
            {
                skillUnlocked.Invoke((skillTypes)i);
                unlockedSkills.Add((skillTypes)i);
                if(GameManagerV2.instance.GetSkillLevelAt(i) >= 3)
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
            var level = GameManagerV2.instance.GetSkillLevelAt((int)selectedSkill.type); //show the current level
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

        disableToggle.isOn = GameManagerV2.instance.GetSkillState(selectedSkill.type);
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

    private void Zoom(float amount)
    {
        float currentScale = skillParent.transform.localScale.x;
        currentScale += amount * zoomFactor;
        currentScale = Mathf.Clamp(currentScale, 1f, 4.7f);
        targetZoom = Vector3.one*currentScale;
        GetPositionTarget();
    }

    private void GetPositionTarget()
    {
        targetPos = skillTransform.anchoredPosition - mousePos.normalized * moveSpeed;
        maxPos.x = Mathf.Lerp(95f, 980f, (targetZoom.x - 1f) / 3.7f);
        maxPos.y = Mathf.Lerp(0f, 634f, (targetZoom.y - 1f) / 3.7f);
        minPos.x = Mathf.Lerp(95f, -975f, (targetZoom.x - 1f) / 3.7f);
        minPos.y = Mathf.Lerp(0f, -640f, (targetZoom.y - 1f) / 3.7f);
        targetPos.x = Mathf.Clamp(targetPos.x, minPos.x, maxPos.x);
        targetPos.y = Mathf.Clamp(targetPos.y, minPos.y, maxPos.y);
        //Debug.Log("target position = " + targetPos);
    }
    #endregion
}
