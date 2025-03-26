using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSigleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public bool isGlobal;
    private static T _instance;
    private readonly static object _lock = new object();

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));
                    }
                }
            }
            return _instance;
        }
    }

    public virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }

        if (isGlobal)
        {
            DontDestroyOnLoad(this);
        }
    }
}
