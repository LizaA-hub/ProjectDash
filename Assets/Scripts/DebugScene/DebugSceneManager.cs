using UnityEngine;

public class DebugSceneManager : MonoBehaviour
{
    [SerializeField]
    GameObject panel;
    [SerializeField]
    EnemySpawnerDebug spawner;

    bool showPanel = true;
    
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
        if(!GameManager.haveProjectile){
            GameManager.Upgrade(PowerUpDataManager.PowerUpType.UnlockProjectile,1 );
            Debug.Log("dash attack unlocked");
        }
        else{
            Debug.Log("dash attack already unlocked");
        }
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
