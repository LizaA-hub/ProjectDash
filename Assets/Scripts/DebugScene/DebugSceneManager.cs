using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class DebugSceneManager : MonoBehaviour
{
    [SerializeField]
    GameObject panel;
    [SerializeField]
    EnemySpawnerDebug spawner;
    [SerializeField]
    TMP_Text DashAttackText, shockWaveText, shieldText, swordText, bombText;
    [SerializeField]
    PowerUpManager powerUpManager;
    public bool debug = true;

    bool showPanel = true;
    int dashAttackLevel = 0, shockWaveLevel = 0, shieldLevel = 0, swordLevel = 0, bombLevel=0;

    #region Unity Functions

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LevelUp();
        }
    }

    #endregion
    #region Public Functions
    public void ShowPanel(bool value){
        showPanel = value;
        panel.SetActive(showPanel);
    }

    public void SetHealth(string value){
        if(float.TryParse(value, out float result)){
            float amount = result - GameManagerV2.instance.GetHealth();
            GameManagerV2.instance.ModifyHealth(amount);
        }
        else{
            Debug.Log("invalid format");
        }
    }

    public void UnlockDash(){
        if(dashAttackLevel >= 9 ){
            Debug.Log("Dash attack is at max level.");
            return;
        }
        dashAttackLevel = powerUpManager.DebbugPowerup(PowerUpType.Projectile);
        DashAttackText.text = "Lvl " + dashAttackLevel;
    }

    public void UnlockWave(){
        shockWaveLevel = powerUpManager.DebbugPowerup(PowerUpType.Wave);
        shockWaveText.text = "Lvl " + shockWaveLevel;
    }

    public void UnlockShield()
    {
        shieldLevel = powerUpManager.DebbugPowerup(PowerUpType.Shield);
        shieldText.text = "Lvl " + shieldLevel;
    }

    public void UnlockSword()
    {
        swordLevel = powerUpManager.DebbugPowerup(PowerUpType.Sword);
        swordText.text = "Lvl " + swordLevel;
    }

    public void UnlockBomb()
    {
        bombLevel = powerUpManager.DebbugPowerup(PowerUpType.Bomb);
        bombText.text = "Lvl " + bombLevel;
    }
    public void SpawnEnemy(int type){
        spawner.TriggerSpawn((EnemyType)type);
        if (debug)
            Debug.Log((EnemyType)type + " enemy spawned");
    }
   
    #endregion
    #region Private Functions
    private void LevelUp(){
        float goal = GameManagerV2.instance.xpToNextLevel;
        float currentXp = GameManagerV2.instance.GetExperience();
        float diff = goal - currentXp;
        GameManagerV2.instance.ModifyExperience(diff);
    }

    #endregion
}
