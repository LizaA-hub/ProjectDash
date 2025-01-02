using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingEnemyControllerTEST : EnemyController
{
    public float chargeTimer = 1f, dashDistance = 3f;
    [HideInInspector]
    public bool isDashing = false,isCharging = false;
    [HideInInspector]
    public Vector3 dashTarget;

    public override void Die(bool disable = true){
        chargeTimer = 1f;
        isDashing = false;
        isCharging = false;
        base.Die();
    }
}
