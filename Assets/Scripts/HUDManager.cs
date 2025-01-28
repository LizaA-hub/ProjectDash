using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [SerializeField]
    Slider XPSlider,healthSlider;
    [SerializeField]
    TMP_Text levelIndicator, timerText;
    /*[SerializeField]
    Image healthSliderFill;*/
    float timer = 0f;

    private void Start() {
        GameManagerV2.instance.XPChange.AddListener(ModifyXPSlider);
        ModifyXPSlider(GameManagerV2.instance.GetExperience());
        GameManagerV2.instance.levelChange.AddListener(ModifyLevel);
        ModifyLevel(1);

        GameManagerV2.instance.healthChange.AddListener(ModifyHealth);
        ModifyHealth(GameManagerV2.instance.skills.maxHealth);
    }

    private void Update() {
        timer += Time.deltaTime;
        UpdateTime(timer);
    }

    private void ModifyXPSlider(float value){
        var maxXP = GameManagerV2.instance.xpToNextLevel;

        XPSlider.value = value/maxXP;
    }

    private void ModifyLevel(int value){
        var text = "Lvl " + value.ToString();
        levelIndicator.text = text;
    }

    private void ModifyHealth(float value){
        var maxHP = GameManagerV2.instance.skills.maxHealth;
        var newValue = value/maxHP;
        healthSlider.value = newValue;
        
        
    }

    private void UpdateTime(float t){
        GameManagerV2.instance.gameDuration = t;
        int min = (int)(t/60f);
        int sec = (int)(t % 60f);
        string text =min.ToString("00") + ":" + sec.ToString("00");
        timerText.text = text;
    }
}
