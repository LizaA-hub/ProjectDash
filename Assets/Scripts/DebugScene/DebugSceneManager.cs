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
    public void SpawnBasicEnemy(){
        SpawnEnemy(EnemyType.Basic);
        if(debug)
            Debug.Log("Basic enemy spawned");
    }

    public void SpawnChargingEnemy(){
        SpawnEnemy(EnemyType.Charging);
        if (debug)
            Debug.Log("Charging enemy spawned");
    }

    public void SpawnTankyEnemy()
    {
        SpawnEnemy(EnemyType.Tanky);
        if (debug)
            Debug.Log("Tanky enemy spawned");
    }
    public void SpawnProjectileEnemy()
    {
        SpawnEnemy(EnemyType.Projectile);
        if (debug)
            Debug.Log("Projectile enemy spawned");
    }

    public void SpawnAcidEnemy()
    {
        SpawnEnemy(EnemyType.Acid);
        if (debug)
            Debug.Log("Acid enemy spawned");
    }

    public void SpawnTeleportingEnemy()
    {
        SpawnEnemy(EnemyType.Teleporting);
        if (debug)
            Debug.Log("Teleporting enemy spawned");
    }
    #endregion
    #region Private Functions
    private void LevelUp(){
        float goal = GameManager.xpToNextLevel;
        float currentXp = GameManager.GetExperience();
        float diff = goal - currentXp;
        GameManager.ModifyExperience(diff);
    }

    private void SpawnEnemy(EnemyType type){
        spawner.TriggerSpawn(type);
    }
    #endregion
}
