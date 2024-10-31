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


    private void Start() {
        GameManager.levelChange.AddListener(NewLevel);
    }
    public void OnFirstButtonClick(){
        SelectOption(0);  
    }
    public void OnSecondButtonClick(){
        SelectOption(1);  
    }
    public void OnThirdButtonClick(){
        SelectOption(2);  
    }

    private void NewLevel(int level){
        Time.timeScale = 0f;
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
        foreach (int powerId in correspondingPower)
        {
            if (powerId == Id){
                return true;
            }
        }
        return false;
    }

    private void SelectOption(int option){
        datas[correspondingPower[option]].level += 1;
        // TO DO : function to trigger change in the gamemanager sending type and level
        panel.SetActive(false);
        Time.timeScale = 1f;
        for(int i = 0; i < correspondingPower.Length ; i++){
            correspondingPower[i] = -1;
        }
    }
}
