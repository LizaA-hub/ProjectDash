using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetEnemyController : EnemyController
{
    public override void TakeDamage(float amount, bool exception = false)
    {
        if (invincible && !exception) return;

        var finalAmount = amount * (1 + PowerUpManager.upgradableDatas.strengthBonus);
        if (takeExtraDamages)
        {
            finalAmount *= (1 + GameManager.skillVariables.supportStrength);
        }

        if (debug)
            Debug.Log(transform.name + " takes " + finalAmount + " damages");

        StartCoroutine(DamageFlash()); // Start the flash coroutine

        if (!exception)
        {
            invincible = true;
        }
    }
}
