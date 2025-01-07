using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashProjectile : MonoBehaviour
{
    public Vector3 direction;
    public PlayerController manager;

    private void OnTriggerEnter2D(Collider2D other) {
        //Debug.Log(other.transform.name);
        if(other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("EnemyShield"))
        {
            manager.DisableTransform(transform);
        }
    }

}
