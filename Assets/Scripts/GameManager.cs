using System;
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
        public float trailDamage, xpMultiplier, dashCooldown, maxHealth, trailDuration;

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
        for (int i = 0; i < currentDatas.skillLevels.Length; i++)
        {
            skillTypes type = (skillTypes)i;
            SetSkillVariable(type, currentDatas.skillLevels[i]);
        }

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

    /*public static void ModifyMaxHealth(float value){
        maxHealth = value;
    }*/

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
                break;
            case skillTypes.General_XP:
                skillVariables.xpMultiplier = 0.1f * level;
                break;
            case skillTypes.Triangle_Damage:
                break;
            case skillTypes.Triangle_Gravity:
                break;
            case skillTypes.Triangle_DOT:
                break;
            case skillTypes.Triangle_Stun:
                break;
            case skillTypes.Triangle_Support:
                break;
            case skillTypes.Triangle_6:
                break;
            case skillTypes.Square_Damage:
                break;
            case skillTypes.Square_Slow:
                break;
            case skillTypes.Square_Flame:
                break;
            case skillTypes.Square_Trap:
                break;
            case skillTypes.Square_Heal:
                break;
            case skillTypes.Square_6:
                break;
            case skillTypes.Pentagon_Damage:
                break;
            case skillTypes.Pentagon_Blade:
                break;
            case skillTypes.Pentagon_Implosion:
                break;
            case skillTypes.Pentagon_Drain:
                break;
            case skillTypes.Pentagon_Bomb:
                break;
            case skillTypes.Pentagon_6:
                break;
            case skillTypes.Hexagon_Damage:
                break;
            case skillTypes.Hexagon_Meteor:
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
