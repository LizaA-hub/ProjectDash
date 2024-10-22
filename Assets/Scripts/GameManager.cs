using UnityEngine;
using UnityEngine.Events;

public static class GameManager
{
    public static UnityEvent<float> XPChange = new UnityEvent<float>();
    public static UnityEvent<int> levelChange = new UnityEvent<int>();
    private static float experience = 0f;
    public static float xpToNextLevel = 100f;
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
        //TO DO : increase xpToNextLevel
        levelChange.Invoke(level);
    }

    public static int GetLevel(){
        return level;
    }

}
