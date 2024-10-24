using UnityEngine;
using UnityEngine.Events;

public static class GameManager
{
    public static UnityEvent<float> XPChange = new UnityEvent<float>(), healthChange = new UnityEvent<float>();
    public static UnityEvent<int> levelChange = new UnityEvent<int>();
    private static float experience = 0f, growFactor = 1.3f, health = 10f;
    public static float xpToNextLevel = 100f, playerStrength = 1f, maxHealth = 10f;
    private static int level= 1;
    

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
        //Debug.Log("new xp goal: " + xpToNextLevel);
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
        
        //add game over if health <=0
        
    }

    public static void ModifyMaxHealth(float value){
        maxHealth = value;
        //set health to max health?
    }

}
