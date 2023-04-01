using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public abstract class PlaceableObjectSO : ScriptableObject
{
    [SerializeField] new string name;
    public string Name => name;
    [SerializeField] Transform prefab;
    public Transform Prefab => prefab;
    [SerializeField] Transform visual;
    public Transform Visual => visual;

    [Space(15)]
    [ReadOnly, SerializeField] protected PlaceableObjectTypes objectType;
    public PlaceableObjectTypes ObjectType => objectType;
    [ReadOnly, SerializeField] protected BuildingCategoryTypes buildingCategoryType;
    public BuildingCategoryTypes BuildingCategoryType => buildingCategoryType;

    [Space(15)]
    [SerializeField] Sprite UIIcon;
    public Sprite UIICon => UIIcon;

    private void Awake()
    {
        SetObjectType();
        SetBuildingCategoryType();
    }

    protected abstract void SetObjectType();
    protected abstract void SetBuildingCategoryType();
}
