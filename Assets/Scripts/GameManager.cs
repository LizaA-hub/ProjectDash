using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public static class GameManager
{
    public static UnityEvent<float> XPChange = new UnityEvent<float>(), healthChange = new UnityEvent<float>();
    public static UnityEvent<int> levelChange = new UnityEvent<int>();
    public static UnityEvent gameOver = new UnityEvent();
    public static float xpToNextLevel = 10f, totalDamages = 0f, gameDuration = 0f;
    public static int enemyKilled = 0;
    

    private static float experience = 0f, growFactor = 1.3f, health = 10f;
    private static int level= 1;
    private static bool inPlayGround = false;

    public struct savedDatas{//datas that need to be saved 
        public int[] skillLevels;
        public float skillPoint;

        public savedDatas(int length) {
            skillLevels = new int[length];
            skillPoint = 0;
        }

    }

    static savedDatas currentDatas;

    public struct SkillVariables //WIP//
    {
        //general skills//
        public float trailDamage, xpMultiplier, dashCooldown, maxHealth, trailDuration, dashSpeed;
        //triangle skills//
        public float triangleDamage, triangleGravityDuration, DOT, stunDuration, supportStrength;
        public bool triangleGravity;
        //square skills//
        public float squareDamage, squareSlow, squareFlame, squareTrap, squareHeal;
        //pentagon skills//
        public float pentagonDamage, bladeDamage, pentagonCriticalChance, pentagonHeal, pentagonBombDamage;
        public bool pentagonBlade, pentagonBomb;
        //hexagon skills//
        public float hexagonDamage, hexagonMeteor, meteorDamage;


    }
    public static SkillVariables skillVariables;

    static GameManager(){
        //fetch datas from saved file//
        string filePath = Application.persistentDataPath + "/playerData.json";
        if (System.IO.File.Exists(filePath))
        {
            string playerData = System.IO.File.ReadAllText(filePath);
            currentDatas = JsonUtility.FromJson<savedDatas>(playerData);
        }
        else // if no saved files create new datas
        {
            int count = Enum.GetNames(typeof(skillTypes)).Length;
            currentDatas = new savedDatas(count);
        }

        //set skills datas based on the saved skill levels//
        skillVariables = new SkillVariables();
        UpdateAllSkills();

        //set other game variables//
        InitializeVariables();
    }

    #region Public Functions

    public static void ModifyExperience(float amount){
        float m = amount*skillVariables.xpMultiplier;
        experience += amount + m; 
        if (experience >= xpToNextLevel){
            experience -= xpToNextLevel;
            ModifyLevel();
        }
        XPChange.Invoke(experience);
    }

    public static float GetExperience(){
        return experience;
    }

    public static void ModifyLevel(){
        level += 1; 
        xpToNextLevel = 10*Mathf.Pow(level,growFactor);
        levelChange.Invoke(level);
    }

    public static int GetLevel() => level;
    
    public static void ModifyHealth(float amount){
        var newHealth = Mathf.Clamp(health + amount,0f,skillVariables.maxHealth);
        if(health != newHealth){
            health = newHealth;
            healthChange.Invoke(health);
        }
        
        if (health == 0f){
            gameOver.Invoke();
            OnGameOver();
        }
        
    }

    public static float GetHealth() => health;

    public static void StartGame(){
        //Reset stats//
        InitializeVariables();

        //load scene//
        if(inPlayGround){
            SceneManager.LoadScene("ProgrammationPlayground1");
        }
        else{
            SceneManager.LoadScene("MainLevel");}
    }

    public static void LoadMenu(){
        SceneManager.LoadScene("MainMenu");
    }

    public static void ModifySkillPoint(float amount){
        currentDatas.skillPoint += amount;
        Save();
    }

    public static float GetSkillPoint(){
        return currentDatas.skillPoint;
    }

    public static void ResetSkillPoints(){
        ModifySkillPoint(-currentDatas.skillPoint);
    }

    public static int SkillEnhance(skillTypes type)
    {
        int pos = (int)type;
        int level = currentDatas.skillLevels[pos];
        level++;
        SetSkillLevel(pos, level);
        SetSkillVariable(type, level);
        return level;
    }

    public static void Save()
    {
        string filePath = Application.persistentDataPath + "/playerData.json";
        string playerData = JsonUtility.ToJson(currentDatas);
        System.IO.File.WriteAllText(filePath, playerData);
        //Debug.Log("datas saved at " + filePath);
    }
    
    public static int[] GetSkillLevels()
    {
        return currentDatas.skillLevels;
    }

    public static int GetSkillLevelAt(int position)
    {
        return currentDatas.skillLevels[position];
    }

    public static void UpdateAllSkills()
    {
        for (int i = 0; i < currentDatas.skillLevels.Length; i++)
        {
            skillTypes type = (skillTypes)i;
            SetSkillVariable(type, currentDatas.skillLevels[i]);
        }
    }
    public static void SetSkillLevel(int position, int value)
    {
        currentDatas.skillLevels[position] = value;
        Save();
    }
    #endregion

    #region Private Functions

    private static void InitializeVariables(){
        //XP variables//
        experience = 0f;
        xpToNextLevel = 10f;
        level = 1;
        //health variables//
        health = skillVariables.maxHealth;
        //game variables//
        totalDamages = 0f;
        enemyKilled = 0;
        gameDuration = 0f;
        //time variables//
        Time.timeScale = 1f;

        //check if playing in playground//
        var scene = SceneManager.GetActiveScene();
        if(scene.name == "ProgrammationPlayground1"){
            inPlayGround = true;
        }
        

    }

    private static void OnGameOver(){
        float newSkillPoint = (float)enemyKilled + totalDamages*gameDuration;
        ModifySkillPoint(newSkillPoint);
    }

    private static void SetSkillVariable(skillTypes type, int level)
    {
        switch (type)
        {
            case skillTypes.General_HP:
                skillVariables.maxHealth = 10f * (1 + 0.1f * level);
                break;
            case skillTypes.Dash_Cooldown:
                skillVariables.dashCooldown = 0.1f * level;
                break;
            case skillTypes.Trail_Damage:
                skillVariables.trailDamage = 0.05f * level;
                break;
            case skillTypes.Trail_Duration:
                skillVariables.trailDuration = 0.5f * level;
                break;
            case skillTypes.Dash_Speed:
                skillVariables.dashSpeed = 0.1f * level;
                break;
            case skillTypes.General_XP:
                skillVariables.xpMultiplier = 0.1f * level;
                break;
            case skillTypes.Triangle_Damage:
                skillVariables.triangleDamage = 2f*(level*0.1f+1);
                break;
            case skillTypes.Triangle_Gravity:
                if (level == 0)
                {
                    skillVariables.triangleGravity = false;
                }
                else
                {
                    skillVariables.triangleGravity = true;
                    skillVariables.triangleGravityDuration = 1f + 0.2f * level;
                }

                break;
            case skillTypes.Triangle_DOT:
                skillVariables.DOT = level;
                break;
            case skillTypes.Triangle_Stun:
                if(level > 0)
                {
                    skillVariables.stunDuration = 1f + 0.5f * (level - 1f);
                }
                else
                {
                    skillVariables.stunDuration = 0f;
                }
                
                break;
            case skillTypes.Triangle_Support:
                if (level > 0)
                {
                    skillVariables.supportStrength = 0.5f + 0.1f * (level - 1f);
                }
                else
                {
                    skillVariables.supportStrength = 0f;
                }
                break;
            case skillTypes.Triangle_6:
                //To be implemented
                break;
            case skillTypes.Square_Damage:
                skillVariables.squareDamage = 2f * (level * 0.1f + 1f);
                break;
            case skillTypes.Square_Slow:
                if(level > 0)
                {
                    skillVariables.squareSlow = 0.2f + 0.1f * level;
                }
                else
                {
                    skillVariables.squareSlow = 0f;
                }
                
                break;
            case skillTypes.Square_Flame:
                skillVariables.squareFlame = level;
                break;
            case skillTypes.Square_Trap:
                skillVariables.squareTrap = 0.05f * level;
                break;
            case skillTypes.Square_Heal:
                skillVariables.squareHeal = level;
                break;
            case skillTypes.Square_6:
                //to be implemented
                break;
            case skillTypes.Pentagon_Damage:
                skillVariables.pentagonDamage = 2f * (level * 0.1f + 1f);
                break;
            case skillTypes.Pentagon_Blade:
                if(level == 0)
                {
                    skillVariables.pentagonBlade = false;
                }
                else
                {
                    skillVariables.pentagonBlade = true;
                    skillVariables.bladeDamage = (1f + 0.1f* level)*4f;
                }
                
                break;
            case skillTypes.Pentagon_Implosion:
                skillVariables.pentagonCriticalChance = level*0.1f;
                break;
            case skillTypes.Pentagon_Drain:
                skillVariables.pentagonHeal = level;
                break;
            case skillTypes.Pentagon_Bomb:
                if(level == 0)
                {
                    skillVariables.pentagonBomb = false;
                }
                else
                {
                    skillVariables.pentagonBomb = true;
                    skillVariables.pentagonDamage = 5f * (1f + 0.1f * level);
                }
                break;
            case skillTypes.Pentagon_6:
                break;
            case skillTypes.Hexagon_Damage:
                skillVariables.hexagonDamage = 2f * (level * 0.1f + 1f);
                break;
            case skillTypes.Hexagon_Meteor:
                skillVariables.hexagonMeteor = level;
                skillVariables.meteorDamage = 5f * (1f + 0.1f * level);
                break;
            case skillTypes.Hexagon_Lightning:
                break;
            case skillTypes.Hexagon_Area:
                break;
            case skillTypes.Hexagon_Slow:
                break;
            case skillTypes.Hexagon_6:
                break;
            case skillTypes.Pentagram_Duration:
                break;
            case skillTypes.Pentagram_Damage:
                break;
            case skillTypes.Pentagram_Critical:
                break;
            case skillTypes.Pentagram_Star:
                break;
            case skillTypes.Pentagram_Octagon:
                break;
            case skillTypes.Pentagram_6:
                break;
            case skillTypes.Branch7_1:
                break;
            case skillTypes.Branch7_2:
                break;
            case skillTypes.Branch7_3:
                break;
            case skillTypes.Branch7_4:
                break;
            case skillTypes.Branch7_5:
                break;
            case skillTypes.Branch7_6:
                break;
            default:
                break;
        }
    }

    #endregion
}
