using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour {


    private Dictionary<string, UnityEvent> eventDictonary;

    private static EventManager eventManager;

    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType<EventManager>();

                if (!eventManager)
                {
                    Debug.LogError("No active Event Manager in the scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }
            return eventManager;
        }
    }

    void Init()
    {
        if(eventDictonary == null)
        {
            eventDictonary = new Dictionary<string, UnityEvent>();
        }
    }



    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;

        if (instance.eventDictonary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.eventDictonary.Add(eventName, thisEvent);
        }


    }

    public static void StopListening(string eventName, UnityAction listener)
    {
        if (eventManager == null)
            return;

        UnityEvent thisEvent = null;

        if(instance.eventDictonary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }


    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;

        if(instance.eventDictonary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
}
