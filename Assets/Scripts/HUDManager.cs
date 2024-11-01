using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [SerializeField]
    Slider XPSlider,healthSlider;
    [SerializeField]
    TMP_Text levelIndicator, timerText;
    [SerializeField]
    Image healthSliderFill;
    float timer = 0f;

    private void Start() {
        GameManager.XPChange.AddListener(ModifyXPSlider);
        ModifyXPSlider(GameManager.GetExperience());
        GameManager.levelChange.AddListener(ModifyLevel);
        ModifyLevel(1);

        GameManager.healthChange.AddListener(ModifyHealth);
        ModifyHealth(GameManager.maxHealth);

    }

    private void Update() {
        timer += Time.deltaTime;
        UpdateTime(timer);
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

    private void UpdateTime(float t){
        GameManager.gameDuration = t;
        int min = (int)(t/60f);
        int sec = (int)(t % 60f);
        string text =min.ToString("00") + ":" + sec.ToString("00");
        timerText.text = text;
    }
}
