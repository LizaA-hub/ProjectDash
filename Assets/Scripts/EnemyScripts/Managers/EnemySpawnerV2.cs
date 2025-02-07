using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public enum EnemyType { Basic, Tanky, Fast, Charging, Projectile, Acid, Teleporting, Shield, Target, Commander,Boss_Test, None }

public class EnemySpawnerV2 : MonoBehaviour
{
    #region Variables Definition
    //inspector variabless//

    [Header("Variables to increase enemy difficulty over time.")]
    [SerializeField,Range(0,1)]
    float speedMultiplier = 0.05f, healthMultiplier = 0.1f, strengthMultiplier = 0.1f;
    [SerializeField]
    WaveScriptableObject[] waves;

    //private variables//
    Transform player;
    List<Transform> instantiatedEnemies = new List<Transform>();
    float[] intraWaveTimers;
    float interWaveTimer = 0f,waveDelay = 30f;
    Vector3 velocity = Vector3.zero, cameraBound;
    int waveNumber = 0, generalWaveNb = 0;
    Camera cam;
    WaveScriptableObject currentWave;
    int[] enemySpawned;
    bool timersOn = true, bossFight = false;

    //spawning function variables//
    EnemyController controller;
    Transform newEnemy;
    EnemyDataManager.EnemyData[] datas;
    EnemyDataManager.EnemyData data;

    #endregion

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
    #region Public Function

    public void EndBossFight()
    {
        if (bossFight) { 
            bossFight = false;
            NextWave();
        }
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
            controller = newEnemy.GetComponent<EnemyController>();
        }

        //set variables//
        newEnemy.position = GetSpawningPosition(); //to do function get random position based on the camera
        controller.health = data.maxHealth * ( 1+healthMultiplier * generalWaveNb);
        controller.strength = data.strength * (1 + strengthMultiplier * generalWaveNb);
        controller.speed = data.speed * (1 + speedMultiplier * generalWaveNb);
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
            if (instantiatedEnemies[i].gameObject.activeSelf && IsNotBoss(instantiatedEnemies[i]))
            {
                var controller = instantiatedEnemies[i].GetComponent<EnemyController>();
                var step = controller.speed * t;
                if (controller.stun>0f)
                {
                    controller.stun -= t;
                    continue;
                }
                else if (controller.isAttracked) //triangle gravity
                {
                    instantiatedEnemies[i].position = Vector3.MoveTowards(instantiatedEnemies[i].position, controller.attractionTarget, 2f * t);
                    continue;
                }
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
                        projectileController.timer -= t;
                        if(projectileController.timer <= 0f)
                        {
                            projectileController.FireBullet();
                            projectileController.timer = 5f;
                        }
                        instantiatedEnemies[i].position = Vector3.MoveTowards(instantiatedEnemies[i].position, projectileController.GetTarget(player.position), step);
                        LookAtPlayer(instantiatedEnemies[i]);
                        break;
                    case EnemyType.Acid:
                        var acidController = instantiatedEnemies[i].GetComponent<AcidEnemyController>();
                        if (acidController.alive)
                        {
                           instantiatedEnemies[i].position = Vector3.MoveTowards(instantiatedEnemies[i].position, player.position, step);
                        }
                        break;
                    case EnemyType.Teleporting:
                        var teleportingController = instantiatedEnemies[i].GetComponent<TeleportingEnemyController>();
                        teleportingController.timer -= t;
                        if(teleportingController.timer <= 0f)
                        {
                            StartCoroutine(teleportingController.Teleport(player.position));
                            teleportingController.timer = 3f;
                        }
                        break;
                    case EnemyType.Shield:
                        LookAtPlayer(instantiatedEnemies[i], step);
                        instantiatedEnemies[i].position = Vector3.MoveTowards(instantiatedEnemies[i].position, player.position, step);
                        break;
                    case EnemyType.Fast:
                        var fastController = instantiatedEnemies[i].GetComponent<AcidEnemyController>();
                        if (fastController.alive)
                        {
                            instantiatedEnemies[i].position = Vector3.MoveTowards(instantiatedEnemies[i].position, player.position, step);
                        }
                        break;
                    case EnemyType.Commander:
                        var commanderController = instantiatedEnemies[i].GetComponent<CommanderEnemycontroller>();
                        commanderController.timer -= t;
                        if(commanderController.timer <= 0f)
                        {
                            commanderController.timer = 10f;
                            LinkCommander(instantiatedEnemies[i]);
                        }
                        instantiatedEnemies[i].position = Vector3.MoveTowards(instantiatedEnemies[i].position, commanderController.GetTarget(player.position), step);
                        LookAtPlayer(instantiatedEnemies[i]);
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
        if(!bossFight)
        {
            interWaveTimer += t;
            if (interWaveTimer >= waveDelay)
            {
                interWaveTimer = 0;
                NextWave();
            }
        }
        
    }

    private void NextWave()
    {
        
        if (waveNumber >= waves.Length)
        {
            waveNumber = 0;
        }

        currentWave = waves[waveNumber];
        bossFight = currentWave.boss;
        //reset timers
        intraWaveTimers = new float[currentWave.enemyGroups.Length];
        enemySpawned = new int[currentWave.enemyGroups.Length];
            
        float duration = 0f;
        for (int i = 0; i < currentWave.enemyGroups.Length; i++) {
            float length = currentWave.enemyGroups[i].maxNumber * currentWave.enemyGroups[i].spawnDelay;
            if (length > duration) {
                duration = length; //find wave duration
            }
            if(currentWave.enemyGroups[i].initialNumber > 0) //initial spawn
            {
                for(int j = 0;j< currentWave.enemyGroups[i].initialNumber; j++)
                {
                    SpawnEnemy(currentWave.enemyGroups[i].enemy, i);
                }
            }
        }
        waveDelay = duration;
        
        waveNumber++;
        generalWaveNb++;
    }

    private void LookAtPlayer(Transform enemy, float step = 0f){
        Vector3 direction = player.position - enemy.position;
        direction = direction.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (step == 0){
            enemy.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            enemy.rotation = Quaternion.Slerp(enemy.rotation, Quaternion.Euler(0, 0, angle), step);
        }
        
    }

    private void LinkCommander(Transform enemy)
    {
        //finding the 5 closer enemies
        List<Transform> activEnemies = new List<Transform>();
        for (int i = 0; i < instantiatedEnemies.Count; i++)
        {
            if (instantiatedEnemies[i].gameObject.activeSelf && instantiatedEnemies[i] != enemy && IsNotBoss(instantiatedEnemies[i]))
            {
                activEnemies.Add(instantiatedEnemies[i]);
            }
        }
        if(activEnemies.Count > 5)
        {
            //arranging the list by distances in ascending order
            activEnemies.Sort((x, y) => Vector3.Distance(enemy.position, x.position).CompareTo( Vector3.Distance(enemy.position, y.position)));
            for (int i = 0; i < 5; i++)
            {
                activEnemies[i].GetComponent<EnemyController>().CommanderLink(true);
            }
        }
        else if (activEnemies.Count > 0) {
            foreach (var activEnemy in activEnemies)
            {
                activEnemy.GetComponent<EnemyController>().CommanderLink(true);
            }
        }
    }

    private bool IsNotBoss(Transform enemy) {
        var controller = enemy.GetComponent<EnemyController>();
        if(controller.type == EnemyType.Boss_Test)
        {
            return false;
        }
        return true;
    }
    #endregion

}
