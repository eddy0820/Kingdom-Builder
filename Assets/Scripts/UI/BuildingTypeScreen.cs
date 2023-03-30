using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;

public class BuildingTypeScreen : MonoBehaviour
{
    [SerializeField] GameObject buildingTypeRowPrefab;

    [Space(10)]

    [SerializeField] Transform contentParent;

    [Space(10)]

    [SerializeField, ReadOnly] BuildingTypes buildingType;
    public BuildingTypes BuildingType => buildingType;

    [SerializeField, ReadOnly] List<GridPlaceableObjectSO> typePlaceableObjects;
    [SerializeField, ReadOnly] SerializableHashSet<SubBuildingTypes> possibleSubBuildingTypes;

    public void Init(BuildingTypes _buildingType, List<GridPlaceableObjectSO> placeableObjects)
    {
        buildingType = _buildingType;

        typePlaceableObjects = GetTypePlaceableObjects(placeableObjects);
        possibleSubBuildingTypes.set = GetPossibleSubBuildingTypes();

        List<SubBuildingTypes> possibleSubBuildingTypesList = possibleSubBuildingTypes.set.ToList();
        possibleSubBuildingTypesList.Sort();

        foreach(SubBuildingTypes subBuildingType in possibleSubBuildingTypesList)
        {
            BuildingTypeRow buildingTypeRow = Instantiate(buildingTypeRowPrefab, contentParent.position, Quaternion.identity, contentParent).GetComponent<BuildingTypeRow>();
            buildingTypeRow.Init(subBuildingType, GetSubTypePlaceableObjects(subBuildingType));
        }
        
    }

    private List<GridPlaceableObjectSO> GetTypePlaceableObjects(List<GridPlaceableObjectSO> placeableObjects)
    {
        List<GridPlaceableObjectSO> newPlaceableObjects = new List<GridPlaceableObjectSO>();

        foreach(GridPlaceableObjectSO gridPlaceableObject in placeableObjects)
        {
            if(gridPlaceableObject.BuildingType == buildingType)
            {
                newPlaceableObjects.Add(gridPlaceableObject);
            }
        }

        return newPlaceableObjects;
    }

    private List<GridPlaceableObjectSO> GetSubTypePlaceableObjects(SubBuildingTypes subBuildingType)
    {
        List<GridPlaceableObjectSO> newSubTypePlaceableObjects = new List<GridPlaceableObjectSO>();

        foreach(GridPlaceableObjectSO gridPlaceableObject in typePlaceableObjects)
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

        foreach(GridPlaceableObjectSO gridPlaceableObject in typePlaceableObjects)
        {
            subBuildingTypes.Add(gridPlaceableObject.SubBuildingType);
        }

        return subBuildingTypes;
    }
}
