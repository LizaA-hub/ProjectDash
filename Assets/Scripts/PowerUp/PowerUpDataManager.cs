using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public static class PowerUpDataManager
{
    public struct PowerUpData{
        public PowerUpType type;
        public string name,description;
        public bool isDashAttack;
        public int level, levelLimit;
        public Sprite icon;

        public PowerUpData(PowerUpScriptableObject scriptableObject)
        {
            type = scriptableObject.type;
            name = scriptableObject.name;
            description = scriptableObject.description;
            isDashAttack = scriptableObject.isDashAttack;
            level = 1;
            levelLimit = scriptableObject.levelLimit;
            icon = scriptableObject.icon;
        }
    }

    public struct UpgradableDatas
    {
        
        public float trailDamage, projectileDamage, waveDamage, swordDamage, bombDamage, xpMultiplier, waveRadius, dashCooldown, bombRadius, strengthBonus;
        public bool haveProjectile, haveWave, haveSword, haveBomb;
        public int projectileNb, shieldLevel;

        public UpgradableDatas(int p)
        {
            //per run variables//
            projectileDamage = 2f;
            waveDamage = 2f;
            swordDamage = 4f;
            bombDamage = 5f;
            waveRadius = 5f;
            bombRadius = 10f;
            strengthBonus = 0f;

            haveProjectile = haveWave = haveSword = haveBomb = false;

            projectileNb = shieldLevel = 0;

            //skill dependent variables//
            trailDamage = 1f + GameManager.skillVariables.trailDamage;
            xpMultiplier = GameManager.skillVariables.xpMultiplier;
            dashCooldown = 2f*(1-GameManager.skillVariables.dashCooldown);


        }
    }
}
