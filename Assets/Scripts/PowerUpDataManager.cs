using System.Collections;
using UnityEngine;

public static class PowerUpDataManager
{
    public enum PowerUpType{Health,Speed,Strength};
    [System.Serializable]
    public struct PowerUpData{
        public PowerUpType type;
        public string name,description;
        public int level;
        public Sprite icon;
    }
}
