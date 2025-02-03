using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    GameObject options;
    // Function to load the Main Gameplay scene
    public void StartGame()
    {
        PlayClickUI();
        //SceneManager.LoadScene("MainLevel");
        GameManagerV2.instance.StartGame(); //use the game manager so statics variables get initialized
    }

    // Function to load the Skill Tree scene
    public void OpenSkillTree()
    {
        PlayClickUI();
        SceneManager.LoadScene("SkillTree");
    }

    // Optional: Function to quit the game
    public void QuitGame()
    {
        PlayClickUI();
        // For Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // For Standalone build
        Application.Quit();
#endif
    }


    public void OpenOptions()
    {
        PlayClickUI();
        options.SetActive(true);
    }


    private void PlayClickUI()
    {
        SoundManager.instance.PlaySound(SoundDataManager.soundType.UI_Click);
    }

}
