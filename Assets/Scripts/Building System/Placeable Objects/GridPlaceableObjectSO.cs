using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridPlaceableObjectSO : PlaceableObjectSO
{
    [SerializeField] MaterialBuildingTypes materialBuildingType;
    public MaterialBuildingTypes MaterialBuildingType => materialBuildingType;
    [SerializeField] BuildingTypes buildingType;
    public BuildingTypes BuildingType => buildingType;
    [SerializeField] SubBuildingTypes subBuildingType;
    public SubBuildingTypes SubBuildingType => subBuildingType;
}
