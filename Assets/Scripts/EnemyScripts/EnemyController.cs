using UnityEngine;
using System.Collections;
public class EnemyController : MonoBehaviour
{
    public EnemyDataManager.EnemyType type;
    [SerializeField]
    Transform orbPrefab;
    [HideInInspector]
    public float health, strength, speed, experience, cooldown = 1f;
    bool invincible = false;

    //added for damage animation
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Awake()
    {
        // Get the SpriteRenderer component and store the original color
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

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

    private void TakeDamage(float amount)
    {
        if (invincible) return;

        health -= amount;
        if (spriteRenderer != null)
        {
            StartCoroutine(DamageFlash()); // Start the flash coroutine
        }

        GameManager.totalDamages += amount;
        
        if (health <= 0f)
        {
            Die();
        }
    }
    
    //Flash animation when the enemy takes damage
    private IEnumerator DamageFlash()
    {
        // Flash white
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f); // Adjust the flash duration as needed

        // Return to the original color
        spriteRenderer.color = originalColor;
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
