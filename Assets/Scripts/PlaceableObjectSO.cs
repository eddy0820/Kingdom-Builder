using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlaceableObjectSO : ScriptableObject
{
    [SerializeField] new string name;
    public string Name => name;
    [SerializeField] Transform prefab;
    public Transform Prefab => prefab;
    [SerializeField] Transform visual;
    public Transform Visual => visual;
    [ReadOnly, SerializeField] protected PlaceableObjectTypes objectType;
    public PlaceableObjectTypes ObjectType => objectType;

    private void Awake()
    {
        SetObjectType();
    }

    protected abstract void SetObjectType();
}