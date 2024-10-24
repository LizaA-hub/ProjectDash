using UnityEngine;
public class EnemyController : MonoBehaviour
{
    public EnemyDataManager.EnemyType type;
    [SerializeField]
    Transform orbPrefab;
    [HideInInspector]
    public float health, strength, speed, experience, cooldown = 1f;
    bool invincible = false;

    private void Update() {
        if (invincible){
            cooldown -= Time.deltaTime;
            if(cooldown <= 0f){
                invincible = false;
                cooldown = 1f;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("PlayerTrail")){
            TakeDamage(GameManager.playerStrength);
        }
    }

    #region Private Functions

    private void TakeDamage(float amount){
        if(invincible) return;

        health -= amount;
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
