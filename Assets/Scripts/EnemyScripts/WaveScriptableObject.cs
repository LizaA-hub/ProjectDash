using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "ScriptableObjects/Wave")]
public class WaveScriptableObject : ScriptableObject
{
    [SerializeField]
    public EnemyDataManager.EnemyGroup[] enemyGroups;
}
