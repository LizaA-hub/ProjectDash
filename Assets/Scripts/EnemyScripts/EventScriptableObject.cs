using UnityEngine;

[CreateAssetMenu(fileName = "Event", menuName = "ScriptableObjects/Event")]
public class EventScriptableObject : ScriptableObject
{
    public EnemyDataManager.EventType type;

    public EnemyScriptableObject Enemy;

    [Header("This event can't appeare if the game "), Header("hasn't run for at least the activeAfter duration.")]
    [Min(0f)]
    public float activeAfter;

    [Min(0f)]
    public float initialDelay;

    [Range(0f,1f)]
    public float probability;

    [Min(1f)]
    public float duration;

    [Header("if spawn delay > duration :"), Header("will spawn just once")] 
    [Min(0f)]
    public float spawnDelay;

    [Header("Every spawnDelay a number between"),Header("x and y of enemies will be spawned.")]
    public Vector2 spawnRange;

    [Header("Distance from the player."),Header("Make it bigger than 20 to spawn off screen."),Header("Will be the radius for the circle events.")]
    [Min(1f)]
    public float spawnDistance;

    [Header("Define the size of the zone where"),Header("the enemy will be spawn. Allow to create"),Header("an ellipse for the circle event.")]
    public Vector2 scale;
}
