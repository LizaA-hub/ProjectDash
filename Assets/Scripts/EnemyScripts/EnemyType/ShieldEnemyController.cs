using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ShieldEnemyController : EnemyController
{
    [HideInInspector]
    public bool shield;
    [HideInInspector]
    public string attackTag = "";
    bool isTrigger;

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("enemy in contact with" + collision.gameObject.name);
        if (attackTag != "")
        {
            if (shield && collision.gameObject.CompareTag(attackTag))
            {
                //Debug.Log("protected!");
                return;
            }
        }
        
        base.OnTriggerEnter2D(collision);
    }

    public override void OnTriggerExit2D(Collider2D collision)
    {
        if(attackTag != "")
        {
            if (collision.gameObject.CompareTag(attackTag) && shield)
            {
                shield = false;
            }
        }
        
        base.OnTriggerExit2D(collision);
    }
}
