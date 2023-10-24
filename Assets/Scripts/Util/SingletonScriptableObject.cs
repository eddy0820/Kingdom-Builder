using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class SingletonScriptableObject<T> : ScriptableObject where T: SingletonScriptableObject<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                T[] assets = Resources.LoadAll<T>("SingletonScriptableObjects");

                if(assets == null || assets.Length < 1)
                {
                    throw new System.Exception("Culd not find a " + typeof(T).Name + " asset in the Resources/SingletonScriptableObjects folder");
                }
                else if(assets.Length > 1)
                {
                    Debug.LogWarning("Found more than one " + typeof(T).Name + " asset in the Resources/SingletonScriptableObjects folder. Using the first one found.");
                }

                instance = assets[0];
            }

            return instance;
        }
    }
}
