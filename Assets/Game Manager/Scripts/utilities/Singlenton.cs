using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singlenton<T> : MonoBehaviour where T : Singlenton<T>
{ 
    private static T instance;
    public static T Instance
    {
        get {return instance;}
    }

    public static bool isInitialized
    {
        get {return instance != null;}
    }
    protected virtual void Awake() 
    {
        if(instance != null)
        {
            Debug.LogError("[Singleton] Trying to instantiate a sceond instance of a singleton class.");
        } else 
        {
            instance = (T) this;
        }
    }
    protected virtual void OnDestroy() 
    {
        if(instance == this)
        {
            instance = null;
        }
    }
}
