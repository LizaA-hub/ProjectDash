using UnityEngine;
using static PowerUpDataManager;

[CreateAssetMenu(fileName = "PowerUp", menuName = "ScriptableObjects/PowerUp")]
public class PowerUpScriptableObject : ScriptableObject
{
    public PowerUpType type;
    public new string name;
    public string description;
    public bool isDashAttack;
    public int levelLimit;
    public Sprite icon;
}
