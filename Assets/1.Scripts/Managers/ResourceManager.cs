using System.Collections.Generic;
using UnityEngine;

public class ResourceManager<T> where T : Object
{
    private Dictionary<string,T> effectDict = new Dictionary<string,T>();
    private string path;

    
    public ResourceManager(string path)
    {
        this.path = path;
    }

    private void InitEffectDict()
    {
        T[] resources = Resources.LoadAll<T>($"{path}");
        foreach ( T resource in resources )
        {
            effectDict.Add(resource.name, resource);
        }
    }

    public T GetResource(string resourceName)
    {
        if (effectDict.Count == 0)
            InitEffectDict();

        return effectDict[resourceName];
    }
}
