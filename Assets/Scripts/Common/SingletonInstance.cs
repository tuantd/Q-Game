using UnityEngine;
using System.Collections;

public class SingletonInstance<T> where T:class,new(){
    protected static T sInstance;
    public static T Instance {
        get {
            if (sInstance == null) {
                sInstance = new T();
                if (sInstance == null)
                    Debug.LogError("An instance of " + typeof(T) + " is needed in the scene, but there is none.");
            }
            return sInstance;
        }
    }
}
