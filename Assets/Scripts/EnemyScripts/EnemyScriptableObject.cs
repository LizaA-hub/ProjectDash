using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    public EnemyType type;
    public Transform prefab;
    public float maxHealth;
    public float strength;
    public float speed;
    public float experience;
    public float spawnDelay;
}
