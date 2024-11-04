using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameOver : MonoBehaviour
{
    [SerializeField]
    GameObject panel;
    [SerializeField]
    TMP_Text time,damage,kill;

    private void Start() {
        GameManager.gameOver.AddListener(OnGameOver);
    }

    public void Restart(){
        GameManager.StartGame();
    }
    public void Menu(){
        GameManager.LoadMenu();
    }
    private void OnGameOver(){
        //stop time//
        Time.timeScale = 0f;

        //show panel//
        panel.SetActive(true);

        //Set Duration text//
        float t = GameManager.gameDuration;
        int min = (int)(t/60f);
        int sec = (int)(t % 60f);
        string text =min.ToString("00") + ":" + sec.ToString("00");
        time.text = text;

        //set damage text//
        t = GameManager.totalDamages;
        damage.text = t.ToString();

        //set kill text//
        min = GameManager.enemyKilled;
        kill.text = min.ToString();
    }

}
