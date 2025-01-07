using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    private ShieldEnemyController parentController;
    private string[] shieldableAttacks = { "PlayerTrail", "Projectile" , "ShockWave" , "PentagonBlade" , "PentagonWave" };
    // Start is called before the first frame update
    void Start()
    {
        parentController = transform.GetComponentInParent<ShieldEnemyController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        bool canProtect = false;
        string attackTag = "";
        foreach (var tag in shieldableAttacks)
        {
            if (collision.gameObject.CompareTag(tag))
            {
                //Debug.Log("shield in contact with" + collision.gameObject.name);
                canProtect = true;
                attackTag = tag;
                break;
            }
        }
        if (canProtect && parentController != null) {
            parentController.shield = true;
            parentController.attackTag = attackTag;
        }
    }

}
