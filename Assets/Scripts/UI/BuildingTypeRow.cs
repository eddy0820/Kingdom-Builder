using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;

public class BuildingTypeRow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] Transform objectButtonsParent;
    [SerializeField] GameObject placeableObjectEntryPrefab;

    [Header("Debug")]
    [ReadOnly, SerializeField] SubBuildingTypes subBuildingType;
    [ReadOnly, SerializeField] List<GridPlaceableObjectSO> placeableObjects;

    public void Init(SubBuildingTypes _subBuildingType, List<GridPlaceableObjectSO> _placeableObjects)
    {
        subBuildingType = _subBuildingType;
        placeableObjects = _placeableObjects;

        if(subBuildingType == SubBuildingTypes.None)
        {
            titleText.gameObject.SetActive(false);
        }
        else
        {
            titleText.text = RandomUtilStatic.AddSpacesToString(subBuildingType.ToString(), false);
        }

        foreach(GridPlaceableObjectSO gridPlaceableObject in placeableObjects)
        {
            PlaceableObjectEntry placeableObjectEntry = Instantiate(placeableObjectEntryPrefab, objectButtonsParent.position, Quaternion.identity, objectButtonsParent).GetComponent<PlaceableObjectEntry>();
            placeableObjectEntry.Init(gridPlaceableObject);
        }
    }
}
