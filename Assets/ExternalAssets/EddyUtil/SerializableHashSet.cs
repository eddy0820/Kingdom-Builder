using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[System.Serializable]
public class SerializableHashSet<T> : ISerializationCallbackReceiver
{
    [ReadOnly, SerializeField] List<T> _list = new List<T>();
    public HashSet<T> set = new HashSet<T>();

    public void OnBeforeSerialize()
    {
        _list.Clear();
        foreach(T entry in set)
        {
            _list.Add(entry);
        }
    }

    public void OnAfterDeserialize() {}
}
