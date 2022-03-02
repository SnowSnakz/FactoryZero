using FactoryZero.Marching;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

using Object = UnityEngine.Object;

public class SerializableFunction<T, K> where K : UnityEvent<T>
{
    public K impl;
    List<Action<T>> actions;
    bool hasInit;

    public void Init()
    {
        if(!hasInit)
        {
            hasInit = true;

            actions = new List<Action<T>>();
            for (int i = 0; i < impl.GetPersistentEventCount(); i++)
            {
                Object g = impl.GetPersistentTarget(i);
                MethodInfo mi = UnityEventBase.GetValidMethodInfo(g, impl.GetPersistentMethodName(i), new Type[] { typeof(T) });
                actions.Add((Action<T>)mi.CreateDelegate(typeof(Action<T>), g));
            }
        }
    }

    public void Invoke(T arg)
    {
        if(!hasInit) Init();

        foreach (Action<T> action in actions)
        {
            action?.Invoke(arg);
        }
    }
    
    public int AddListener(Action<T> action)
    {
        if (!hasInit) Init();

        int idx = actions.Count;
        actions.Add(action);
        return idx;
    }

    public int Count 
    { 
        get
        {
            int nc = 0;
            for(int i = 0; i < actions.Count; i++)
            {
                if (actions[i] != null) nc++;
            }

            return nc;
        }
    }

    public int IndexOf(Action<T> action)
    {
        if (!hasInit) Init();

        return actions.IndexOf(action);
    }

    public void RemoveListener(Action<T> action)
    {
        if (!hasInit) Init();

        int idx = actions.IndexOf(action);
        if(idx != -1)
        {
            RemoveListener(idx);
        }
    }

    public void RemoveListener(int idx)
    {
        if (!hasInit) Init();

        if (idx >= 0 && idx < actions.Count)
        {
            actions[idx] = null;
        }

        bool clearAbove = true;
        for (int i = idx; i < actions.Count; i++)
        {
            if(actions[i] != null)
            {
                clearAbove = false;
            }
        }

        if(clearAbove)
        {
            actions.RemoveRange(idx, actions.Count - idx);
        }
    }
}
