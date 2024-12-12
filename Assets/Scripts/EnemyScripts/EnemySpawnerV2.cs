using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public enum EnemyType { Basic, Tanky, Fast, Charging, Projectile, None }

public class EnemySpawnerV2 : MonoBehaviour
{
    //inspector variabless//
    [SerializeField]
    float waveDelay = 30f;
    [Header("Variables to increase enemy difficulty over time.")]
    [SerializeField,Range(0,1)]
    float speedMultiplier = 0.05f, healthMultiplier = 0.1f, strengthMultiplier = 0.1f;
    [SerializeField]
    WaveScriptableObject[] waves;

    //private variables//
    Transform player;
    List<Transform> instantiatedEnemies = new List<Transform>();
    float[] intraWaveTimers;
    float interWaveTimer = 0f;
    Vector3 velocity = Vector3.zero, cameraBound;
    int waveNumber = 0;
    Camera cam;
    WaveScriptableObject currentWave;
    int[] enemySpawned;
    bool timersOn = true;

    //spawning function variables//
    EnemyController controller;
    Transform newEnemy;
    EnemyDataManager.EnemyData[] datas;
    EnemyDataManager.EnemyData data;

    #region Unity Functions
    private void Start() {
        player = GameObject.Find("Player").GetComponent<Transform>();
        if (player == null) {
            Debug.Log("player object not found");
        }

        cam = Camera.main;
        cameraBound = cam.ScreenToWorldPoint(Vector3.zero);

        if(waves.Length > 0)
        {
            NextWave();
        }
        else
        {
            Debug.Log("no wave in the waves array.");
        }
            
    }

    private void Update() {
        
        UpdateEnemies(Time.deltaTime);

        if(timersOn)
            UpdateTimers(Time.deltaTime);
    }

    #endregion
    #region Private Functions

    private void SpawnEnemy(EnemyScriptableObject enemy, int enemyGroup){
        
        bool instantiateNewEnemy = true;

        data = enemy.data;

        //check if an already instantiated child is available//
        foreach (var enemyTransform in instantiatedEnemies)
        {
            if(!enemyTransform.gameObject.activeSelf){
                controller = enemyTransform.GetComponent<EnemyController>() ; 
                if(controller.type == data.type){
                    newEnemy = enemyTransform;
                    newEnemy.gameObject.SetActive(true);
                    instantiateNewEnemy = false;
                    break;
                }
            }
        }

        //if no child available, instantiate a new one//
        if(instantiateNewEnemy){
            newEnemy = Instantiate(data.prefab);
            instantiatedEnemies.Add(newEnemy);
            newEnemy.SetParent(transform,false); 
            controller = newEnemy.GetComponent<EnemyController>() ; 
        }

        //set variables//
        newEnemy.position = GetSpawningPosition(); //to do function get random position based on the camera
        controller.health = data.maxHealth * ( 1+healthMultiplier * waveNumber);
        controller.strength = data.strength * (1 + strengthMultiplier * waveNumber);
        controller.speed = data.speed * (1 + speedMultiplier * waveNumber);
        controller.experience = data.experience;

        //check for special property in the wave
        if (currentWave.enemyGroups[enemyGroup].specialProperties == null) return;
        var length = currentWave.enemyGroups[enemyGroup].specialProperties.Length;
        if ( length > 0) {
            for (int i = 0; i < length; i++)
            {
                var property = currentWave.enemyGroups[enemyGroup].specialProperties[i].property;
                var multiplier = currentWave.enemyGroups[enemyGroup].specialProperties[i].propertyMultiplier;

                switch (property) 
                {
                    case EnemyDataManager.propertyType.Health:
                        controller.health *= (1 + multiplier);
                        break;
                    case EnemyDataManager.propertyType.Strength:
                        controller.strength *= (1 + multiplier);
                        break;
                    case EnemyDataManager.propertyType.Speed:
                        controller.speed *= (1 + multiplier);
                        break;
                    case EnemyDataManager.propertyType.XP:
                        controller.experience *= (1 + multiplier);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private Vector3 GetSpawningPosition()
    {
        int[] randomInt = {-1, 1};
        float X = 0f, Y = 0f;
        Vector3 position = Vector3.zero;
        switch (Random.Range(0, 2))
        {
            case 0://position up or down the camera
                X = Random.Range(-1, 1);
                Y = randomInt[Random.Range(0, 2)];
                position = new Vector3(X*cameraBound.x, Y*cameraBound.y, 0f);
                break;

            default://position left or rigth the camera
                Y = Random.Range(-1, 1);
                X = randomInt[Random.Range(0, 2)];
                position = new Vector3(X * cameraBound.x, Y * cameraBound.y, 0f);
                break;
        }

        position = new Vector3(X * cameraBound.x + cam.transform.position.x, Y * cameraBound.y + cam.transform.position.y, 0f);
        return position;
    }

    private void UpdateEnemies(float t){
        for (int i = 0; i < instantiatedEnemies.Count; i++)
        {
            if (instantiatedEnemies[i].gameObject.activeSelf)
            {
                var controller = instantiatedEnemies[i].GetComponent<EnemyController>();
                var step = controller.speed * t;
                switch (controller.type)
                {
                    //Dashing enemy movement//
                    case EnemyType.Charging:
                        var chargingController = instantiatedEnemies[i].GetComponent<ChargingEnemyControllerTEST>();
                        if (chargingController.isCharging)
                            {
                                LookAtPlayer(instantiatedEnemies[i]);
                                chargingController.chargeTimer -= t;
                                if (chargingController.chargeTimer <= 0f)
                                {
                                    chargingController.chargeTimer = 1f;
                                    chargingController.isCharging = false;
                                    chargingController.isDashing = true;
                                    chargingController.dashTarget = instantiatedEnemies[i].position + (player.position - instantiatedEnemies[i].position).normalized * chargingController.dashDistance;
                                }
                            }
                        else if (chargingController.isDashing)
                            {
                                instantiatedEnemies[i].position = Vector3.MoveTowards(instantiatedEnemies[i].position, chargingController.dashTarget, step);
                                // Stop dashing once the enemy has moved the full dash distance
                                if (Vector3.Distance(instantiatedEnemies[i].position, chargingController.dashTarget) < 0.1f)
                                {
                                    chargingController.isDashing = false;
                                }
                            }
                        else
                            {
                                LookAtPlayer(instantiatedEnemies[i]);
                                chargingController.isCharging = true;
                            }
                        break;
                    case EnemyType.Projectile:
                        var projectileController = instantiatedEnemies[i].GetComponent<ProjectileEnemyController>();
                        instantiatedEnemies[i].position = Vector3.MoveTowards(instantiatedEnemies[i].position, projectileController.GetTarget(player.position), step);
                        LookAtPlayer(instantiatedEnemies[i]);
                        projectileController.UpdateBullet(t);
                        break;
                    default://basic and tank enemies
                        instantiatedEnemies[i].position = Vector3.MoveTowards(instantiatedEnemies[i].position, player.position, step);
                        break;
            }
            }
        }
    }


    private void UpdateTimers(float t){

        //Timer inside wave
        for (int i = 0; i < intraWaveTimers.Length; i++)
        {
            if (enemySpawned[i] >= currentWave.enemyGroups[i].maxNumber) //check if the number of enemies in this wave is reach 
                continue;
            
            intraWaveTimers[i] += t;
            if (currentWave.enemyGroups[i].spawnDelay <= intraWaveTimers[i])
            {
                intraWaveTimers[i] = 0;
                enemySpawned[i]++;
                SpawnEnemy(currentWave.enemyGroups[i].enemy,i);
            }
        }
        //Global timer
        interWaveTimer += t;
        if (interWaveTimer >= waveDelay) {
            interWaveTimer = 0;
            NextWave();
        }
        
    }

    private void NextWave()
    {
        waveNumber++;
        if (waveNumber > waves.Length)
        {
            Debug.Log("no more waves!");
            timersOn = false;
        }
        else
        {
            currentWave = waves[waveNumber-1];
            //reset timers
            intraWaveTimers = new float[currentWave.enemyGroups.Length];
            enemySpawned = new int[currentWave.enemyGroups.Length];
        }
    }

    private void LookAtPlayer(Transform enemy){
        Vector3 direction = player.position - enemy.position;
        direction = direction.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        enemy.rotation = Quaternion.Euler(0, 0, angle);

    }
    #endregion

}
