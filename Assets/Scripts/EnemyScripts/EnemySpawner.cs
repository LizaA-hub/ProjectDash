using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    Transform map, player;
    [SerializeField]
    float difficultyIncreaseDelay = 30f;
    [SerializeField,Range(0,1)]
    float difficultyIncreasePercentage = 0.05f, functionParameter = 0.1f;

    private enum functionType{Linear, Exponential};
    [SerializeField]
    functionType function;
    [SerializeField]
    EnemyDataManager.EnemyData[] datas;

    Vector3[] spawningPoints = new Vector3[4];
    List<Transform> instantiatedEnemies = new List<Transform>();
    float[] timers;
    float difficultyTimer = 0f;
    Vector3 velocity = Vector3.zero;

    //spawninf function variables//
    EnemyController controller;
    Transform newEnemy;
    EnemyDataManager.EnemyData data;

    #region Unity Functions
    private void Start() {
        Vector3 mapSize = map.localScale;
        //implementing spawning point
        spawningPoints[0] = new Vector3(mapSize.x/2,0f,0f);
        spawningPoints[1] = new Vector3(0f,mapSize.y/2,0f);
        spawningPoints[2] = new Vector3(-mapSize.x/2,0f,0f);
        spawningPoints[3] = new Vector3(0f,-mapSize.y/2,0f);

        //setting a timer for each enemy type
        timers = new float[datas.Length];

        //spawning the firts enemies
        foreach (var point in spawningPoints)
        {
            SpawnEnemy(EnemyDataManager.EnemyType.Basic,point);
        }

    }

    private void Update() {
        
        UpdateEnemies(Time.deltaTime);

        UpdateTimers(Time.deltaTime);
    }

    #endregion
    #region Public Functions

    /*public void RemoveEnemy(Transform enemy){
        int toRemove = -1;
        for (int i = 0; i < instantiatedEnemies.Count; i++)
        {
            if(instantiatedEnemies[i] == enemy){
                toRemove = i;
            }
        }
        if (toRemove == -1){
            return;
        }
        instantiatedEnemies.RemoveAt(toRemove);
        GameManager.enemyKilled += 1;
    }*/
    #endregion
    #region Private Functions

    private void SpawnEnemy(EnemyDataManager.EnemyType type,Vector3 position){
        
        bool instantiateNewEnemy = true;

        //find the data corresponding to the type//
        foreach (var correspondingData in datas)
        {
            if(type == correspondingData.type){
                data = correspondingData;
                break;
            }
        }

        //check if an already instantiated child is available//
        foreach (var enemy in instantiatedEnemies)
        {
            if(!enemy.gameObject.activeSelf){
                controller = enemy.GetComponent<EnemyController>() ; 
                if(controller.type == type){
                    newEnemy = enemy;
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
        newEnemy.position = position;
        controller.health = data.maxHealth;
        controller.strength = data.strength;
        controller.speed = data.speed;
        controller.experience = data.experience;
        
        
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
                    //Basic enemy movement//
                    case EnemyDataManager.EnemyType.Basic:
                        instantiatedEnemies[i].position = Vector3.MoveTowards(instantiatedEnemies[i].position, player.position, step);
                        break;
                    //Dashing enemy movement//
                    case EnemyDataManager.EnemyType.Charging:
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
    
                    default:
                        instantiatedEnemies[i].position = Vector3.MoveTowards(instantiatedEnemies[i].position, player.position, step);
                        break;
            }
            }
        }
    }


    private void UpdateTimers(float t){

        for (int i = 0; i < datas.Length; i++)
        {
          timers[i] += t;
          if (timers[i] >= datas[i].spawnDelay){//does the timer exceed the corresponding delay?
            timers[i] -= datas[i].spawnDelay;
            foreach (var point in spawningPoints)
            {
                SpawnEnemy(datas[i].type,point);
            }
          }
          
        }

        difficultyTimer += t;
        if (difficultyTimer >= difficultyIncreaseDelay){
            difficultyTimer -= difficultyIncreaseDelay;
            IncreaseDifficulty();
        }
    }

    private void IncreaseDifficulty(){
        
        for (int i = 0; i < datas.Length; i++)
        {
            if (function == functionType.Linear){
                var _delay = datas[i].spawnDelay;
                if (_delay > 1f){
                    _delay -= functionParameter;
                    datas[i].spawnDelay = _delay;
                }
                
            }
            else{
                var _delay = datas[i].spawnDelay;
                if (_delay > 1f){
                    _delay *= Mathf.Exp(-functionParameter);
                    datas[i].spawnDelay = _delay;
                }
            }
            datas[i].maxHealth += datas[i].maxHealth*difficultyIncreasePercentage;
            datas[i].strength += datas[i].strength*difficultyIncreasePercentage;
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
