using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButton : MonoBehaviour
{
    [SerializeField]
    SkillScriptableObject skill;

    Button button;
    SkillTreeManager manager;

    private void Start()
    {
        button = GetComponent<Button>();
        manager = GameObject.FindObjectOfType<SkillTreeManager>();
        manager.skillUnlocked.AddListener(ToggleButton);
    }
    /*
    public void OnButtonClick()
    {
        if (skillTreeManager != null && skill != null)
        {
            if (skillTreeManager.UnlockSkill(skill))
            {
                UpdateButtonState();
            }
        }
    }*/

    public void ToggleButton(skillTypes type)
    {
        if(type == skill.type)
            button.interactable = true;
    }
}
