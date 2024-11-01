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

    public struct persistantStats{//a separate struct for the stats that can be upgraded by the player between run
        public float defaultHealth;
        public float defaultStrength;

        public persistantStats(float h, float s){
            defaultHealth = h;
            defaultStrength = s;
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
        }
        
    }

    public static void ModifyMaxHealth(float value){
        maxHealth = value;
        //set health to max health?
    }

    public static void RestartGame(){
        //Reset stats//
        InitializeVariables();

        //reload scene//
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
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
    }

    #endregion
}
