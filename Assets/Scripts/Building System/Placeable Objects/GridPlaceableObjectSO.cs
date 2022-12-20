using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridPlaceableObjectSO : PlaceableObjectSO
{
    [SerializeField] MaterialBuildingTypes materialBuildingType;
    public MaterialBuildingTypes MaterialBuildingType => materialBuildingType;
    [SerializeField] BuildingTypes buildingType;
    public BuildingTypes BuildingTypes => buildingType;
    [SerializeField] SubBuildingTypes subBuildingType;
    public SubBuildingTypes SubBuildingType => subBuildingType;
}
