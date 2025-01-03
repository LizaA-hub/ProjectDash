using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TeleportingEnemyController : EnemyController
{
    public float timer = 3f;
    [SerializeField]
    float r = 5f;
    [SerializeField]
    Transform bulletPrefab, bullet;
    private Vector3[] relativeDirections = { new Vector3(1, 0, 0), new Vector3(Mathf.Cos(Mathf.PI/4), Mathf.Sin(Mathf.PI/4), 0) , new Vector3(0, 1, 0) , new Vector3(Mathf.Cos(3*Mathf.PI/4), Mathf.Sin(3*Mathf.PI/4), 0)
                                             , new Vector3(-1, 0, 0) , new Vector3(Mathf.Cos(5*Mathf.PI/4), Mathf.Sin(5*Mathf.PI/4),0f) , new Vector3(0, -1, 0) , new Vector3(Mathf.Cos(7*Mathf.PI/4), Mathf.Sin(7*Mathf.PI/4), 0f) };
    private Transform bulletContainer;

    private void Start()
    {
        bulletContainer = GameObject.Find("BulletParent").transform;
        if (bulletContainer == null)
        {
            Debug.Log("bullet parent object not found");
        }
    }

    public IEnumerator Teleport(Vector3 playerPos)
    {
        float angle = Random.Range(0, 2 * Mathf.PI);
        Vector3 relativPos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
        transform.position = playerPos + r * relativPos;
        yield return new WaitForSeconds(0.5f);
        FireBullets();
    }

    private void FireBullets()
    {
        for (int j = 0; j < 8; j++)
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
                bullet = Instantiate(bulletPrefab, bulletContainer);
            }

            Bullet bulletControler = bullet.GetComponent<Bullet>();
            bulletControler.strength = strength;
            bulletControler.direction = relativeDirections[j];
            bullet.position = transform.position;
        }
    }

}
