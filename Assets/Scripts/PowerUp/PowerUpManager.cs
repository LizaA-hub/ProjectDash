using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Data;

public enum PowerUpType { Trail, XP, Strength, Projectile, Wave, Shield, Magnet, Cooldown, Sword, Bomb, None };

public class PowerUpManager : MonoBehaviour
{
    [SerializeField]
    GameObject panel;
    [SerializeField]
    PowerUpScriptableObject[] upgradePool;
    [SerializeField]
    Transform[] buttons;
    int[] correspondingPower = {-1,-1,-1};
    List<int> generalUpgrades = new List<int>(), dashUpgrades = new List<int>();
    float cooldown = 1f;
    bool countdown = false;
    [HideInInspector]
    public UnityEvent<PowerUpType> powerupUnlocked = new UnityEvent<PowerUpType>();
    [HideInInspector]
    public static UnityEvent<int> trailIncrease = new UnityEvent<int>();
    [HideInInspector]
    public static UnityEvent magnetIncrease = new UnityEvent();
    [HideInInspector]
    public static PowerUpDataManager.UpgradableDatas upgradableDatas;

    PowerUpDataManager.PowerUpData[] datas;

    #region Unity Functions

    private void Awake()
    {
        upgradableDatas = new PowerUpDataManager.UpgradableDatas(0);
    }
    private void Start() {
        
        GameManager.levelChange.AddListener(NewLevel);
        datas = new PowerUpDataManager.PowerUpData[upgradePool.Length];
        for(int i = 0; i < upgradePool.Length ; i++)
        {
            datas[i] = new PowerUpDataManager.PowerUpData(upgradePool[i]);
        }
    }
    
    private void Update() {
        if(countdown){
            cooldown-=Time.unscaledDeltaTime;
            if (cooldown <=0f){
                cooldown = 1f;
                countdown = false;
            }
        }
    }
    #endregion
    #region Public Functions
    public void OnFirstButtonClick(){
        if(!countdown){
            SelectOption(0); 
        }  
    }
    public void OnSecondButtonClick(){
        if(!countdown){
            SelectOption(1); 
        }  
    }
    public void OnThirdButtonClick(){
        if(!countdown){
            SelectOption(2); 
        }  
    }

    public int IncreaseLevel(PowerUpType type)
    {
        for (int i = 0; i < datas.Length; i++)
        {
            if (datas[i].type == type)
            {
                if ((datas[i].levelLimit>0)&&(datas[i].levelLimit < datas[i].level))
                {
                    return 0;
                }
                else
                {
                    datas[i].level += 1;
                    return datas[i].level;
                }
                
            }
        }
        return 0;
    }

    public int DebbugPowerup(PowerUpType type)
    {
        int id = 0;
        for (int i = 0; i < datas.Length; i++)
        {
            if (type == datas[i].type)
            {
                id = i; break;
            }
        }

        var data = datas[id];
        int level = data.level;
        if (data.isDashAttack)
        {
            if(dashUpgrades.Count == 3 && !dashUpgrades.Contains(id))
            {
                Debug.Log("Dash slots are full.");
                return level-1;
            }
        }
        else
        {
            if(generalUpgrades.Count == 3 && !generalUpgrades.Contains(id))
            {
                Debug.Log("general slots are full.");
                return level-1;
            }
        }

        Upgrade(type, level);

        
        if (IncreaseLevel(type) == 0)
        {
            Debug.Log("max level reached");
        }

        UnlockPowerup(id);

        return level;
    }

    public bool IsDashAttack(PowerUpType type)
    {
        foreach (var data in datas)
        {
            if(data.type == type)
            {
                return data.isDashAttack;
            }
        }
        return false;
    }

    public Sprite GetIcon(PowerUpType type)
    {
        foreach (var data in datas)
        {
            if (data.type == type)
            {
                return data.icon;
            }
        }
        return null;
    }

    public string GetName(PowerUpType type)
    {
        foreach (var data in datas)
        {
            if (data.type == type)
            {
                return data.name;
            }
        }
        return "";
    }

    public int GetLevel(PowerUpType type)
    {
        foreach (var data in datas)
        {
            if (data.type == type)
            {
                return data.level;
            }
        }
        return 0;
    }

    #endregion
    #region Private Functions
    private void NewLevel(int level){
        Time.timeScale = 0f;
        countdown = true;
        ShowPanel();
    }

    private void SetButtonText(){
        for (int i = 0; i < buttons.Length; i++)
        {
            TMP_Text name = buttons[i].Find("Panel/Name").gameObject.GetComponent<TMP_Text>();
            TMP_Text description= buttons[i].Find("Description").gameObject.GetComponent<TMP_Text>();
            TMP_Text level = buttons[i].Find("Panel/Level").gameObject.GetComponent<TMP_Text>();
            Image icon = buttons[i].Find("Panel/Icon").gameObject.GetComponent<Image>();

            int powerId = GetRandomPower();
          
            correspondingPower[i] = powerId;
            name.text = datas[powerId].name;
            description.text = datas[powerId].description;

            if (datas[powerId].levelLimit > 0)
            {
                if (datas[powerId].level >= datas[powerId].levelLimit)
                {
                    level.text = "Level max";
                }
                else
                {
                    level.text = "Level : " + datas[powerId].level;
                }
            }
            else
            {
                level.text = "Level : " + datas[powerId].level;
            }
            
            icon.sprite = datas[powerId].icon;
        }
    }

    private void ShowPanel(){
        panel.SetActive(true);
        SetButtonText();
        return;
    }

    private bool IsPowerTaken(int Id){
        if(datas[Id].levelLimit > 0){
            if(datas[Id].level >= datas[Id].levelLimit){
                return true;
            }
        }
        //Debug.Log("current displayed power :");
        foreach (int powerId in correspondingPower)
        {
            //Debug.Log(powerId);
            if (powerId == Id){
                return true;
            }
        }
        return false;
    }

    private void SelectOption(int option){
        int id = correspondingPower[option];
        var data = datas[id];
        PowerUpType type = data.type;
        int level = data.level; 
        
        if(IncreaseLevel(type) == 0)
        {
            Debug.Log("upgrade is at max level");
        }
        else
        {
            Upgrade(type,level);
        }

        panel.SetActive(false);
        Time.timeScale = 1f;
        for(int i = 0; i < correspondingPower.Length ; i++){
            correspondingPower[i] = -1;
        }

        UnlockPowerup(id);
    }
    public static void Upgrade(PowerUpType type, int level)
    {
        switch (type)
        {
            case PowerUpType.Trail:
                trailIncrease.Invoke(level);
                break;
            case PowerUpType.Strength:
                upgradableDatas.strengthBonus += 0.1f;
                break;
            case PowerUpType.XP:
                upgradableDatas.xpMultiplier = 0.1f * level;
                break;
            case PowerUpType.Projectile:
                if (level == 1)
                {
                    upgradableDatas.haveProjectile = true;
                }
                upgradableDatas.projectileNb = level;

                break;
            case PowerUpType.Wave:
                if (level == 1)
                {
                    upgradableDatas.haveWave = true;
                }
                else
                {
                    upgradableDatas.waveRadius += upgradableDatas.waveRadius * 0.1f;
                    upgradableDatas.waveDamage += upgradableDatas.waveDamage * 0.1f;
                }
                break;
            case PowerUpType.Shield:
                upgradableDatas.shieldLevel += 1;
                break;
            case PowerUpType.Magnet:
                magnetIncrease.Invoke();
                break;
            case PowerUpType.Cooldown:
                upgradableDatas.dashCooldown -= upgradableDatas.dashCooldown * 0.1f;
                break;
            case PowerUpType.Sword:
                if (level == 1)
                {
                    upgradableDatas.haveSword = true;
                }
                else
                {
                    upgradableDatas.swordDamage += upgradableDatas.swordDamage * 0.1f;
                }
                break;
            case PowerUpType.Bomb:
                if (level == 1)
                {
                    upgradableDatas.haveBomb = true;
                }
                else
                {
                    upgradableDatas.bombDamage += upgradableDatas.bombDamage * 0.1f;
                    upgradableDatas.bombRadius += upgradableDatas.bombRadius * 0.1f;
                }
                break;
            default:
                break;
        }
    }

    private int GetRandomPower(){
        //the upgrade slots are all full//
        if((generalUpgrades.Count == 3) && (dashUpgrades.Count ==3)){
            int powerId = Random.Range(0,6); //take a power within the slots
            if(powerId >= 3){// taking a dash attack 
                powerId -= 3;
                for (int i = 0; i < 3; i++)
                {
                    powerId += i;
                    if (powerId >= 3)
                    {
                        powerId = 0;
                    }
                    if (!IsPowerTaken(dashUpgrades[powerId]))
                    {
                        return dashUpgrades[powerId];
                    }
                }
            }
            for (int i = 0; i < 3; i++)
            {
                powerId += i;
                if (powerId >= 3)
                {
                    powerId = 0;
                }
                if (!IsPowerTaken(generalUpgrades[powerId]))
                {
                    break;
                }
            }
            return generalUpgrades[powerId];
        }
        //upgrade slots are not all full//
        else{
            int powerId = Random.Range(0,datas.Length);
            var data = datas[powerId];
            if(data.isDashAttack){
                //dash attack slots are full//
                if(dashUpgrades.Count ==3){
                    if(dashUpgrades.Contains(powerId) && !IsPowerTaken(powerId)){ //upgrade in slot and available
                        return(powerId);
                    }
                    else{// take an upgrade within dash attack slots which is also available
                        int powerSlot = Random.Range(0,3);
                        for (int i = 0; i < 3; i++)
                        {
                            powerSlot += i;
                            if (powerSlot >= 3)
                            {
                                powerSlot = 0;
                            }
                            if (!IsPowerTaken(dashUpgrades[powerSlot]))
                            {
                                break;
                            }
                        }
                        return dashUpgrades[powerSlot];
                    }
                }
                else{//there's slots available
                    for (int i = 0; i < 3; i++)// if upgrade not available, take another dash attack upgrade in the pool
                    {
                        powerId += i;
                        if (powerId >= datas.Length)
                        {
                            powerId = 0;
                        }
                        if (!IsPowerTaken(powerId) && data.isDashAttack)
                        {
                            break;
                        }
                    }
                    return powerId;
                }
            
            }
            else{//is a general upgrade
                if(generalUpgrades.Count ==3){ // general slots are full
                    if (generalUpgrades.Contains(powerId) && !IsPowerTaken(powerId)){ // is within slots and available
                        return(powerId);
                    }
                    else{ // take another upgrade in general slots that is available
                        int powerSlot = Random.Range(0,3);
                        for (int i = 0; i < 3; i++)
                        {
                            powerSlot += i;
                            if (powerSlot >= 3)
                            {
                                powerSlot = 0;
                            }
                            if (!IsPowerTaken(generalUpgrades[powerSlot]))
                            {
                                break;
                            }
                        }
                        return generalUpgrades[powerSlot];
                    }
                }
                else{ // there's free slots
                    for (int i = 0; i < 3; i++)//if upgrade not available take another one that is also a general upgrade
                    {
                        powerId += i;
                        if (powerId >= datas.Length)
                        {
                            powerId = 0;
                        }
                        if (!IsPowerTaken(powerId) && !data.isDashAttack)
                        {
                            break;
                        }
                    }
                    return powerId;
                }
            }
        }
    }

    private void UnlockPowerup(int id)
    {
        if (datas[id].isDashAttack)
        {
            if (!dashUpgrades.Contains(id))
            {
                dashUpgrades.Add(id);
                powerupUnlocked.Invoke(datas[id].type);
            }
        }
        else
        {
            if (!generalUpgrades.Contains(id))
            {
                generalUpgrades.Add(id);
                powerupUnlocked.Invoke(datas[id].type);
            }
        }
    }

    
    #endregion
}
