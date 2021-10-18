﻿using System;

using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
            if (instance == null)
            {
                GameObject singletonContainer = new GameObject($"{typeof(T).Name} singleton");
                instance = singletonContainer.AddComponent<T>();
            }

            return instance;
        }
        set => instance = value;
    }

    public static T InstanceIfInitialized => IsInitialized ? instance : null;

    public static bool IsInitialized => instance != null;

    protected virtual void Awake()
    {
        if (!IsInitialized)
        {
            Instance = this as T;
        }
        else
        {
            DestroyThis(false);
            throw new Exception($"Duplicate Singleton for '{GetType()}'");
        }
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    /// <summary>
    /// Sets the instance of the singleton to null.
    /// </summary>
    public void DestroyInstance()
    {
        DestroyThis(true);
    }

    private void DestroyThis(bool destroyInstance)
    {
        if (destroyInstance)
        {
            instance = null;
        }

        if (gameObject.name.ToLower().Contains("singleton"))
        {
            Destroy(gameObject);
            return;
        }

        Destroy(this);
    }
}