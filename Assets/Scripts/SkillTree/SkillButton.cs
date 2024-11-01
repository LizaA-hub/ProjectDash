using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButton : MonoBehaviour
{
    public Skill skill;
    public Image icon;
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI costText;
    private SkillTreeManager skillTreeManager;
    private Button button;

    private void Start()
    {
        // Locate SkillTreeManager
        skillTreeManager = FindObjectOfType<SkillTreeManager>();
        if (skillTreeManager == null)
        {
            Debug.LogError("SkillTreeManager not found in the scene.");
            return;
        }

        // Locate the Button component on this GameObject or its children
        button = GetComponentInChildren<Button>();
        if (button == null)
        {
            Debug.LogError("Button component missing on SkillButton or its children.");
            return;
        }

        // Ensure Skill is assigned
        if (skill != null)
        {
            if (icon != null) icon.sprite = skill.icon;
            if (skillNameText != null) skillNameText.text = skill.skillName;
            if (costText != null) costText.text = $"Cost: {skill.cost}";
        }
        else
        {
            Debug.LogError("Skill not assigned to SkillButton.");
        }

        UpdateButtonState();
    }

    public void OnButtonClick()
    {
        if (skillTreeManager != null && skill != null)
        {
            if (skillTreeManager.UnlockSkill(skill))
            {
                UpdateButtonState();
            }
        }
    }

    private void UpdateButtonState()
    {
        if (skill == null)
        {
            Debug.LogError("Skill not assigned to SkillButton.");
            return;
        }

        // Set the button's interactable state based on the skill's unlock status
        button.interactable = !skill.isUnlocked;
    }
}
