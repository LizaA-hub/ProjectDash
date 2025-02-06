
using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [HideInInspector]
    public EnemySpawnerV2 spawner;
    [HideInInspector]
    public Transform player;
    //[HideInInspector]
    public float speed = 1;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool invincible, burning, takeExtraDamages;
    private float cooldown=0.5f, health, DOT_Timer;
    private void Awake()
    {
        // Get the SpriteRenderer component and store the original color
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void Update()
    {
        if (invincible)
        {
            cooldown -= Time.deltaTime;
            if (cooldown <= 0f)
            {
                invincible = false;
                cooldown = 0.5f;
            }
        }

        UpdateMovement(Time.deltaTime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerTrail"))
        {
            TakeDamage(PowerUpManager.upgradableDatas.trailDamage);
        }
        else if (other.gameObject.CompareTag("ClosedShape"))
        {
            ClosedShape closedShape = other.gameObject.GetComponent<ClosedShape>();
            ShapeAttack(closedShape.shape);
        }
        else if (other.gameObject.CompareTag("Projectile"))
        {
            TakeDamage(PowerUpManager.upgradableDatas.projectileDamage);
        }
        else if (other.gameObject.CompareTag("ShockWave"))
        {
            TakeDamage(PowerUpManager.upgradableDatas.waveDamage);
        }
        else if (other.gameObject.CompareTag("Bomb"))
        {
            TakeDamage(PowerUpManager.upgradableDatas.bombDamage);
        }
        else if (other.gameObject.CompareTag("PentagonBlade"))
        {
            TakeDamage(GameManagerV2.instance.skills.bladeDamage, true);
        }
        else if (other.gameObject.CompareTag("PentagonWave"))
        {
            float chance = Random.Range(0f, 1f);
            if (chance > GameManagerV2.instance.skills.pentagonCriticalChance)
            {
                TakeDamage(GameManagerV2.instance.initialStats.pentagonImplosionBaseDamage, true);
            }
            else
            {
                TakeDamage(GameManagerV2.instance.initialStats.pentagonImplosionCriticalDamage, true);
            }

        }
        else if (other.gameObject.CompareTag("PentagonBomb"))
        {
            TakeDamage(GameManagerV2.instance.skills.pentagonBombDamage, true);
        }
        else if (other.gameObject.CompareTag("Meteor"))
        {
            TakeDamage(GameManagerV2.instance.skills.meteorDamage, true);
        }

    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ClosedShape") && 
            collision.gameObject.GetComponent<ClosedShape>().shape == GeometricalShape.Shape.Square 
            && burning) burning = false;
        
    }

    protected virtual void UpdateMovement(float t)
    {
        if (spawner != null) {
            spawner.EndBossFight();
        }
    }

    protected virtual void TakeDamage(float amount, bool exception = false)
    {
        if (invincible && !exception) return;

        var finalAmount = amount * (1 + PowerUpManager.upgradableDatas.strengthBonus);
        if (takeExtraDamages)
        {
            finalAmount *= (1 + GameManagerV2.instance.skills.supportStrength);
        }

        health -= finalAmount;

        GameManagerV2.instance.totalDamage += finalAmount;

        if (health <= 0f)
        {
            Die();
        }
        else if (spriteRenderer != null)
        {
            StartCoroutine(DamageFlash()); // Start the flash coroutine
        }

        if (!exception)
        {
            invincible = true;
        }
    }

    protected virtual void Die()
    {

    }

    private void ShapeAttack(GeometricalShape.Shape shape)
    {
        switch (shape)
        {
            case GeometricalShape.Shape.Triangle:
                TakeDamage(GameManagerV2.instance.skills.triangleDamage);

                if (GameManagerV2.instance.skills.DOT > 0f)
                {
                    //Debug.Log("enemy taking DOT");
                    DOT_Timer += 5f;
                    if (gameObject.activeSelf)
                        StartCoroutine(DamageOverTime());
                }

                if ((GameManagerV2.instance.skills.supportStrength > 0f) && !takeExtraDamages)
                {
                    takeExtraDamages = true;
                }

                break;

            case GeometricalShape.Shape.Square:
                TakeDamage(GameManagerV2.instance.skills.squareDamage);

                if ((GameManagerV2.instance.skills.squareFlame > 0f) && gameObject.activeSelf)
                {
                    burning = true;
                    StartCoroutine(FlameDamage());
                }

                break;

            case GeometricalShape.Shape.Pentagon:
                TakeDamage(GameManagerV2.instance.skills.pentagonDamage);
                break;

            case GeometricalShape.Shape.Hexagon:
                TakeDamage(GameManagerV2.instance.skills.hexagonDamage);
                break;

            default:
                TakeDamage(PowerUpManager.upgradableDatas.trailDamage);
                break;
        }
    }
    private IEnumerator DamageFlash()
    {
        // Flash white
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f); // Adjust the flash duration as needed

        // Return to the original color
        spriteRenderer.color = originalColor;
    }
    private IEnumerator DamageOverTime()
    {
        while (DOT_Timer > 0f)
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
}
