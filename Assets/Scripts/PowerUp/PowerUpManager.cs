using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUpManager : MonoBehaviour
{
    [SerializeField]
    GameObject panel;
    [SerializeField]
    PowerUpDataManager.PowerUpData[] datas;
    [SerializeField]
    Transform[] buttons;
    int[] correspondingPower = {-1,-1,-1};
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

            int powerId = Random.Range(0,datas.Length);
            if(datas.Length >= buttons.Length){
                
                        while (IsPowerTaken(powerId))
                        {
                            powerId +=1;
                            if(powerId >= datas.Length){
                                powerId = 0;
                            }
                        }
                }
          
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
        PowerUpDataManager.PowerUpType type = datas[correspondingPower[option]].type;
        int level = datas[correspondingPower[option]].level; 
        GameManager.Upgrade(type,level);
        datas[correspondingPower[option]].level += 1;
        
        panel.SetActive(false);
        Time.timeScale = 1f;
        for(int i = 0; i < correspondingPower.Length ; i++){
            correspondingPower[i] = -1;
        }
    }
    #endregion
}
