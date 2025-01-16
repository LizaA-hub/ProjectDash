using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    GameObject options;
    // Function to load the Main Gameplay scene
    public void StartGame()
    {
        //SceneManager.LoadScene("MainLevel");
        GameManager.StartGame(); //use the game manager so statics variables get initialized
    }

    // Function to load the Skill Tree scene
    public void OpenSkillTree()
    {
        SceneManager.LoadScene("SkillTree");
    }

    // Optional: Function to quit the game
    public void QuitGame()
    {
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
        options.SetActive(true);
    }

}
