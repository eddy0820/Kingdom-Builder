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

    protected override void OnEnter(ButtonEntry buttonEntry)
    {
        base.OnEnter(buttonEntry);
        BuildingTypeRowButtonEntry entry = (BuildingTypeRowButtonEntry) buttonEntry;

        if(entry.GridPlaceableObjectSO.UIICons.Length > 0)
        {   
            entry.currentCoroutine = StartCoroutine(AnimateImage(entry.GridPlaceableObjectSO.UIICons, entry.Button.transform.GetChild(0).GetComponent<Image>()));
        }
    }

    protected override void OnExit(ButtonEntry buttonEntry)
    {
        base.OnExit(buttonEntry);
        BuildingTypeRowButtonEntry entry = (BuildingTypeRowButtonEntry) buttonEntry;

        if(entry.GridPlaceableObjectSO.UIICons.Length > 0)
        { 
            StopCoroutine(entry.currentCoroutine);
            entry.Button.transform.GetChild(0).GetComponent<Image>().sprite = entry.GridPlaceableObjectSO.MainIcon;
        }
    }

    IEnumerator AnimateImage(Sprite[] icons, Image component)
    {
        yield return new WaitForSeconds(GridBuildingManager.Instance.UIIconAnimationDelay);

        for(int i = 0; i < icons.Length; i++)
        {
            component.sprite = icons[i];
            yield return new WaitForSeconds(GridBuildingManager.Instance.UIIconAnimationSpeed);

            if(i == icons.Length - 1)
            {
                i = 0;
            }
        }
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

    private void OnDisable()
    {
        foreach(BuildingTypeRowButtonEntry entry in buttons)
        {
            entry.SetIsHovered(false);

            if(entry.GridPlaceableObjectSO.UIICons.Length > 0)
            { 
                if(entry.currentCoroutine != null) StopCoroutine(entry.currentCoroutine);
                entry.Button.transform.GetChild(0).GetComponent<Image>().sprite = entry.GridPlaceableObjectSO.MainIcon;
            }
        }
    }

    [System.Serializable]
    public class BuildingTypeRowButtonEntry : ButtonEntry
    {
        [SerializeField, ReadOnly] GridPlaceableObjectSO gridPlaceableObjectSO;
        public GridPlaceableObjectSO GridPlaceableObjectSO => gridPlaceableObjectSO;

        [HideInInspector]
        public Coroutine currentCoroutine;

        public BuildingTypeRowButtonEntry(GameObject _button, GridPlaceableObjectSO _gridPlaceableObjectSO)
        {
            button = _button;
            gridPlaceableObjectSO = _gridPlaceableObjectSO;
        }
    }
}