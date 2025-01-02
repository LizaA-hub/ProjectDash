using System.Collections;
using UnityEngine;

public class AcidEnemyController : EnemyController
{
    public bool alive;
    [SerializeField]
    GameObject acidPool;
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
        acidPool.GetComponent<AcidPool>().strength = strength;
        yield return new WaitForSeconds(3f);
        acidPool.SetActive(false);
        gameObject.SetActive(false);
    }
}
