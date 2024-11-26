using UnityEngine;
using System.Collections;

public static class EnemyDataManager
{
    public enum EnemyType {Basic,Tanky,Fast,Charging,Projectile,None}
    [System.Serializable]
    public struct EnemyData{
        public EnemyType type;
        public Transform prefab;
        public float maxHealth;
        public float strength;
        public float speed;
        public float experience;
        public float spawnDelay;
    }
}
