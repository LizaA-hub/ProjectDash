using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashProjectile : MonoBehaviour
{
    public Vector3 direction;
    public PlayerController manager;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Enemy")){
            manager.DisableProjectile(transform);
        }
    }
}
