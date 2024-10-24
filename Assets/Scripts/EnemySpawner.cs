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
    
    public EnemyDataManager.EnemyData[] datas;

    Vector3[] spawningPoints = new Vector3[4];
    List<Transform> instantiatedEnemies = new List<Transform>();
    float[] timers;
    float difficultyTimer = 0f;
    Vector3 velocity = Vector3.zero;

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
        //make the instantiated enemies folow the player
        FollowPlayer(Time.deltaTime);

        UpdateTimers(Time.deltaTime);
    }

    #endregion
    #region Public Functions

    public void RemoveEnemy(Transform enemy){
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
    }
    #endregion
    #region Private Functions

    private void SpawnEnemy(EnemyDataManager.EnemyType type,Vector3 position){
        foreach (var data in datas)
        {
            if(data.type == type){
                Transform enemy = Instantiate(data.prefab);
                enemy.position = position;
                instantiatedEnemies.Add(enemy);
                enemy.SetParent(transform,false); 
                var controller = enemy.GetComponent<EnemyController>();
                controller.health = data.maxHealth;
                controller.strength = data.strength;
                controller.speed = data.speed;
                controller.experience = data.experience;
                return;
            }
        }
        
    }

    private void FollowPlayer(float t){
        for (int i = 0; i < instantiatedEnemies.Count; i++)
        {
            var controller = instantiatedEnemies[i].GetComponent<EnemyController>();
            var step = controller.speed * t;
            
            instantiatedEnemies[i].position = Vector3.MoveTowards(instantiatedEnemies[i].position, player.position, step);
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
    #endregion

}
