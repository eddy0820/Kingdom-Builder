using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlaceableObject : MonoBehaviour
{
    [SerializeField] protected Transform centerPivot;
    public Transform CenterPivot => centerPivot;

    protected PlaceableObjectTypes objectType;
    public PlaceableObjectTypes ObjectType => objectType;

    BuildingTypes buildingType;
    public BuildingTypes BuildingType => buildingType;

    protected void Awake()
    {
        objectType = GetObjectType();
    }

    public void SetBuildingType(BuildingTypes _buildingType)
    {
        buildingType = _buildingType;
    }

    public abstract void DestroySelf();

    protected abstract PlaceableObjectTypes GetObjectType();
}
