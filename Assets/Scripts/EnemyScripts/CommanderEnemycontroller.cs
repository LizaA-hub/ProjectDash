using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderEnemycontroller : EnemyController
{
    public float timer=10f, distToPlayer = 6f;

    public Vector3 GetTarget(Vector3 playerPos)
    {
        Vector3 diff = transform.position - playerPos;
        diff = diff.normalized * distToPlayer;
        if (Vector3.Distance(playerPos, transform.position) <= distToPlayer)
        {
            return transform.position + diff;
        }
        else
        {
            return playerPos - diff;
        }
    }
}
