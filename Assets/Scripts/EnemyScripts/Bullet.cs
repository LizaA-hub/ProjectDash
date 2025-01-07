using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 direction;
    public float speed = 5f, strength;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ( collision.transform.CompareTag("PlayerTrail"))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }
}
