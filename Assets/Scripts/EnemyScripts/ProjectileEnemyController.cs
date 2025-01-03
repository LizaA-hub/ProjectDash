using UnityEngine;

public class ProjectileEnemyController : EnemyController
{
    public float timer = 5f;
    [SerializeField]
    Transform bulletPrefab;
    float distToPlayer = 4f;
    protected bool bulletMoving = false;
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
    public virtual void FireBullet()
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
        bulletControler.strength = strength;
        bullet.position = transform.position;
        bulletControler.direction = transform.right;
    }
}
