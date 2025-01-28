using System.Linq;
using UnityEngine;


public enum PackType { Square, Triangle, None};
public class HealthPack : MonoBehaviour
{
    public float healingAmount = 1.0f;
    public GeometricalShape.Shape[] acceptedShapes;
    public PackType type;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ClosedShape"))
        {
            ClosedShape closedShape = collision.gameObject.GetComponent<ClosedShape>();
            if (acceptedShapes.Contains(closedShape.shape))
            {
                GameManagerV2.instance.ModifyHealth(healingAmount);
                transform.parent.gameObject.GetComponent<HealthPackManager>().RemoveHealthPack(transform);
            }
        }
    }

}
