using System.Collections;
using UnityEngine;

public class AcidEnemyController : EnemyController
{
    public bool alive;
    [SerializeField]
    GameObject acidPool;
    [SerializeField]
    float strengthFactor = 1f, areaDuration = 3f;
    private CircleCollider2D circleCollider;

    private void OnEnable()
    {
        alive = true;
        if (circleCollider == null)
        {
            circleCollider = gameObject.GetComponent<CircleCollider2D>();
        }
        spriteRenderer.enabled = true;
        circleCollider.enabled = true;

    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && type == EnemyType.Fast) {
            Die();
        }
    }
    public override void Die(bool disable = false)
    {
        if (!alive)
            return;


        base.Die(false);
        alive = false;
        spriteRenderer.enabled = false;
        circleCollider.enabled = false;

        StartCoroutine(Acid());
    }

    private IEnumerator Acid()
    {
        acidPool.SetActive(true);
        acidPool.GetComponent<DamageArea>().strength = strength* strengthFactor;
        yield return new WaitForSeconds(areaDuration);
        acidPool.SetActive(false);
        gameObject.SetActive(false);
    }
}
