using UnityEngine;

public class Bullet : EnemyController
{
    public ProjectileEnemyController parent;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.transform.CompareTag("Enemy") && !other.transform.CompareTag("PlayerField"))
        {
            Die();
        }
    }
    public override void Die(bool disable = true)
    {
        parent.StopBullet();
    }
}
