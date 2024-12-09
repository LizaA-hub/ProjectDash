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

        image = GetComponent<Image>();
        image.color = lockedColor;
    }
    
    public void OnButtonClick()
    {
        if (manager != null)
        {
            manager.ShowPanel(skill);
        }
    }

    public void ToggleButton(skillTypes type)
    {
        if (type == skill.type)
            image.color = Color.white;
    }
}
