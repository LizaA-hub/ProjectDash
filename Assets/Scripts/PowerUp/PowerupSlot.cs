using UnityEngine;
using UnityEngine.UI;

public class PowerupSlot : MonoBehaviour
{
    [SerializeField]
    int Id;
    public Image Icon, disableIcon;
    PowerupSlotManager controller;
    private void Start()
    {
        controller = transform.parent.gameObject.GetComponent<PowerupSlotManager>();
    }

    public void OnMouseOver()
    {
        controller.ShowDescription(Id);
    }

    public void OnMouseExit()
    {
        controller.HideDescription();
    }
}
