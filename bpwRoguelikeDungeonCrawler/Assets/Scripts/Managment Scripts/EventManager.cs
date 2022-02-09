using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager {
    private static Dictionary<string, System.Action> eventDictionary = new Dictionary<string, System.Action>();

    public static void AddListener(string eventName, System.Action function) {
        if (!eventDictionary.ContainsKey(eventName)) {
            eventDictionary.Add(eventName, null);
        }
        eventDictionary[eventName] += function;
    }

    public static void RemoveListener(string eventName, System.Action function) {
        if (eventDictionary.ContainsKey(eventName) && eventDictionary[eventName] != null) {
            eventDictionary[eventName] -= function;
        }
    }

    public static void InvokeEvent(string eventName) {
        eventDictionary[eventName]?.Invoke();
    }
} 