using System.Collections;
using UnityEngine;

public static class PowerUpDataManager
{
    public struct PowerUpData{
        public PowerUpType type;
        public string name,description;
        public bool isDashAttack;
        public int level, levelLimit;
        public Sprite icon;

        public PowerUpData(PowerUpScriptableObject scriptableObject)
        {
            type = scriptableObject.type;
            name = scriptableObject.name;
            description = scriptableObject.description;
            isDashAttack = scriptableObject.isDashAttack;
            level = 1;
            levelLimit = scriptableObject.levelLimit;
            icon = scriptableObject.icon;
        }
    }
}
