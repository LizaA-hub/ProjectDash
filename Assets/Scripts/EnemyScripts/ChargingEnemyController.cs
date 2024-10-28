using UnityEngine;
using System.Collections;

public class ChargingEnemyController : MonoBehaviour
{
    public float health = 50f;
    public float strength = 10f;
    public float speed = 2f;
    public float experience = 5f;
    public float cooldown = 1f;
    public float chargeDuration = 1f;     // Time to charge up before dashing
    public float dashSpeed = 5f;          // Speed during the dash
    public float dashDistance = 3f;       // Fixed distance for each dash
    [SerializeField]
    private Transform orbPrefab;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Vector3 dashDirection;        // Stores the direction for the dash
    private Vector3 dashTarget;           // The target position for the dash
    private bool isCharging = false;      // Indicates if the enemy is charging
    private bool isDashing = false;       // Indicates if the enemy is currently dashing
    private float chargeTimer = 0f;       // Tracks the charge duration
    private bool invincible = false;      // Prevents taking damage during cooldown

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
                cooldown = 1f;
            }
        }

        if (!isCharging && !isDashing)
        {
            FindPlayerAndStartCharge();
        }
        else if (isCharging)
        {
            chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0f)
            {
                StartDash();
            }
        }
        else if (isDashing)
        {
            DashForwardFixedDistance();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerTrail"))
        {
            TakeDamage(GameManager.playerStrength);
        }
    }

    private void FindPlayerAndStartCharge()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 direction = player.transform.position - transform.position;
            dashDirection = direction.normalized;  // Store the normalized direction to the player
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle); // Rotate to face the direction of the dash

            isCharging = true;
            chargeTimer = chargeDuration;  // Set timer for charging
        }
    }

    private void StartDash()
    {
        isCharging = false;
        isDashing = true;

        // Calculate dash target position once at the start of the dash
        dashTarget = transform.position + dashDirection * dashDistance;
    }

    private void DashForwardFixedDistance()
    {
        // Move towards the pre-calculated dash target
        transform.position = Vector3.MoveTowards(transform.position, dashTarget, dashSpeed * Time.deltaTime);

        // Stop dashing once the enemy has moved the full dash distance
        if (Vector3.Distance(transform.position, dashTarget) < 0.1f)
        {
            isDashing = false;
        }
    }

    private void TakeDamage(float amount)
    {
        if (invincible) return;

        health -= amount;
        if (spriteRenderer != null)
        {
            StartCoroutine(DamageFlash()); // Start the flash coroutine
        }

        if (health <= 0f)
        {
            Die();
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

    private void Die()
    {
        var orb = Instantiate(orbPrefab);
        orb.gameObject.GetComponent<XPOrb>().XPAmount = experience;
        orb.position = transform.position;

        Destroy(gameObject);
    }
}
