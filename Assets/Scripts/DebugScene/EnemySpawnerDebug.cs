using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;

public class EnemySpawnerDebug : MonoBehaviour
{
    [SerializeField]
    EnemyScriptableObject[] enemyPool;
    
    Transform player;
    List<Transform> instantiatedEnemies = new List<Transform>();
    
    Vector3 velocity = Vector3.zero, cameraBound;
    Camera cam;
    //spawninf function variables//
    EnemyController controller;
    Transform newEnemy;
    EnemyDataManager.EnemyData data;

    #region Unity Functions
    private void Start() {
        player = GameObject.Find("Player").GetComponent<Transform>();
        if (player == null)
        {
            Debug.Log("player object not found");
        }

        cam = Camera.main;
        cameraBound = cam.ScreenToWorldPoint(Vector3.zero);

    }

    private void Update() {
        
        UpdateEnemies(Time.deltaTime);
    }

    #endregion
    #region Public Functions

    public void TriggerSpawn(EnemyType type){
            SpawnEnemy(type);
    }
    #endregion
    #region Private Functions

    private void SpawnEnemy(EnemyType type){
        
        bool instantiateNewEnemy = true;
        bool spawnNearPlayer = false;

        //find the data corresponding to the type//
        foreach (var enemy in enemyPool)
        {
            if(enemy.data.type == type){
                data = enemy.data;
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

        //check is it's a targetr dummy//
        if(type == EnemyType.Target)
        {
            spawnNearPlayer = true;
        }

        //set variables//
        newEnemy.position = GetSpawningPosition(spawnNearPlayer);
        controller.health = data.maxHealth;
        controller.strength = data.strength;
        controller.speed = data.speed;
        controller.experience = data.experience;
        
        
    }

    private Vector3 GetSpawningPosition(bool nearPlayer)
    {
        Vector3 position = Vector3.zero;

        if (nearPlayer)
        {
            float angle = Random.Range(0, 2 * Mathf.PI);
            Vector3 relativPos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
            position = player.position + 5f * relativPos;
        }
        else
        {
            int[] randomInt = { -1, 1 };
            float X = 0f, Y = 0f;
            switch (Random.Range(0, 2))
            {
                case 0://position up or down the camera
                    X = Random.Range(-1, 1);
                    Y = randomInt[Random.Range(0, 2)];
                    position = new Vector3(X * cameraBound.x, Y * cameraBound.y, 0f);
                    break;

                default://position left or rigth the camera
                    Y = Random.Range(-1, 1);
                    X = randomInt[Random.Range(0, 2)];
                    position = new Vector3(X * cameraBound.x, Y * cameraBound.y, 0f);
                    break;
            }

            position = new Vector3(X * cameraBound.x + cam.transform.position.x, Y * cameraBound.y + cam.transform.position.y, 0f);
        }
        return position;
    }

    private void UpdateEnemies(float t)
    {
        for (int i = 0; i < instantiatedEnemies.Count; i++)
        {
            if (instantiatedEnemies[i].gameObject.activeSelf)
            {
                var controller = instantiatedEnemies[i].GetComponent<EnemyController>();
                var step = controller.speed * t;
                if (controller.stun > 0f)
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
                        if (projectileController.timer <= 0f)
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
                        if (teleportingController.timer <= 0f)
                        {
                            StartCoroutine(teleportingController.Teleport(player.position));
                            teleportingController.timer = 3f;
                        }
                        break;
                    case EnemyType.Shield:
                        LookAtPlayer(instantiatedEnemies[i], step);
                        instantiatedEnemies[i].position = Vector3.MoveTowards(instantiatedEnemies[i].position, player.position, step);
                        break;
                    case EnemyType.Target:
                        break;
                    case EnemyType.Fast:
                        var fastController = instantiatedEnemies[i].GetComponent<AcidEnemyController>();
                        if (fastController.alive)
                        {
                            instantiatedEnemies[i].position = Vector3.MoveTowards(instantiatedEnemies[i].position, player.position, step);
                        }
                        break;
                    default://basic and tank enemies
                        instantiatedEnemies[i].position = Vector3.MoveTowards(instantiatedEnemies[i].position, player.position, step);
                        break;
                }
            }
        }
    }

    private void LookAtPlayer(Transform enemy, float step = 0f){
        Vector3 direction = player.position - enemy.position;
        direction = direction.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (step == 0)
        {
            enemy.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            enemy.rotation = Quaternion.Slerp(enemy.rotation, Quaternion.Euler(0, 0, angle), step);
        }

    }
    #endregion

}
