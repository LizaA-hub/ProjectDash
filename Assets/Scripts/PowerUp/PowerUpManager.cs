using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PowerUpManager : MonoBehaviour
{
    [SerializeField]
    GameObject panel;
    [SerializeField]
    PowerUpDataManager.PowerUpData[] datas;
    [SerializeField]
    Transform[] buttons;
    int[] correspondingPower = {-1,-1,-1};
    List<int> generalUpgrades = new List<int>(), dashUpgrades = new List<int>();
    float cooldown = 1f;
    bool countdown = false;

    #region Unity Functions
    private void Start() {
        GameManager.levelChange.AddListener(NewLevel);
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
            level.text = "Level : " + datas[powerId].level;
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
            if(datas[Id].levelLimit >= datas[Id].level){
                return true;
            }
        }
        foreach (int powerId in correspondingPower)
        {
            if (powerId == Id){
                return true;
            }
        }
        return false;
    }

    private void SelectOption(int option){
        int id = correspondingPower[option];
        var data = datas[id];
        PowerUpDataManager.PowerUpType type = data.type;
        int level = data.level; 
        GameManager.Upgrade(type,level);
        data.level += 1;
        
        panel.SetActive(false);
        Time.timeScale = 1f;
        for(int i = 0; i < correspondingPower.Length ; i++){
            correspondingPower[i] = -1;
        }
        
        //add upgrade to slot//
        if(data.isDashAttack){
            if(!dashUpgrades.Contains(id)){
                dashUpgrades.Add(id); //TO DO: function to set up UI slot
            }
        }
        else{
            if(!generalUpgrades.Contains(id)){
                generalUpgrades.Add(id);//TO DO: function to set up UI slot
            }
        }
    }

    private int GetRandomPower(){
        //the upgrade slots are all full//
        if((generalUpgrades.Count == 3) && (dashUpgrades.Count ==3)){
            int powerId = Random.Range(0,6); //take a power within the slots
            if(powerId >= 3){// taking a dash attack 
                powerId -= 3;
                while (IsPowerTaken(dashUpgrades[powerId]))
                {
                    powerId += 1;
                    if(powerId >= 3){
                        powerId = 0;
                    }
                }
                return dashUpgrades[powerId];
            }
            else{//taking general upgrade
                while (IsPowerTaken(generalUpgrades[powerId]))
                {
                    powerId += 1;
                    if(powerId >= 3){
                        powerId = 0;
                    }
                }
                return generalUpgrades[powerId];
            }
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
                        while (IsPowerTaken(dashUpgrades[powerSlot]))
                        {
                            powerSlot += 1;
                            if(powerSlot >= 3){
                                powerSlot = 0;
                            }
                        }
                        return(dashUpgrades[powerSlot]);
                    }
                }
                else{//there's slots available
                    while (IsPowerTaken(powerId) && !data.isDashAttack) // if upgrade not available, take another dash attack upgrade in the pool
                    {
                        powerId += 1;
                        if(powerId >= datas.Length){
                            powerId = 0;
                        }
                        data = datas[powerId];
                    }
                    
                    return powerId;
                }
            }
            else{//is a general upgrade
                if(generalUpgrades.Count ==3){ // general slots are full
                    if(generalUpgrades.Contains(powerId) && !IsPowerTaken(powerId)){ // is within slots and available
                        return(powerId);
                    }
                    else{ // take another upgrade in general slotsthat is available
                        int powerSlot = Random.Range(0,3);
                        while (IsPowerTaken(generalUpgrades[powerSlot]))
                        {
                            powerSlot += 1;
                            if(powerSlot >= 3){
                                powerSlot = 0;
                            }
                        }
                        return(generalUpgrades[powerSlot]);
                    }
                }
                else{ // there's free slots
                    while (IsPowerTaken(powerId) && data.isDashAttack) //if upgrade not available take another one that is also a general upgrade
                    {
                        powerId += 1;
                        if(powerId >= datas.Length){
                            powerId = 0;
                        }
                        data = datas[powerId];
                    }
                    
                    return powerId;
                }
            }
        }
    }
    #endregion
}
