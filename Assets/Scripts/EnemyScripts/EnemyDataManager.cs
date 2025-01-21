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
        [Header("The number of enemies spawned at the start of the wave")]
        public int initialNumber;
        [Header("one enemy spawned every spawnDelay until maxNumber is reached")]
        public int maxNumber;
        public float spawnDelay;
        public SpecialProperty[] specialProperties;
    }

    [System.Serializable]
    public struct SpecialProperty
    {
        public propertyType property;
        public float propertyMultiplier;
    }

    //variables for the event scriptable object//
    public enum EventType { Circle, Cluster, Maze, None}
    
}
