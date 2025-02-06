using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "ScriptableObjects/Wave")]
public class WaveScriptableObject : ScriptableObject
{
    public bool boss = false;
    [SerializeField]
    public EnemyDataManager.EnemyGroup[] enemyGroups;
}
