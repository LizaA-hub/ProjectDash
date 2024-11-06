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
    Transform orb;

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
            //Debug.Log("enemy touched trail");
        }
        else if(other.gameObject.CompareTag("ClosedShape")){
            //Debug.Log("enemy in a shape");
            TakeDamage(GameManager.playerStrength*5f); //will have its own variable
        }
        else if(other.gameObject.CompareTag("Projectile")){
            //Debug.Log("enemy in a shape");
            TakeDamage(GameManager.projectileDamage);
        }
    }

    #region Private Functions

    private void TakeDamage(float amount)
    {
        if (invincible) return;

        health -= amount;

        GameManager.totalDamages += amount;
        
        if (health <= 0f)
        {
            Die();
        }
        else if(spriteRenderer != null)
        {
            StartCoroutine(DamageFlash()); // Start the flash coroutine
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

        //check if an orb object is available//
        bool instantiateNewOrb = true;
        var orbParent = GameObject.Find("OrbParent");
        for (int i = 0; i < orbParent.transform.childCount; i++)
        {
            Transform child = orbParent.transform.GetChild(i);
            if(!child.gameObject.activeSelf){
                orb = child;
                orb.gameObject.SetActive(true);
                instantiateNewOrb = false;
                break;
            }
        }
        //if no orb available instantiate a new one//
        if(instantiateNewOrb){
            orb = Instantiate(orbPrefab);
            orb.SetParent(orbParent.transform, false);
        }
        //set variables
        orb.gameObject.GetComponent<XPOrb>().XPAmount = experience;
        orb.position = transform.position;

        /*//remove self form enemySpawner//
        var spawner = transform.parent.gameObject.GetComponent<EnemySpawner>();
        spawner.RemoveEnemy(transform);

        Destroy(gameObject);*/
        GameManager.enemyKilled += 1;
        spriteRenderer.color = originalColor;
        gameObject.SetActive(false);
    }

    #endregion

}
