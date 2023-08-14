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
    [SerializeField] bool rotateable;
    public bool Rotateable => rotateable;

    [Space(15)]
    [ReadOnly, SerializeField] protected PlaceableObjectTypes objectType;
    public PlaceableObjectTypes ObjectType => objectType;
    [ReadOnly, SerializeField] protected BuildingCategoryTypes buildingCategoryType;
    public BuildingCategoryTypes BuildingCategoryType => buildingCategoryType;

    [Space(15)]

    [SerializeField] Sprite[] UIIcons;
    public Sprite[] UIICons => UIIcons;
    public Sprite MainIcon => GetFirstIcon();

    [Space(15)]
    [SerializeField] EMaterialSoundType materialSoundType;
    public EMaterialSoundType MaterialSoundType => materialSoundType;

    private void Awake()
    {
        SetObjectType();
        SetBuildingCategoryType();
    }

    private Sprite GetFirstIcon()
    {
        if(UIIcons.Length > 0) return UIIcons[0];
        return null;
    }

    protected abstract void SetObjectType();
    protected abstract void SetBuildingCategoryType();
}
