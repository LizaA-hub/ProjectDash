using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButton : MonoBehaviour
{
    [SerializeField]
    SkillScriptableObject skill;

    Color lockedColor = new Color(0.8f, 0.8f,0.8f, 0.5f);

    Button button;
    Image image;
    SkillTreeManager manager;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);

        manager = GameObject.FindObjectOfType<SkillTreeManager>();
        manager.skillUnlocked.AddListener(ToggleButton);
        manager.unlockAdjacent.AddListener(GetAdjacentSkills);
        manager.resetSkills.AddListener(ResetColor);

        image = GetComponent<Image>();
        ResetColor();
    }
    
    public void OnButtonClick()
    {
        if (manager != null)
        {
            manager.LevelUPSkill();
        }
    }

    public void OnButtonHovered()
    {
        if (manager != null)
        {
            manager.ShowPanel(skill);
        }
    }

    private void ToggleButton(skillTypes type)
    {
        if (type == skill.type)
            image.color = Color.white;
    }

    private void GetAdjacentSkills(skillTypes type)
    {
        if (type == skill.type)
        {
            manager.UnlockSkills(skill.nextSkills);
        }
    }

    private void ResetColor()
    {
        image.color = lockedColor;
    }

}
