using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTest : EnemyController
{
    private EnemySpawnerV2 spawner;
    private float rotationSpeed = 100f, chargeCooldown = 2f;
    private bool charging = false, preCharge = false;
    private Vector3 target;
    private Transform player;

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.Find("Player").GetComponent<Transform>();
        }
        if (player == null)
            Debug.Log("boss controller : player not found");

        if(spawner == null)
        {
            spawner = GetComponentInParent<EnemySpawnerV2>();
        }
        if (spawner == null)
            Debug.Log("boss  controller : enemy spawner not found");
    }
    protected override void UpdateMovement(float t)
    {
        chargeCooldown -= t;
        if(chargeCooldown <= 0f)
        {
            if (charging)
            {
                chargeCooldown = 3f;
                charging = false;
                preCharge = false;
                rotationSpeed /= 3;
            }
            else
            {
                chargeCooldown = 2f;
                charging = true;
                target = (player.transform.position - transform.position).normalized;
            }
        }
        else if (chargeCooldown <= 1f && preCharge == false)
        {
            rotationSpeed *= 3;
            preCharge = true;
        }
        transform.Rotate(Vector3.back, t*rotationSpeed);
        if(charging)
            transform.position = Vector3.MoveTowards(transform.position, transform.position+target, speed*t);
    }

    public override void Die(bool disable = true)
    {
        spawner.EndBossFight();
        base.Die(disable);
    }
}
