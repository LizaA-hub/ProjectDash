
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObjects/Player")]
public class PlayerStatsScriptableObject : ScriptableObject
{
    //General Stats//
    public float maxHealth = 10f, dashSpeed = 10f, xpGoal = 10f, xpGoalGrowFactor = 1.3f ;
    //dash attacks//
    public float dashCooldown = 2f, trailDamage = 1f, trailDuration = 1f, projectileDamage = 2f, waveDamage = 2f,
        swordDamage = 4f, bombDamage = 5f, waveRadius = 5f, bombRadius = 10f;
    //triangle attacks//
    public float triangleDamage = 2f, triangleGravityduration = 1f, triangleDOT= 1f, triangleDOTInterval = 1f;
    //square attack//
    public float squareDamage = 2f, squareFlameDamage = 1f,squareFlameDamageInterval = 1f, squareHeal = 1f;
    //pentagon attacks//
    public float pentagonDamage = 2f, pentagonBladesDamage = 4f, pentagonImplosionBaseDamage = 5f,
        pentagonImplosionCriticalDamage = 10f,pentagonDrain = 1f, pentagonDrainChance = 0.3f, pentagonBombDamage = 6f;
    //hexagon attacks//
    public float hexagonDamage = 3f, hexagonMeteorDamage = 6f, hexagonLightningDamage = 7f; 
}
