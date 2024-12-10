using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public static class GameManager
{
    public static UnityEvent<float> XPChange = new UnityEvent<float>(), healthChange = new UnityEvent<float>();
    public static UnityEvent<int> levelChange = new UnityEvent<int>(), trailIncrease = new UnityEvent<int>();
    public static UnityEvent gameOver = new UnityEvent(), magnetIncrease = new UnityEvent();
    public static float xpToNextLevel = 10f, playerStrength = 1f, maxHealth = 10f, totalDamages = 0f, gameDuration = 0f, xpMultiplier = 0f, projectileDamage = 2f,
                        shockWaveMaxRadius = 5f, shockWaveStrength = 2f, dashCooldown = 2f, swordStrength = 4f, bombStrength = 5f, bombRadius = 10f;
    public static int enemyKilled = 0, projectileNb = 0, dashShieldLevel = 0;
    public static bool haveProjectile = false, haveShockWave = false, haveSword = false, haveBomb = false;
    

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

    static GameManager(){
        string filePath = Application.persistentDataPath + "/playerData.json";
        if (System.IO.File.Exists(filePath))
        {
            string playerData = System.IO.File.ReadAllText(filePath);
            currentDatas = JsonUtility.FromJson<savedDatas>(playerData);
        }
        else
        {
            int count = Enum.GetNames(typeof(skillTypes)).Length;
            currentDatas = new savedDatas(count);
        }
        InitializeVariables();
    }

    #region Public Functions

    public static void ModifyExperience(float amount){
        float m = amount*xpMultiplier;
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

    public static float GetHealth() => health;

    public static void ModifyMaxHealth(float value){
        maxHealth = value;
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
        currentDatas.skillPoint += amount;
        Save();
    }

    public static float GetSkillPoint(){
        return currentDatas.skillPoint;
    }

    public static void ResetSkillPoints(){
        ModifySkillPoint(-currentDatas.skillPoint);
    }

    public static void Upgrade(PowerUpType type, int level){
        switch (type)
        {
            case PowerUpType.Trail :
                trailIncrease.Invoke(level);
                break;
            case PowerUpType.Strength :
                playerStrength += playerStrength*0.1f;
                projectileDamage += projectileDamage*0.1f;
                shockWaveStrength += shockWaveStrength*0.1f;
                swordStrength += swordStrength * 0.1f;
                bombStrength += bombStrength * 0.1f;
                break;
            case PowerUpType.XP :
                xpMultiplier = 0.1f*level;
                break;
            case PowerUpType.Projectile :
                if(level == 1){
                    haveProjectile = true;
                }
                projectileNb = level;
                
                break;
            case PowerUpType.Wave :
                if(level == 1){
                    haveShockWave = true;
                }
                else{
                    shockWaveMaxRadius += shockWaveMaxRadius*0.1f;
                    shockWaveStrength += shockWaveStrength*0.1f;
                }
                break;
            case PowerUpType.Shield:
                dashShieldLevel += 1;
                break;
            case PowerUpType.Magnet:
                magnetIncrease.Invoke();
                break;
            case PowerUpType.Cooldown:
                dashCooldown -= dashCooldown * 0.1f;
                break;
            case PowerUpType.Sword:
                if (level == 1)
                {
                    haveSword = true;
                }
                else
                {
                    swordStrength += swordStrength * 0.1f;
                }
                break;
            case PowerUpType.Bomb:
                if (level == 1)
                {
                    haveBomb = true;
                }
                else
                {
                    bombStrength += bombStrength * 0.1f;
                    bombRadius += bombRadius * 0.1f;
                }
                break;
            default:
            break;
        }
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
        xpMultiplier = 0f;
        //health variables//
        maxHealth = 10f;
        health = maxHealth;
        //damage variables//
        playerStrength = 1f;
        totalDamages = 0f;
        enemyKilled = 0;
        projectileDamage = 2f;
        projectileNb = 0;
        gameDuration = 0f;
        shockWaveStrength = 2f;
        shockWaveMaxRadius = 5f;
        dashShieldLevel = 0;
        bombStrength = 5f;
        bombRadius = 10f;
        //time variables//
        Time.timeScale = 1f;
        dashCooldown = 2f;
        //bool variables//
        haveProjectile = false;
        haveShockWave = false;
        haveBomb = false;
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
