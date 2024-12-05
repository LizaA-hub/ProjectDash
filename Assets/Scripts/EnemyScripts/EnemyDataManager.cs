using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public static class EnemyDataManager
{
    //variables for the array of datas in the enemy spawner//
    [System.Serializable]
    public struct EnemyData{
        public EnemyType type;
        public Transform prefab;
        public float maxHealth;
        public float strength;
        public float speed;
        public float experience;

    }

    //variables for the wave scriptable object//
    public enum propertyType { Health,Speed,XP,Strength,None};
    [System.Serializable]
    public struct EnemyGroup
    {
        public EnemyScriptableObject enemy;
        public int maxNumber;
        public float spawnDelay;
        public propertyType property;
        public float propertyMultiplier;
    }
}
