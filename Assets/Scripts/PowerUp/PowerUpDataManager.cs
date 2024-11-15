using System.Collections;
using UnityEngine;

public static class PowerUpDataManager
{
    public enum PowerUpType{Trail,XP,Strength,Projectile,None};
    [System.Serializable]
    public struct PowerUpData{
        public PowerUpType type;
        public string name,description;
        public int level, levelLimit;
        public Sprite icon;
    }
}
