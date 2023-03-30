using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class BuildingTypeScreen : MonoBehaviour
{
    [SerializeField, ReadOnly] BuildingTypes buildingType;
    public BuildingTypes BuildingType => buildingType;
    [SerializeField, ReadOnly] List<GridPlaceableObjectSO> typePlaceableObjects;
    [SerializeField, ReadOnly] HashSet<SubBuildingTypes> possibleSubBuildingTypes;

    public void Init(BuildingTypes _buildingType, List<GridPlaceableObjectSO> placeableObjects)
    {
        buildingType = _buildingType;

        typePlaceableObjects = GetTypePlaceableObjects(placeableObjects);
        possibleSubBuildingTypes = GetPossibleSubBuildingTypes();

        // you are here
    }

    private List<GridPlaceableObjectSO> GetTypePlaceableObjects(List<GridPlaceableObjectSO> placeableObjects)
    {
        List<GridPlaceableObjectSO> newPlaceableObjects = new List<GridPlaceableObjectSO>();

        foreach(GridPlaceableObjectSO gridPlaceableObject in placeableObjects)
        {
            if(gridPlaceableObject.BuildingType is BuildingTypes.Wall)
            {
                newPlaceableObjects.Add(gridPlaceableObject);
            }
        }

        return newPlaceableObjects;
    }

    private HashSet<SubBuildingTypes> GetPossibleSubBuildingTypes()
    {
        HashSet<SubBuildingTypes> subBuildingTypes = new HashSet<SubBuildingTypes>();

        foreach(GridPlaceableObjectSO gridPlaceableObject in typePlaceableObjects)
        {
            subBuildingTypes.Add(gridPlaceableObject.SubBuildingType);
        }

        return subBuildingTypes;
    }
}
