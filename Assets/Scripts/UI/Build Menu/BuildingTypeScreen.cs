using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;
using EddyLib.Util.Collections;

public class BuildingTypeScreen : MonoBehaviour
{
    [SerializeField] GameObject buildingTypeRowPrefab;

    [Space(10)]

    [SerializeField] Transform contentParent;

    [Space(10)]

    [SerializeField, ReadOnly] BuildingTypes buildingType;
    public BuildingTypes BuildingType => buildingType;

    [SerializeField, ReadOnly] List<PlaceableGridObjectSO> typePlaceableObjects;
    [SerializeField, ReadOnly] SerializableHashSet<SubBuildingTypes> possibleSubBuildingTypes;

    public void Init(BuildingTypes _buildingType, List<PlaceableGridObjectSO> placeableObjects)
    {
        buildingType = _buildingType;

        typePlaceableObjects = GetTypePlaceableObjects(placeableObjects);
        possibleSubBuildingTypes.Value = GetPossibleSubBuildingTypes();

        List<SubBuildingTypes> possibleSubBuildingTypesList = possibleSubBuildingTypes.Value.ToList();
        possibleSubBuildingTypesList.Sort();

        foreach(SubBuildingTypes subBuildingType in possibleSubBuildingTypesList)
        {
            BuildingTypeRowInterface buildingTypeRow = Instantiate(buildingTypeRowPrefab, contentParent.position, Quaternion.identity, contentParent).GetComponent<BuildingTypeRowInterface>();
            buildingTypeRow.Init(subBuildingType, GetSubTypePlaceableObjects(subBuildingType));
        }
        
    }

    private List<PlaceableGridObjectSO> GetTypePlaceableObjects(List<PlaceableGridObjectSO> placeableObjects)
    {
        List<PlaceableGridObjectSO> newPlaceableObjects = new List<PlaceableGridObjectSO>();

        foreach(PlaceableGridObjectSO gridPlaceableObject in placeableObjects)
        {
            if(gridPlaceableObject.BuildingType == buildingType)
            {
                newPlaceableObjects.Add(gridPlaceableObject);
            }
        }

        return newPlaceableObjects;
    }

    private List<PlaceableGridObjectSO> GetSubTypePlaceableObjects(SubBuildingTypes subBuildingType)
    {
        List<PlaceableGridObjectSO> newSubTypePlaceableObjects = new List<PlaceableGridObjectSO>();

        foreach(PlaceableGridObjectSO gridPlaceableObject in typePlaceableObjects)
        {
            if(gridPlaceableObject.SubBuildingType == subBuildingType)
            {
                newSubTypePlaceableObjects.Add(gridPlaceableObject);
            }
        }

        return newSubTypePlaceableObjects;
    }

    private HashSet<SubBuildingTypes> GetPossibleSubBuildingTypes()
    {
        HashSet<SubBuildingTypes> subBuildingTypes = new HashSet<SubBuildingTypes>();

        foreach(PlaceableGridObjectSO gridPlaceableObject in typePlaceableObjects)
        {
            subBuildingTypes.Add(gridPlaceableObject.SubBuildingType);
        }

        return subBuildingTypes;
    }
}
