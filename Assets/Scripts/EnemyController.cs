using UnityEngine;
public class EnemyController : MonoBehaviour
{
    public EnemyDataManager.EnemyType type;
    [HideInInspector]
    public float health, strength, speed, experience;

    #region Private Functions

    private void TakeDamage(float amount){
        health =- amount;
        if (health <= 0f){
            Die();
        }
    }

    private void Die(){
        //drop orb//
        //remove self form enemySpawner//
        Destroy(gameObject);
    }

    #endregion

}
