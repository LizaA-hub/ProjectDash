using System.Collections;
using UnityEngine;

public static class PowerUpDataManager
{
    public enum PowerUpType{Trail,XP,Strength,Projectile,Wave,Shield,None};
    [System.Serializable]
    public struct PowerUpData{
        public PowerUpType type;
        public string name,description;
        public bool isDashAttack;
        public int level, levelLimit;
        public Sprite icon;
    }
}
