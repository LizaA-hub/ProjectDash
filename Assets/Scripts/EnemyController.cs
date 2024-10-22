using UnityEngine;
public class EnemyController : MonoBehaviour
{
    public EnemyDataManager.EnemyType type;
    [SerializeField]
    Transform orbPrefab;
    [HideInInspector]
    public float health, strength, speed, experience, timer = 5f;

    //WILL BE REMOVED - DEBUG SYSTEM//
    private void Update() {
        timer -= Time.deltaTime;
        if (timer <= 0f){
            Die();
        }
    }

    #region Private Functions

    private void TakeDamage(float amount){
        health =- amount;
        if (health <= 0f){
            Die();
        }
    }

    private void Die(){
        //drop orb//
        var orb = Instantiate(orbPrefab);
        orb.gameObject.GetComponent<XPOrb>().XPAmount = experience;
        orb.position = transform.position;

        //remove self form enemySpawner//
        var spawner = transform.parent.gameObject.GetComponent<EnemySpawner>();
        spawner.RemoveEnemy(transform);

        Destroy(gameObject);
    }

    #endregion

}
