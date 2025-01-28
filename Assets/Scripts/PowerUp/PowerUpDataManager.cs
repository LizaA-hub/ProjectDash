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
        
        public float trailDamage,trailDuration, projectileDamage, waveDamage, swordDamage, bombDamage, xpMultiplier, waveRadius, dashCooldown, bombRadius, strengthBonus;
        public bool haveProjectile, haveWave, haveSword, haveBomb;
        public int projectileNb, shieldLevel;

        public UpgradableDatas(int p)
        {
            //per run variables//
            projectileDamage = GameManagerV2.instance.initialStats.projectileDamage;
            waveDamage = GameManagerV2.instance.initialStats.waveDamage;
            swordDamage = GameManagerV2.instance.initialStats.swordDamage;
            bombDamage = GameManagerV2.instance.initialStats.bombDamage;
            waveRadius = GameManagerV2.instance.initialStats.waveRadius;
            bombRadius = GameManagerV2.instance.initialStats.bombRadius;
            strengthBonus = 0f;

            haveProjectile = haveWave = haveSword = haveBomb = false;

            projectileNb = shieldLevel = 0;

            //skill dependent variables//
            trailDamage = GameManagerV2.instance.skills.trailDamage;
            trailDuration = GameManagerV2.instance.skills.trailDuration;
            xpMultiplier = GameManagerV2.instance.skills.xpMultiplier;
            dashCooldown = GameManagerV2.instance.skills.dashCooldown;


        }
    }
}
