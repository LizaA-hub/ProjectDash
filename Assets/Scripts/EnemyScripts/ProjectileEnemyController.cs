using UnityEngine;

public class ProjectileEnemyController : EnemyController
{
    [SerializeField]
    Transform bulletPrefab;
    float distToPlayer = 4f, Timer = 5f, bulletSpeed = 5f;
    bool inCoolDown = true, bulletMoving = false;
    Vector3 bulletDir;
    Transform bulletContainer, bullet;

    private void Start()
    {
        bulletContainer = GameObject.Find("BulletParent").transform;
        if(bulletContainer == null)
        {
            Debug.Log("bullet parent object not found");
        }
    }
    public Vector3 GetTarget(Vector3 playerPos)
    {
        Vector3 diff = transform.position - playerPos;
        diff = diff.normalized*distToPlayer;
        if (Vector3.Distance(playerPos,transform.position)<= distToPlayer){
            return transform.position+diff;
        }
        else
        {
            return playerPos - diff;
        }
    }

    public void UpdateBullet(float t)
    {
        Timer -= t;
        if (Timer <= 0f)
        {
            if (!inCoolDown)
            {
                inCoolDown = true;
                StopBullet();
                Timer = 3f;
            }
            else
            {
                inCoolDown = false;
                FireBullet();
                Timer = 2f;
            }
        }

        if (bulletMoving)
        {
            float step = t * bulletSpeed;
            bullet.position = Vector3.MoveTowards(bullet.position, bullet.position + bulletDir, step);
        }
    }

    public void StopBullet()
    {
        bulletMoving = false;
        bullet.gameObject.SetActive(false);
    }

    private void FireBullet()
    {
        bool createBullet = true;
        //check for available bullet//
        if (bulletContainer.childCount > 0)
        {
            for (int i = 0; i < bulletContainer.childCount; i++)
                {
                    Transform child = bulletContainer.GetChild(i);
                    if (!child.gameObject.activeSelf)
                    {
                        bullet = child;
                        bullet.gameObject.SetActive(true);
                        createBullet = false;
                        break;

                    }
                }
        }
        if (createBullet)
        {
            bullet = Instantiate(bulletPrefab,bulletContainer);
        }
        
        Bullet bulletControler = bullet.GetComponent<Bullet>();
        bulletControler.parent = this;
        bulletControler.health = 1f;
        bulletControler.strength = strength;
        bullet.position = transform.position;
        bulletDir = transform.right;
        bulletMoving = true;
    }
}
