using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public static class GameManager
{
    public static UnityEvent<float> XPChange = new UnityEvent<float>(), healthChange = new UnityEvent<float>();
    public static UnityEvent<int> levelChange = new UnityEvent<int>();
    public static UnityEvent gameOver = new UnityEvent();
    private static float experience = 0f, growFactor = 1.3f, health = 10f;
    public static float xpToNextLevel = 100f, playerStrength = 1f, maxHealth = 10f, totalDamages = 0f, gameDuration = 0f;
    private static int level= 1;
    public static int enemyKilled = 0;
    private static bool inPlayGround = false;

    public struct persistantStats{//a separate struct for the stats that can be upgraded by the player between run
        public float defaultHealth;
        public float defaultStrength;
        public float skillPoint;

        public persistantStats(float h, float s){
            defaultHealth = h;
            defaultStrength = s;
            //check if there's saved data for skill points//
            if(PlayerPrefs.HasKey("skillPoint")){
                skillPoint = PlayerPrefs.GetFloat("skillPoint");
            }
            else{
                skillPoint = 0f;
            }
        }
    }

    static persistantStats currentStats;

    static GameManager(){
        currentStats = new persistantStats(10f,1f);
        InitializeVariables();
    }

    #region Public Functions

    public static void ModifyExperience(float amount){
        experience += amount; 
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
        xpToNextLevel = 100*Mathf.Pow(level,growFactor);
        levelChange.Invoke(level);
    }

    public static int GetLevel(){
        return level;
    }

    public static void ModifyHealth(float amount){
        var newHealth = Mathf.Clamp(health + amount,0f,maxHealth);
        if(health != newHealth){
            health = newHealth;
            healthChange.Invoke(health);
        }
        
        if (health == 0f){
            gameOver.Invoke();
            OnGameOver();
        }
        
    }

    public static void ModifyMaxHealth(float value){
        maxHealth = value;
        //set health to max health?
    }

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
        currentStats.skillPoint += amount;
        PlayerPrefs.SetFloat("skillPoint",currentStats.skillPoint);
    }

    public static float GetSkillPoint(){
        return currentStats.skillPoint;
    }

    public static void ResetSkillPoints(){
        ModifySkillPoint(-currentStats.skillPoint);
    }

    #endregion

    #region Private Functions

    private static void InitializeVariables(){
        experience = 0f;
        maxHealth = currentStats.defaultHealth;
        health = maxHealth;
        xpToNextLevel = 100f;
        playerStrength = currentStats.defaultStrength;
        totalDamages = 0f;
        gameDuration = 0f;
        level = 1;
        enemyKilled = 0;
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

    #endregion
}
