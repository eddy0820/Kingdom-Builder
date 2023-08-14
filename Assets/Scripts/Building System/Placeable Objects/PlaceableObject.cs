using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public abstract class PlaceableObject : MonoBehaviour
{
    [SerializeField] protected Transform centerPivot;
    public Transform CenterPivot => centerPivot;
    [SerializeField] protected Collider colliderToApplyMaterialSoundTypeHolder;
    public Collider ColliderToApplyMaterialSoundTypeHolder => colliderToApplyMaterialSoundTypeHolder;

    [Space(5)]

    [ReadOnly, SerializeField] protected PlaceableObjectTypes objectType;
    public PlaceableObjectTypes ObjectType => objectType;

    [ReadOnly, SerializeField] protected BuildingTypes buildingType;
    public BuildingTypes BuildingType => buildingType;

    [ReadOnly, SerializeField] protected EMaterialSoundType materialSoundType;
    public EMaterialSoundType MaterialSoundType => materialSoundType; 

    protected void Awake()
    {
        objectType = GetObjectType();

        OnAwake();
    }

    public void SetBuildingType(BuildingTypes _buildingType)
    {
        buildingType = _buildingType;
    }

    public void SetMaterialSoundType(EMaterialSoundType _materialSoundType)
    {
        materialSoundType = _materialSoundType;

       colliderToApplyMaterialSoundTypeHolder.gameObject.AddComponent<MaterialSoundTypeHolder>().SetMaterialSoundType(_materialSoundType);
    }

    public abstract void DestroySelf();

    protected abstract PlaceableObjectTypes GetObjectType();

    protected virtual void OnAwake() {}
}
