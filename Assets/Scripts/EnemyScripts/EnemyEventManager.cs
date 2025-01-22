
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static EnemyDataManager;

public class EnemyEventManager : MonoBehaviour
{
    [SerializeField]
    private EventScriptableObject[] eventPool;

    [Header("Variables to increase enemy difficulty over time.")]
    [SerializeField, Range(0, 1)]
    float speedMultiplier = 0.05f, healthMultiplier = 0.1f, strengthMultiplier = 0.1f;

    private float timer;
    private Transform player;
    private struct activeEvent
    {
        public EventScriptableObject scriptableObject;
        public bool isActive;
        public float timer, initialDelay, duration;
        public List<Transform> instantiatedEnemies;

        public activeEvent(EventScriptableObject _scriptableObject)
        {
            scriptableObject = _scriptableObject;
            isActive = false;
            timer = _scriptableObject.spawnDelay;
            initialDelay = _scriptableObject.initialDelay;
            duration = _scriptableObject.duration;
            instantiatedEnemies = new List<Transform>();
        }

        public void Reset()
        {
            isActive = false;
            timer = scriptableObject.spawnDelay;
            initialDelay = scriptableObject.initialDelay;
            duration = scriptableObject.duration;
        }
    }

    private List<activeEvent> activeEvents = new List<activeEvent>();
    private List<activeEvent> inactiveEvents = new List<activeEvent>();

    private void Start()
    {
        //Debug.Log("at start active events count = " + activeEvents.Count);
        player = GameObject.Find("Player").GetComponent<Transform>();
        if (player == null)
        {
            Debug.Log("player object not found");
        }
    }
    private void Update()
    {
        if (timer <= 0f)
        {
            if (eventPool.Length > 0)
            {
                PickEvent();
            }
            timer = 30f;
        }
        else
        {
            timer -= Time.deltaTime;
        }

        UpdateEvents(Time.deltaTime);
    }

    private void PickEvent()
    {
        //Debug.Log("picking a new event");
        List<ScriptableObject> possibleEvents = new List<ScriptableObject>();
        possibleEvents.AddRange(eventPool);

        //pick a random event in the possible even. Removing it if not valid and picking another one
        int i = Random.Range(0, possibleEvents.Count);
        EventScriptableObject newEvent = eventPool[i];
        while (!IsEventValid(newEvent) && possibleEvents.Count > 1)
        {
            possibleEvents.Remove(newEvent);
            i = Random.Range(0, possibleEvents.Count);
            newEvent = eventPool[i];
        }

        if (IsEventValid(newEvent))
        {
            if (newEvent.probability < 1f) //if the probability of the event is not 1, check if it occures
            {
                float porb = Random.Range(0f, 1f);
                if (porb >= newEvent.probability)
                {
                    return;
                }
            }
            StartEvent(newEvent);
        }
    }

    //cluster not implemented//
    private bool IsEventValid(EventScriptableObject _event) => (_event.activeAfter > GameManager.gameDuration || _event.type == EnemyDataManager.EventType.None || _event.type == EnemyDataManager.EventType.Cluster) ? false : true;

    private void StartEvent(EventScriptableObject _event)
    {
        //Debug.Log("starting event");
        bool createEvent = true;
        activeEvent newEvent;
        //check if there's a corresponding event in the inactive events
        if (inactiveEvents.Count > 0)
        {
            for (int i = 0; i < inactiveEvents.Count; i++)
            {
                if (inactiveEvents[i].scriptableObject == _event)
                {
                    newEvent = inactiveEvents[i];
                    createEvent = false;
                    newEvent.Reset();
                    activeEvents.Add(newEvent);
                    inactiveEvents.RemoveAt(i);
                    break;
                }
            }
        }
        //creating a new event
        if (createEvent)
        {
            newEvent = new activeEvent(_event);
            activeEvents.Add(newEvent);
        }

        //Debug.Log("after starting event active events count = " + activeEvents.Count);
    }

    private void UpdateEvents(float t)
    {

        if (activeEvents.Count > 0)
        {
            for(int i = activeEvents.Count-1; i >= 0; i--)
            {
                activeEvent iteration = activeEvents[i];
                if (activeEvents[i].isActive)
                {
                    //event end in this frame//
                    iteration.duration -= t;
                    if (iteration.duration <= 0)
                    {
                        StopEvent(iteration);
                        inactiveEvents.Add(activeEvents[i]);
                        activeEvents.RemoveAt(i);
                        continue;
                    }

                    //update enemies
                    foreach (var enemy in iteration.instantiatedEnemies)
                    {
                        if (!enemy.gameObject.activeSelf) continue;

                        var controller = enemy.gameObject.GetComponent<EnemyController>();
                        if (controller.isAttracked) //triangle gravity
                        {
                            enemy.position = Vector3.MoveTowards(enemy.position, controller.attractionTarget, 2f * t);
                            continue;
                        }

                        switch (iteration.scriptableObject.type)
                        {
                            case EnemyDataManager.EventType.Cluster:
                                if (controller.stun > 0f)
                                {
                                    controller.stun -= t;
                                    continue;
                                }
                                var step = controller.speed * t;
                                enemy.position = Vector3.MoveTowards(enemy.position, enemy.position + controller.direction, step);
                                break;
                        }
                    }

                    //checking the spawning delay//
                    iteration.timer -= t;
                    if (iteration.timer <= 0)
                    {
                        SpawnEnemies(iteration);
                        iteration.timer = iteration.scriptableObject.spawnDelay;
                    }
                }
                else //event inactive yet but in the active event list
                {
                    iteration.initialDelay -= t;
                    if (iteration.initialDelay <= 0)
                    {
                        SpawnEnemies(iteration);
                        iteration.isActive = true;
                    }


                }
                activeEvents[i] = iteration;
            }
        }

    }

    private void StopEvent(activeEvent _event)
    {
        //Debug.Log("stopping event");
        _event.isActive = false;
        foreach (var enemy in _event.instantiatedEnemies)
        {
            enemy.gameObject.SetActive(false);
        }
    }

    private void SpawnEnemies(activeEvent _event)
    {
        //Debug.Log("initializing enemies");
        float eventNumber = GameManager.gameDuration % 30f;
        float angle = 0;
        float X = 0;
        float Y = 0;
        Vector3 relativPos = Vector3.zero;
        Vector3 direction = Vector3.zero;
        Vector3 position = Vector3.zero;

        //get the number of enemies to spawn//
        float maxAmount = Mathf.Max(_event.scriptableObject.spawnRange.x, _event.scriptableObject.spawnRange.y);
        float minAmount = Mathf.Min(_event.scriptableObject.spawnRange.x, _event.scriptableObject.spawnRange.y);
        float amount = Random.Range(minAmount, maxAmount);
        amount = Mathf.Round(amount);

        for (int i = 0; i <= amount; i++)
        {
            switch (_event.scriptableObject.type) //setting variables based on the type of event
            {
                case EnemyDataManager.EventType.Cluster:
                    direction = Vector3.Normalize(player.position - position);

                    angle = Random.Range(0, 2 * Mathf.PI);
                    relativPos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
                    position = player.position + relativPos * _event.scriptableObject.spawnDistance;
                    X = Mathf.Max(_event.scriptableObject.scale.x, 1f);
                    Y = Mathf.Max(_event.scriptableObject.scale.y, 1f);
                    position += new Vector3(Random.Range(-X, X), Random.Range(-Y, Y), 0f);

                    break;
                case EnemyDataManager.EventType.Circle:
                    angle = (2 * Mathf.PI / (amount+1)) * i;
                    X = _event.scriptableObject.scale.x;
                    Y = _event.scriptableObject.scale.y;
                    relativPos = new Vector3(Mathf.Cos(angle) * X, Mathf.Sin(angle) * Y, 0f);
                    position = player.position + relativPos * _event.scriptableObject.spawnDistance;
                    
                    break;
            }

            Transform enemy = FindAvailableEnemy(_event); //check if an enemy transform is available
            var enemyData = _event.scriptableObject.Enemy.data;
            if (enemy == null)
            {
                enemy = Instantiate(enemyData.prefab, transform);
                _event.instantiatedEnemies.Add(enemy);
            }
            else
            {
                enemy.gameObject.SetActive(true);
            }
            

            //initialize variables
            enemy.position = position;
            var controller = enemy.gameObject.GetComponent<EnemyController>();
            controller.health = enemyData.maxHealth * (1 + healthMultiplier * eventNumber);
            controller.strength = enemyData.strength * (1 + strengthMultiplier * eventNumber);
            controller.speed = enemyData.speed * (1 + speedMultiplier * eventNumber);
            controller.experience = enemyData.experience;
            controller.direction = direction;
            
            }

        }

    private Transform FindAvailableEnemy(activeEvent _event)
    {
        if(_event.instantiatedEnemies.Count > 0)
        {
            foreach (var enemy in _event.instantiatedEnemies)
            {
                if (!enemy.gameObject.activeSelf)
                {
                    return enemy;
                }
            }
        }
        return null;
        
    }
}
        
