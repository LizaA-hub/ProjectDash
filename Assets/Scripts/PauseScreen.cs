using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    [SerializeField]
    Transform panel, optionPanel;
    bool isOpen = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePanel();
        }
    }

    public void TogglePanel()
    {
        isOpen = !isOpen;
        panel.gameObject.SetActive(isOpen);
        Time.timeScale = isOpen ? 0.0f : 1.0f;
    }

    public void Menu()
    {
        Time.timeScale = 1f;
        GameManager.LoadMenu();
    }

    public void Options()
    {
        optionPanel.gameObject.SetActive(true);
    }

}
