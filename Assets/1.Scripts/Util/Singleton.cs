using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    var obj = new GameObject("InstancedSingleton");
                    DontDestroyOnLoad(obj);
                    instance = obj.AddComponent<T>();
                    
                }
            }
            return instance;
        }
    }

    protected void Start()
    {
        if(instance != null)
        {
            if(instance.gameObject != this.gameObject)
            {
                DestroyImmediate(this.gameObject);
            }
        }
        else
        {
            instance = this.GetComponent<T>();
            DontDestroyOnLoad(instance);
        }
        OnStart();
    }
    protected void Awake()
    {
        OnAwake();
    }


    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }
}
