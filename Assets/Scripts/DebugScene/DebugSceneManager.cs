using UnityEngine;
using TMPro;

public class DebugSceneManager : MonoBehaviour
{
    [SerializeField]
    GameObject panel;
    [SerializeField]
    EnemySpawnerDebug spawner;
    [SerializeField]
    TMP_Text DashAttackText, shockWaveText;

    bool showPanel = true;
    int dashAttackLevel = 0, shockWaveLevel = 0;
    
    #region Unity Functions

    private void Update() {
        if(Input.GetKeyDown(KeyCode.L)){
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
        dashAttackLevel += 1;
        GameManager.Upgrade(PowerUpDataManager.PowerUpType.Projectile,dashAttackLevel);
        DashAttackText.text = "Lvl " + dashAttackLevel;
    }

    public void UnlockWave(){
        shockWaveLevel += 1;
        GameManager.Upgrade(PowerUpDataManager.PowerUpType.Wave,shockWaveLevel);
        shockWaveText.text = "Lvl " + shockWaveLevel;
    }

    public void SpawnBasicEnemy(){
        SpawnEnemy(EnemyDataManager.EnemyType.Basic);
        Debug.Log("Basic enemy spawned");
    }

    public void SpawnChargingEnemy(){
        SpawnEnemy(EnemyDataManager.EnemyType.Charging);
        Debug.Log("Charging enemy spawned");
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
