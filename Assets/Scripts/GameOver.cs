using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameOver : MonoBehaviour
{
    [SerializeField]
    GameObject panel;
    [SerializeField]
    TMP_Text time,damage,kill;

    float cooldown = 1f;
    bool countdown = false;

    private void Start() {
        GameManager.gameOver.AddListener(OnGameOver);
    }

    private void Update() {
        if(countdown){
            cooldown-=Time.unscaledDeltaTime;
            if (cooldown <=0f){
                cooldown = 1f;
                countdown = false;
            }
        }
    }

    public void Restart(){
        if(!countdown){
            GameManager.StartGame();
        }
    }
    public void Menu(){
        if(!countdown){
            GameManager.LoadMenu();
        }
    }
    private void OnGameOver(){
        //stop time//
        Time.timeScale = 0f;
        countdown = true;

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
