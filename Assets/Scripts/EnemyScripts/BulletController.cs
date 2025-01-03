
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField]
    Transform map;
    // Update is called once per frame
    void Update()
    {
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.gameObject.activeSelf)
                {
                    Bullet controller = child.gameObject.GetComponent<Bullet>();//float step = t * bulletSpeed;bullet.position = Vector3.MoveTowards(bullet.position, bullet.position + bulletDir, step);
                    float step = Time.deltaTime * controller.speed;
                    child.position = Vector3.MoveTowards(child.position, child.position + controller.direction, step);

                    if ((Mathf.Abs(child.position.x) > map.lossyScale.x / 2) || (Mathf.Abs(child.position.y) > map.lossyScale.y / 2))
                    {
                        child.gameObject.SetActive(false);
                    }
                }

            }
        }
    }
}
