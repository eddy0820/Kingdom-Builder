using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingCategoryInterface : ButtonInterface<BuildingCategoryInterface.SelectionButtonEntry>
{
    [Header("Colors")]
    [SerializeField] Color defaultButtonColor;
    public Color DefaultButtonColor => defaultButtonColor;
    [SerializeField] Color selectedButtonColor;

    [Header("Prefab Parents")]
    [SerializeField] RectTransform buildingCategoryScreenParent;

    [Header("Prefabs")]
    [SerializeField] GameObject buildingCategoryButtonPrefab;
    [SerializeField] GameObject buildingCategoryScreenPrefab;

    protected override void OnAwake()
    {
        CreatePlaceableObjectsList(out List<GridPlaceableObjectSO> houseBuildingPlaceableObjects, out List<LooseObjectSO> looseObjects);

        foreach(int i in Enum.GetValues(typeof(BuildingCategoryTypes)))
        {
            GameObject button = Instantiate(buildingCategoryButtonPrefab, buildingCategoryButtonPrefab.transform.position, buildingCategoryButtonPrefab.transform.rotation, transform);
            GameObject screen = Instantiate(buildingCategoryScreenPrefab, buildingCategoryScreenParent.position, buildingCategoryScreenPrefab.transform.rotation, buildingCategoryScreenParent);
            string newName = RandomUtilStatic.AddSpacesToString(Enum.GetName(typeof(BuildingCategoryTypes), i), true);
            button.GetComponentInChildren<TextMeshProUGUI>().text = newName;
            //screen.GetComponentInChildren<TextMeshProUGUI>().text = newName;
            buttons.Add(new SelectionButtonEntry(button, screen));

            switch((BuildingCategoryTypes) i)
            {
                case BuildingCategoryTypes.HouseBuilding:
                    screen.GetComponent<BuildingCategoryScreen>().HouseBuildingInit(houseBuildingPlaceableObjects);
                break;

                case BuildingCategoryTypes.Props:
                    screen.GetComponent<BuildingCategoryScreen>().PropInit(looseObjects);
                break;
            }
            
        }

        base.OnAwake();

        foreach(SelectionButtonEntry buttonEntry in Buttons)
        {
            buttonEntry.Button.GetComponent<Image>().color = defaultButtonColor;
            buttonEntry.Screen.SetActive(false);
        }

        OnSelectButton(Buttons[0]);
    }

    private void CreatePlaceableObjectsList(out List<GridPlaceableObjectSO> houseBuildingPlaceableObjects, out List<LooseObjectSO> looseObjects)
    {
        houseBuildingPlaceableObjects = new List<GridPlaceableObjectSO>();
        looseObjects = new List<LooseObjectSO>();

        foreach(PlaceableObjectSO placeableObjectSO in PlayerSpawner.Instance.GridBuildingInfo.PlaceableObjectsDatabase.PlaceableObjects)
        {
            switch(placeableObjectSO.BuildingCategoryType)
            {
                case BuildingCategoryTypes.HouseBuilding:
                    houseBuildingPlaceableObjects.Add((GridPlaceableObjectSO) placeableObjectSO);
                break;

                case BuildingCategoryTypes.Props:
                    looseObjects.Add((LooseObjectSO) placeableObjectSO);
                break;
            }
        }
    }

    public override void OnSelectButton(ButtonEntry buttonEntry)
    {
        DeselectAllButtons();
        buttonEntry.Button.GetComponent<Image>().color = selectedButtonColor;
        ((SelectionButtonEntry) buttonEntry).Screen.SetActive(true);
    }

    private void DeselectAllButtons()
    {
        foreach(ButtonEntry buttonEntry in Buttons)
        {
            buttonEntry.Button.GetComponent<Image>().color = defaultButtonColor;
            ((SelectionButtonEntry) buttonEntry).Screen.SetActive(false);
        }
    }

    [System.Serializable]
    public class SelectionButtonEntry : ButtonEntry
    {
        [SerializeField] GameObject screen;
        public GameObject Screen => screen;

        public SelectionButtonEntry(GameObject _button, GameObject _screen)
        {
            button = _button;
            screen = _screen;
        }
    }
}