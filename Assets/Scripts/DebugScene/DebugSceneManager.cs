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
    TMP_Text DashAttackText, shockWaveText, shieldText, swordText;
    [SerializeField]
    PowerUpManager powerUpManager;

    bool showPanel = true;
    int dashAttackLevel = 0, shockWaveLevel = 0, shieldLevel = 0, swordLevel = 0;

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
            float amount = result - GameManager.GetHealth();
            GameManager.ModifyHealth(amount);
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
        dashAttackLevel = powerUpManager.DebbugPowerup(PowerUpDataManager.PowerUpType.Projectile);
        DashAttackText.text = "Lvl " + dashAttackLevel;
    }

    public void UnlockWave(){
        shockWaveLevel = powerUpManager.DebbugPowerup(PowerUpDataManager.PowerUpType.Wave);
        shockWaveText.text = "Lvl " + shockWaveLevel;
    }

    public void UnlockShield()
    {
        shieldLevel = powerUpManager.DebbugPowerup(PowerUpDataManager.PowerUpType.Shield);
        shieldText.text = "Lvl " + shieldLevel;
    }

    public void UnlockSword()
    {
        swordLevel = powerUpManager.DebbugPowerup(PowerUpDataManager.PowerUpType.Sword);
        swordText.text = "Lvl " + swordLevel;
    }
    public void SpawnBasicEnemy(){
        SpawnEnemy(EnemyDataManager.EnemyType.Basic);
        Debug.Log("Basic enemy spawned");
    }

    public void SpawnChargingEnemy(){
        SpawnEnemy(EnemyDataManager.EnemyType.Charging);
        Debug.Log("Charging enemy spawned");
    }

    public void SpawnTankyEnemy()
    {
        SpawnEnemy(EnemyDataManager.EnemyType.Tanky);
        Debug.Log("Tanky enemy spawned");
    }
    #endregion
    #region Private Functions
    private void LevelUp(){
        float goal = GameManager.xpToNextLevel;
        float currentXp = GameManager.GetExperience();
        float diff = goal - currentXp;
        GameManager.ModifyExperience(diff);
    }

    private void SpawnEnemy(EnemyDataManager.EnemyType type){
        spawner.TriggerSpawn(type);
    }
    #endregion
}
