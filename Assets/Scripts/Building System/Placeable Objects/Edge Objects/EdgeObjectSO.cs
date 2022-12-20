using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Edge Object", menuName = "Building System/Edge Object")]
public class EdgeObjectSO : GridPlaceableObjectSO
{
    [Space(15)]
    [SerializeField] EdgeWidth width;
    public EdgeWidth Width => width;

    protected override void SetBuildingCategoryType()
    {
        buildingCategoryType = BuildingCategoryTypes.HouseBuilding;
    }
    
    protected override void SetObjectType()
    {
        objectType = PlaceableObjectTypes.EdgeObject;
    }

    public enum EdgeWidth
    {
        One,
        Two
    }
}
