using System.Collections;
using UnityEngine;

public static class PowerUpDataManager
{
    public enum PowerUpType{Trail,XP,Strength,UnlockProjectile,ProjectileDamage,ProjectileCooldown,None};
    [System.Serializable]
    public struct PowerUpData{
        public PowerUpType type;
        public string name,description;
        public int level;
        public Sprite icon;
    }
}
