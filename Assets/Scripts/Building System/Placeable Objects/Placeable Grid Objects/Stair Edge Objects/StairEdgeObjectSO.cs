using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stair Edge Object", menuName = "Building System/Placeable Objects/Stair Edge Object")]
public class StairEdgeObjectSO : PlaceableGridObjectSO
{
    protected override void SetBuildingCategoryType()
    {
        buildingCategoryType = BuildingCategoryTypes.HouseBuilding;
    }

    protected override void SetObjectType()
    {
        objectType = PlaceableObjectTypes.StairEdgeObject;
    }
}
