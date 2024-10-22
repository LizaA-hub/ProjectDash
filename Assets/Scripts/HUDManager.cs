using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [SerializeField]
    Slider XPSlider;
    [SerializeField]
    TMP_Text levelIndicator;

    private void Start() {
        GameManager.XPChange.AddListener(ModifyXPSlider);
        ModifyXPSlider(0f);
        GameManager.levelChange.AddListener(ModifyLevel);
        ModifyLevel(1);
    }

    private void ModifyXPSlider(float value){
        var maxXP = GameManager.xpToNextLevel;

        XPSlider.value = value/maxXP;
    }

    private void ModifyLevel(int value){
        var text = "Lvl " + value.ToString();
        levelIndicator.text = text;
    }
}
