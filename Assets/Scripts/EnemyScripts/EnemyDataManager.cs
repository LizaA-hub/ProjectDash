using UnityEngine;
using System.Collections;

public static class EnemyDataManager
{
    public struct EnemyData{
        public EnemyType type;
        public Transform prefab;
        public float maxHealth;
        public float strength;
        public float speed;
        public float experience;
        public float spawnDelay;

        public EnemyData(EnemyScriptableObject scriptableObject)
        {
            type = scriptableObject.type;
            prefab = scriptableObject.prefab;
            maxHealth = scriptableObject.maxHealth;
            strength = scriptableObject.strength;
            speed = scriptableObject.speed;
            experience = scriptableObject.experience;
            spawnDelay = scriptableObject.spawnDelay;
        }
    }
}
