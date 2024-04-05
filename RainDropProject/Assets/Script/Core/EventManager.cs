using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    private static EventManager _instance;

    private Dictionary<string, Action<object[]>> _listenersDictionary = new Dictionary<string, Action<object[]>>();

    public static EventManager Instance
    {
        get
        {
            // Se l'istanza non esiste, la crea
            if (_instance == null)
            {
                // Cerca un'istanza esistente nella scena
                _instance = new EventManager();
            }
            return _instance;
        }
    }

    private EventManager() { }

    public void Subscribe(string eventName, params Action<object[]>[] listeners)
    {
        for (int i = 0; i < listeners.Length; i++)
        {
            Action<object[]> listener = listeners[i];
            if (!_listenersDictionary.ContainsKey(eventName))
            {
                _listenersDictionary[eventName] = listener;
            }
            else
            {
                _listenersDictionary[eventName] += listener;
            }
        }
    }

    public void Unsubscribe(string eventName, params Action<object[]>[] listeners)
    {
        for (int i = 0; i < listeners.Length; i++)
        {
            Action<object[]> listener = listeners[i];
            if (!_listenersDictionary.ContainsKey(eventName))
            {
                _listenersDictionary[eventName] = listener;
            }
            else
            {
                _listenersDictionary[eventName] -= listener;
            }
        }
    }

    public void Publish(string eventName, params object[] args)
    {
#if UNITY_EDITOR
        Debug.Log("Event Published: " +  eventName);
#endif
        if (_listenersDictionary.ContainsKey(eventName))
        {
            _listenersDictionary[eventName]?.Invoke(args);
        }
    }
}