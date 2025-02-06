using UnityEngine;
using System.Collections;
public class EnemyController : MonoBehaviour
{
    public EnemyType type;
    [SerializeField]
    Transform orbPrefab, shieldTransform;
    [HideInInspector]
    public float health, strength,  experience, cooldown = 1f, DOT_Timer = 0f, stun =0f, speed;
    [HideInInspector]
    public Vector3 attractionTarget, direction;
    [HideInInspector]
    public bool isAttracked = false;
    public bool debug = false;
    protected bool invincible = false, takeExtraDamages = false, slowed = false, burning = false, trapped = false, inPentagon = false, shielded = false; 
    Transform orb;
    private float initialSpeed;

    //added for damage animation
    protected SpriteRenderer spriteRenderer;
    private Color originalColor;

    #region Unity Functions
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
    public virtual void OnTriggerEnter2D(Collider2D other) {
        string attack = "";
        if(other.gameObject.CompareTag("PlayerTrail")){
            TakeDamage(PowerUpManager.upgradableDatas.trailDamage);
            attack = "player trail";
        }
        else if(other.gameObject.CompareTag("ClosedShape")){
            ClosedShape closedShape = other.gameObject.GetComponent<ClosedShape>();
            ShapeAttack(closedShape.shape);
            closedShape.AddEnemy(); //for triangle gravity
            attack = "closed shape";
        }
        else if(other.gameObject.CompareTag("Projectile")){
            attack = "projectile";
            TakeDamage(PowerUpManager.upgradableDatas.projectileDamage);
        }
        else if(other.gameObject.CompareTag("ShockWave")){
            attack = "shock wave";
            TakeDamage(PowerUpManager.upgradableDatas.waveDamage);
        }
        else if (other.gameObject.CompareTag("Bomb"))
        {
            TakeDamage(PowerUpManager.upgradableDatas.bombDamage);
            attack = "bomb";
        }
        else if (other.gameObject.CompareTag("ShapeField"))
        {
            attack = "triangle field";
            isAttracked = true;
            attractionTarget = other.transform.position;
        }
        else if (other.gameObject.CompareTag("PentagonBlade"))
        {
            TakeDamage(GameManagerV2.instance.skills.bladeDamage, true);
            attack = "pentagon blade";
        }
        else if (other.gameObject.CompareTag("PentagonWave"))
        {
            float chance = Random.Range(0f, 1f);
            if(chance > GameManagerV2.instance.skills.pentagonCriticalChance)
            {
                TakeDamage(GameManagerV2.instance.initialStats.pentagonImplosionBaseDamage, true);
            }
            else
            {
                TakeDamage(GameManagerV2.instance.initialStats.pentagonImplosionCriticalDamage, true);
            }
            attack = "pentagon shock wave";
            
        }
        else if (other.gameObject.CompareTag("PentagonBomb"))
        {
            TakeDamage(GameManagerV2.instance.skills.pentagonBombDamage, true);
            attack = "pentagon bomb";
        }
        else if (other.gameObject.CompareTag("Meteor"))
        {
            TakeDamage(GameManagerV2.instance.skills.meteorDamage, true);
            attack = "meteor";
        }

        if(debug && attack != "")
        {
            Debug.Log(transform.name + "collide with " + attack);
        }
    }

    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ShapeField"))
        {
            //Debug.Log("enemy leave the triangle flied");
            isAttracked = false;
        }
        else if (collision.gameObject.CompareTag("ClosedShape"))
        {
            ClosedShape closedShape = collision.gameObject.GetComponent<ClosedShape>();
            switch (closedShape.shape) { 
                case GeometricalShape.Shape.Square:
                    if (burning)
                        burning = false;

                    if (slowed)
                    {
                        slowed = false;
                        speed = initialSpeed;
                    }
                    break;
                case GeometricalShape.Shape.Pentagon:
                    inPentagon = false; 
                    break;
                default:
                    break;
              
            }
            
        }
    }
    #endregion
    #region Public Functions
    public virtual void TakeDamage(float amount, bool exception = false)
    {
        if (invincible && !exception) return;
        if (shielded)
        {
            CommanderLink(false);
            return;
        }

        var finalAmount = amount * (1 + PowerUpManager.upgradableDatas.strengthBonus);
        if (takeExtraDamages)
        {
            finalAmount *= (1 + GameManagerV2.instance.skills.supportStrength);
        }

        if(debug)
            Debug.Log(transform.name + " takes " + finalAmount + " damages");
        
        health -= finalAmount;

        GameManagerV2.instance.totalDamage += finalAmount;
        
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
    public virtual void Die(bool disable = true){
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

        GameManagerV2.instance.enemyKilled += 1;
        spriteRenderer.color = originalColor;
        isAttracked = false;
        takeExtraDamages = false;
        DOT_Timer = 0f;
        stun = 0f;

        if (inPentagon)
        {
            float chance = Random.Range(0f, 1f);
            if(chance <= GameManagerV2.instance.initialStats.pentagonDrainChance){ 
                GameManagerV2.instance.ModifyHealth(GameManagerV2.instance.skills.pentagonHeal);
            }
            inPentagon = false;
        }

        if(disable)
            gameObject.SetActive(false);
    }

    public void CommanderLink(bool value)
    {
        if (shielded == value)
        {
            return;
        }
        if (value)
        {
            shielded = true;
            shieldTransform.gameObject.SetActive(true);
            speed += speed * 0.5f;
        }
        else
        {
            shielded = false;
            shieldTransform.gameObject.SetActive(false);
            speed -= speed * 0.5f;
        }
    }
    #endregion

    #region Private Functions

    //Flash animation when the enemy takes damage
    protected IEnumerator DamageFlash()
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
                TakeDamage(GameManagerV2.instance.skills.triangleDamage);

                if(GameManagerV2.instance.skills.DOT > 0f)
                {
                    //Debug.Log("enemy taking DOT");
                    DOT_Timer += 5f;
                    if(gameObject.activeSelf)
                        StartCoroutine(DamageOverTime());
                }

                stun = GameManagerV2.instance.skills.stunDuration;

                if ((GameManagerV2.instance.skills.supportStrength > 0f) && !takeExtraDamages)
                {
                    takeExtraDamages = true;
                }

               
                break;
            case GeometricalShape.Shape.Square:
                TakeDamage(GameManagerV2.instance.skills.squareDamage);

                if((GameManagerV2.instance.skills.squareTrap > 0f) && !trapped)
                {
                    trapped = true;
                    speed *= 1-GameManagerV2.instance.skills.squareTrap;
                }

                if((GameManagerV2.instance.skills.squareSlow > 0f)&&!slowed)
                {
                    initialSpeed = speed;
                    speed *= 1 - GameManagerV2.instance.skills.squareSlow;
                    slowed = true;
                }

                if((GameManagerV2.instance.skills.squareFlame > 0f) && gameObject.activeSelf)
                {
                    burning = true;
                    StartCoroutine(FlameDamage());
                }

                break;
            case GeometricalShape.Shape.Pentagon:
                TakeDamage(GameManagerV2.instance.skills.pentagonDamage);
                inPentagon = true;
                break;
            case GeometricalShape.Shape.Hexagon:
                TakeDamage(GameManagerV2.instance.skills.hexagonDamage);
                if(GameManagerV2.instance.skills.hexagonSlow > 0f)
                {
                    speed *= 1 - GameManagerV2.instance.skills.hexagonSlow;
                }
                break;
            default:
                TakeDamage(PowerUpManager.upgradableDatas.trailDamage);
                break;
        }
    }

    private IEnumerator DamageOverTime()
    {
        while(DOT_Timer > 0f)
        {
            TakeDamage(GameManagerV2.instance.skills.DOT, true);
            DOT_Timer -= 1f;
            yield return new WaitForSeconds(GameManagerV2.instance.initialStats.triangleDOTInterval);
        }
        
    }

    private IEnumerator FlameDamage()
    {
        while (burning)
        {
            TakeDamage(GameManagerV2.instance.skills.squareFlame, true);
            yield return new WaitForSeconds(GameManagerV2.instance.initialStats.squareFlameDamageInterval);
        }

    }

    #endregion

}
