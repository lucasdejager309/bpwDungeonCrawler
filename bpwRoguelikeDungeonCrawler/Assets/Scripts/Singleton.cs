using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour 
{
    private static T instance;
    public static T Instance {
        get {return instance;}
        set {
            if (instance == null) {
                instance = value;
            }
        }
    }
}