using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Loose Object", menuName = "Building System/Placeable Objects/Loose Object")]
public class LooseObjectSO : PlaceableObjectSO
{
    protected override void SetBuildingCategoryType()
    {
        buildingCategoryType = BuildingCategoryTypes.Props;
    }

    protected override void SetObjectType()
    {
        objectType = PlaceableObjectTypes.LooseObject;
    }
}
