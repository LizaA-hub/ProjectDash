using UnityEngine;
using System.Collections;
public class EnemyController : MonoBehaviour
{
    public EnemyType type;
    [SerializeField]
    Transform orbPrefab;
    [HideInInspector]
    public float health, strength, speed, experience, cooldown = 1f, DOT_Timer = 0f, stun =0f;
    public Vector3 attractionTarget;
    public bool isAttracked = false;
    bool invincible = false, takeExtraDamages = false; 
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
            TakeDamage(PowerUpManager.upgradableDatas.trailDamage);
            //Debug.Log("enemy touched trail");
        }
        else if(other.gameObject.CompareTag("ClosedShape")){
            ClosedShape closedShape = other.gameObject.GetComponent<ClosedShape>();
            ShapeAttack(closedShape.shape);
            closedShape.AddEnemy(); //for triangle gravity
            //Debug.Log("enemy in shape");
        }
        else if(other.gameObject.CompareTag("Projectile")){
            //Debug.Log("enemy touche by projectile");
            TakeDamage(PowerUpManager.upgradableDatas.projectileDamage);
        }
        else if(other.gameObject.CompareTag("ShockWave")){
            //Debug.Log("enemy colliding with wave");
            TakeDamage(PowerUpManager.upgradableDatas.waveDamage);
        }
        else if (other.gameObject.CompareTag("Bomb"))
        {
            TakeDamage(PowerUpManager.upgradableDatas.bombDamage);
            //Debug.Log("enemy touch bomb");
        }
        else if (other.gameObject.CompareTag("ShapeField"))
        {
            //Debug.Log("enemy is in the triangle flied");
            isAttracked = true;
            attractionTarget = other.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ShapeField"))
        {
            //Debug.Log("enemy leave the triangle flied");
            isAttracked = false;
        }
    }
    #region Public Functions
    public void TakeDamage(float amount, bool exception = false)
    {
        if (invincible && !exception) return;

        var finalAmount = amount * (1 + PowerUpManager.upgradableDatas.strengthBonus);
        if (takeExtraDamages)
        {
            finalAmount *= (1 + GameManager.skillVariables.supportStrength);
        }
        //Debug.Log("enemy takes " + finalAmount + " damages");
        health -= finalAmount;

        GameManager.totalDamages += finalAmount;
        
        if (health <= 0f)
        {
            Die();
        }
        else if(spriteRenderer != null)
        {
            StartCoroutine(DamageFlash()); // Start the flash coroutine
        }

        if (!exception)
        {
            invincible = true;
        }
    }
    #endregion
    #region Virtual Function
    public virtual void Die(){
        StopAllCoroutines();
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

        GameManager.enemyKilled += 1;
        spriteRenderer.color = originalColor;
        isAttracked = false;
        takeExtraDamages = false;
        DOT_Timer = 0f;
        stun = 0f;
        gameObject.SetActive(false);
    }

    #endregion

    #region Private Functions

    //Flash animation when the enemy takes damage
    private IEnumerator DamageFlash()
    {
        // Flash white
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f); // Adjust the flash duration as needed

        // Return to the original color
        spriteRenderer.color = originalColor;
    }

    private void ShapeAttack(GeometricalShape.Shape shape)
    {
        switch (shape) {
            case GeometricalShape.Shape.Triangle:
                TakeDamage(GameManager.skillVariables.triangleDamage);

                if(GameManager.skillVariables.DOT > 0f)
                {
                    //Debug.Log("enemy taking DOT");
                    DOT_Timer += 5f;
                    if(gameObject.activeSelf)
                        StartCoroutine(DamageOverTime());
                }

                stun = GameManager.skillVariables.stunDuration;

                if ((GameManager.skillVariables.supportStrength > 0f) && !takeExtraDamages)
                {
                    takeExtraDamages = true;
                }
                break;
            default:
                TakeDamage(PowerUpManager.upgradableDatas.trailDamage * 5f);
                break;
        }
    }

    private IEnumerator DamageOverTime()
    {
        while(DOT_Timer > 0f)
        {
            TakeDamage(GameManager.skillVariables.DOT, true);
            DOT_Timer -= 1f;
            yield return new WaitForSeconds(1f);
        }
        
    }

    #endregion

}
