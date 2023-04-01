using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using TMPro;
using UnityEngine.Events;

public class BuildingTypeRowInterface : ButtonInterface<BuildingTypeRowInterface.BuildingTypeRowButtonEntry>
{
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] Transform objectButtonsParent;
    [SerializeField] GameObject placeableObjectEntryPrefab;

    [Header("Debug")]
    [ReadOnly, SerializeField] SubBuildingTypes subBuildingType;
    [ReadOnly, SerializeField] List<GridPlaceableObjectSO> placeableObjects;

    protected override void OnAwake() 
    {
        InputManager.Instance.OnNumberKeyPressed.AddListener(OnNumberKeyPressedCallback);
    }

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
            buttons.Add(new BuildingTypeRowButtonEntry(placeableObjectEntry.gameObject, gridPlaceableObject));
        }

        base.OnAwake();
    }

    public override void OnSelectButton(ButtonEntry buttonEntry)
    {
        BuildingTypeRowButtonEntry entry = (BuildingTypeRowButtonEntry) buttonEntry;
        base.OnSelectButton(buttonEntry);

        GridBuildingManager.Instance.SelectPlaceableObject(entry.GridPlaceableObjectSO);
        PlayerController.Instance.UICanvas.ToggleBuildMenu(false);
        PlayerController.Instance.UICanvas.BuildHotbarInterface.DeselectAllEntries();
    }

    private void OnNumberKeyPressedCallback(int key)
    {
        if(PlayerController.Instance.UICanvas.BuildMenuEnabled)
        {
            foreach(BuildingTypeRowButtonEntry entry in buttons)
            {
                if(entry.IsHovered)
                {
                    BuildHotbarInterface.BuildHotBarEntry buildHotBarEntry = PlayerController.Instance.UICanvas.BuildHotbarInterface.GetEntryFromNumberKey(key);
                    buildHotBarEntry.SetPlaceableObjectSO(entry.GridPlaceableObjectSO);
                }
            }
        } 
    }

    [System.Serializable]
    public class BuildingTypeRowButtonEntry : ButtonEntry
    {
        [SerializeField, ReadOnly] GridPlaceableObjectSO gridPlaceableObjectSO;
        public GridPlaceableObjectSO GridPlaceableObjectSO => gridPlaceableObjectSO;



        public BuildingTypeRowButtonEntry(GameObject _button, GridPlaceableObjectSO _gridPlaceableObjectSO)
        {
            button = _button;
            gridPlaceableObjectSO = _gridPlaceableObjectSO;
        }
    }
}
