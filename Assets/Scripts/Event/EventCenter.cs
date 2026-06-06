using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public static class EventCenter
{
    private static Dictionary<Type,Delegate> _eventsDictionary = new Dictionary<Type,Delegate>();
    public static void AddListener<T>(Action<T> handler) where T: class
    {
        if(handler == null)  throw new ArgumentNullException("handler");
        var type = typeof(T);
        if(_eventsDictionary.TryGetValue(type,out Delegate existingDelegate))
        {
            existingDelegate = Delegate.Combine(existingDelegate,handler);
            _eventsDictionary[type] = existingDelegate;
        }
        else
        {
            _eventsDictionary[type] = handler;
        }
    }
    public static void RemoveListener<T>(Action<T> handler) where T: class
    {
        if(handler == null)  throw new ArgumentNullException("handler");
        var type = typeof(T);
        if(_eventsDictionary.TryGetValue(type,out Delegate existingDelegate))
        {
            var newDelegate = Delegate.Remove(existingDelegate,handler);
            if(newDelegate == null)
            {
                _eventsDictionary.Remove(type);
            }
            else
            {
                _eventsDictionary[type] = newDelegate;
            }
        }
    }
    public static void Broadcast<T>(T message) where T : class
    {
        if(message == null)  throw new ArgumentNullException(nameof(message));
        if(_eventsDictionary.TryGetValue(typeof(T),out Delegate delegateToInvoke))
        {
            (delegateToInvoke as Action<T>).Invoke(message);
        }
    }
}
