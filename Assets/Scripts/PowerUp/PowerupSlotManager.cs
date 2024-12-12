using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PowerupSlotManager : MonoBehaviour
{
    [SerializeField]
    Transform description;
    PowerUpManager powerUpManager;
    Image[] slotsIcon = new Image[6], iconDisable = new Image[6];
    PowerUpType[] unlockedPowerups = new PowerUpType[6];
    TMP_Text descriptionUI;

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var controller = child.gameObject.GetComponent<PowerupSlot>();
            slotsIcon[i] = controller.Icon;
            iconDisable[i] = controller.disableIcon;
        }

        powerUpManager = GameObject.Find("LevelUpUI").GetComponent<PowerUpManager>();
        if (powerUpManager != null)
        {
            powerUpManager.powerupUnlocked.AddListener(UnlockUpgrade);
            for (int i = 0; i < 6; i++)
            {
                unlockedPowerups[i] = PowerUpType.None;
            }
        }
        else
        {
            Debug.Log("level up UI object not found");
        }

        descriptionUI = description.gameObject.GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (unlockedPowerups.Length > 0)
            UpdateIconTimers();
    }

    private void UpdateIconTimers()
    {
        float div = 1;
        for(int i = 0; i < unlockedPowerups.Length; i ++)
        {
            var power = unlockedPowerups[i];
            switch (power)
            {
                case PowerUpType.Projectile:
                    if(PowerUpManager.upgradableDatas.projectileNb > 0)
                    {
                        div -= PlayerController.projectileTimer / PowerUpManager.upgradableDatas.dashCooldown;
                    }
                    break;
                case PowerUpType.Wave:
                    if (PowerUpManager.upgradableDatas.haveWave)
                    {
                        div -= PlayerController.shockWaveTimer / PowerUpManager.upgradableDatas.dashCooldown;
                    }
                    break;
                case PowerUpType.Shield:
                    if (PowerUpManager.upgradableDatas.shieldLevel > 0)
                    {
                        div -= Mathf.Clamp(PlayerController.dashShield,0,1);
                        
                    }
                    break;
                case PowerUpType.Sword:
                    if (PowerUpManager.upgradableDatas.haveSword)
                    {
                        div -= PlayerController.swordTimer / PowerUpManager.upgradableDatas.dashCooldown;
                    }
                    break;
                case PowerUpType.Bomb:
                    if (PowerUpManager.upgradableDatas.haveBomb)
                    {
                        div -= PlayerController.bombTimer / PowerUpManager.upgradableDatas.dashCooldown;
                    }
                    break;
                default:
                    break;
            }
            if(div != slotsIcon[i].fillAmount)
                slotsIcon[i].fillAmount = div;
        }
    }

    public void ShowDescription(int slot)
    {
        //get variables//
        if(unlockedPowerups[slot] == PowerUpType.None)
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

    private void UnlockUpgrade(PowerUpType type)
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
            if (unlockedPowerups[i] == PowerUpType.None)
            {
                unlockedPowerups[i] = type;
                Sprite icon = powerUpManager.GetIcon(type);
                if (icon != null)
                {
                    slotsIcon[i].enabled = true;
                    slotsIcon[i].sprite = icon;
                    iconDisable[i].sprite = icon;
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
