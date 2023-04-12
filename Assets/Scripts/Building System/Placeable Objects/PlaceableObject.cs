using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlaceableObject : MonoBehaviour
{
    [SerializeField] protected Transform centerPivot;
    public Transform CenterPivot => centerPivot;

    protected PlaceableObjectTypes objectType;
    public PlaceableObjectTypes ObjectType => objectType;

    protected void Awake()
    {
        objectType = GetObjectType();
    }

    public abstract void DestroySelf();

    protected abstract PlaceableObjectTypes GetObjectType();
}
