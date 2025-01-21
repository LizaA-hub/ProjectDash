
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyEventManager : MonoBehaviour
{
    [SerializeField]
    private EventScriptableObject[] eventPool;

    /*[Header("Variables to increase enemy difficulty over time.")]
    [SerializeField, Range(0, 1)]
    float speedMultiplier = 0.05f, healthMultiplier = 0.1f, strengthMultiplier = 0.1f;*/

    private float timer;

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
        }
    }

    private List<activeEvent> activeEvents = new List<activeEvent>();
    private List<activeEvent> inactiveEvents = new List<activeEvent>();

    private void Update()
    {
        if(timer <= 0f)
        {
            PickEvent();
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
        List<ScriptableObject> possibleEvents = new List<ScriptableObject>();
        possibleEvents.AddRange(eventPool);
        int i = Random.Range(0, possibleEvents.Count);
        EventScriptableObject newEvent = eventPool[i];
        while((newEvent.activeAfter > GameManager.gameDuration) && possibleEvents.Count > 0)
        {
            possibleEvents.Remove(newEvent);
            i = Random.Range(0, possibleEvents.Count);
            newEvent = eventPool[i];
        }
        if(newEvent.activeAfter < GameManager.gameDuration)
        {
            if(newEvent.probability < 1f)
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

    private void StartEvent(EventScriptableObject _event)
    {
        bool createEvent = true;
        activeEvent newEvent;
        //check if there's a corresponding event in the inactive events
        if (inactiveEvents.Count > 0) {
            for(int i = 0; i < inactiveEvents.Count; i++)
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
    }

    private void UpdateEvents(float t)
    {
        List<activeEvent> eventDeactivated = new List<activeEvent>();
        if (activeEvents.Count > 0) {
            for(int i =0;i<activeEvents.Count;i++)
            {
                activeEvent activeEvent = activeEvents[i];
                if (activeEvent.isActive)
                {
                    activeEvent.duration -= t;
                    if(activeEvent.duration <= 0)
                    {
                        //deactivate event
                    }

                    //TO DO : update each enemies if they're moving

                    activeEvent.timer -= t;
                    if(activeEvent.timer <= 0)
                    {
                        //activeEvent.scriptableObject.SpawnEnemies(transform);//to do
                        activeEvent.timer = activeEvent.scriptableObject.spawnDelay;
                    }
                }
                else
                {
                    activeEvent.initialDelay -= t;
                    if(activeEvent.initialDelay <= 0)
                    {
                        //activeEvent.scriptableObject.SpawnEnemies(transform);//to do
                        activeEvent.isActive = true;
                    }
                           

                }
                activeEvents[i] = activeEvent;
            }
        }

        //TO DO : move the deactivated events from activ to inactiv
    }
}
