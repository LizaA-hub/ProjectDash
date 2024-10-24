using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [SerializeField]
    Slider XPSlider,healthSlider;
    [SerializeField]
    TMP_Text levelIndicator;
    [SerializeField]
    Image healthSliderFill;

    private void Start() {
        GameManager.XPChange.AddListener(ModifyXPSlider);
        ModifyXPSlider(0f);
        GameManager.levelChange.AddListener(ModifyLevel);
        ModifyLevel(1);

        GameManager.healthChange.AddListener(ModifyHealth);
        ModifyHealth(GameManager.maxHealth);
    }

    private void ModifyXPSlider(float value){
        var maxXP = GameManager.xpToNextLevel;

        XPSlider.value = value/maxXP;
    }

    private void ModifyLevel(int value){
        var text = "Lvl " + value.ToString();
        levelIndicator.text = text;
    }

    private void ModifyHealth(float value){
        var maxHP = GameManager.maxHealth;
        var newValue = value/maxHP;
        healthSlider.value = newValue;
        //set color//
        healthSliderFill.color = Vector4.Lerp(Color.red,Color.green,newValue);
        
        
    }
}
