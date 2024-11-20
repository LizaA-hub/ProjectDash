using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PowerupSlotManager : MonoBehaviour
{
    [SerializeField]
    Transform description;
    [SerializeField]
    PowerUpManager powerUpManager;
    Image[] slotsIcon = new Image[6];
    PowerUpDataManager.PowerUpType[] unlockedPowerups = new PowerUpDataManager.PowerUpType[6];
    TMP_Text descriptionUI;

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            slotsIcon[i] = child.gameObject.GetComponent<Image>();
        }
      
        powerUpManager.powerupUnlocked.AddListener(UnlockUpgrade);
        for (int i = 0; i < 6; i++)
        {
            unlockedPowerups[i] = PowerUpDataManager.PowerUpType.None;
        }

        descriptionUI = description.gameObject.GetComponent<TMP_Text>();
    }

    public void ShowDescription(int slot)
    {
        //get variables//
        if(unlockedPowerups[slot] == PowerUpDataManager.PowerUpType.None)
        {
            return;
        }
        string name = powerUpManager.GetName(unlockedPowerups[slot]);
        int level = powerUpManager.GetLevel(unlockedPowerups[slot]);
        level -= 1;
        //set text//
        string text = name +  "<br>" + "Level " + level;
        descriptionUI.enabled = true;
        descriptionUI.text = text;
        //set position//
        Vector2 position = description.position;
        Vector2 slotPosition = slotsIcon[slot].transform.position;
        position.x = slotPosition.x;
        description.position = position;

    }

    public void HideDescription()
    {
        descriptionUI.enabled = false;
    }

    private void UnlockUpgrade(PowerUpDataManager.PowerUpType type)
    {
        bool isDashAttack = powerUpManager.IsDashAttack(type);
        int a = 0, b = 6;

        if (isDashAttack) {
            a = 3;
        }
        else
        {
            b = 3;
        }

        for (int i = a; i < b; i++)
        {
            if (unlockedPowerups[i] == PowerUpDataManager.PowerUpType.None)
            {
                unlockedPowerups[i] = type;
                Sprite icon = powerUpManager.GetIcon(type);
                if (icon != null)
                {
                    slotsIcon[i].sprite = icon;
                }
                else
                {
                    Debug.Log("powerup slot manager can't find power up icon");
                }
                break;
            }
        }
    }


}