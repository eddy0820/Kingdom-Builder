using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Edge Object", menuName = "Building System/Placeable Objects/Edge Object")]
public class EdgeObjectSO : PlaceableGridObjectSO
{
    [Space(15)]
    [SerializeField] EdgeWidth width;
    public EdgeWidth Width => width;

    [Space(10)]
    [SerializeField] List<BuildingTypes> compatibleBuildingTypes;
    public List<BuildingTypes> CompatibleBuildingTypes => compatibleBuildingTypes;

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
