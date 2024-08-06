using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using EddyLib.Util;

public class BuildingCategoryInterface : ButtonInterface<BuildingCategoryInterface.SelectionButtonEntry>
{
    [Header("Colors")]
    [SerializeField] Color defaultButtonColor;
    [SerializeField] Color selectedButtonColor;
    [SerializeField] Color hoveredButtonColor;
    static Color defaultButtonColorStatic;
    static Color selectedButtonColorStatic;
    static Color hoveredButtonColorStatic;

    [Header("Scale")]
    [SerializeField] float defaultScaleFactor = 1.0f;
    [SerializeField] float hoveredScaleFactor = 1.2f;
    [SerializeField] float scaleLength = 0.2f;
    static float defaultScaleFactorStatic;
    static float hoveredScaleFactorStatic;
    static float scaleLengthStatic;

    [Header("Prefab Parents")]
    [SerializeField] RectTransform buildingCategoryScreenParent;

    [Header("Prefabs")]
    [SerializeField] GameObject buildingCategoryButtonPrefab;
    [SerializeField] GameObject buildingCategoryScreenPrefab;

    protected override void OnAwake()
    {
        defaultScaleFactorStatic = defaultScaleFactor;
        hoveredScaleFactorStatic = hoveredScaleFactor;
        scaleLengthStatic = scaleLength;

        defaultButtonColorStatic = defaultButtonColor;
        selectedButtonColorStatic = selectedButtonColor;
        hoveredButtonColorStatic = hoveredButtonColor;

        CreatePlaceableObjectsList(out List<PlaceableGridObjectSO> houseBuildingPlaceableObjects, out List<PlaceableLooseObjectSO> looseObjects);

        foreach(int i in Enum.GetValues(typeof(BuildingCategoryTypes)))
        {
            GameObject button = Instantiate(buildingCategoryButtonPrefab, buildingCategoryButtonPrefab.transform.position, buildingCategoryButtonPrefab.transform.rotation, transform);
            GameObject screen = Instantiate(buildingCategoryScreenPrefab, buildingCategoryScreenParent.position, buildingCategoryScreenPrefab.transform.rotation, buildingCategoryScreenParent);
            string newName = StringUtil.AddSpaces(Enum.GetName(typeof(BuildingCategoryTypes), i), true);
            button.GetComponentInChildren<TextMeshProUGUI>().text = newName;

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

        DeselectAllButtons();
        buttons[0].SetIsSelected(true);
    }

    private void CreatePlaceableObjectsList(out List<PlaceableGridObjectSO> houseBuildingPlaceableObjects, out List<PlaceableLooseObjectSO> looseObjects)
    {
        houseBuildingPlaceableObjects = new List<PlaceableGridObjectSO>();
        looseObjects = new List<PlaceableLooseObjectSO>();

        foreach(PlaceableObjectSO placeableObjectSO in PlayerSpawner.Instance.GridBuildingInfo.PlaceableObjectsDatabase.PlaceableObjects)
        {;
            switch(placeableObjectSO.BuildingCategoryType)
            {
                case BuildingCategoryTypes.HouseBuilding:
                    houseBuildingPlaceableObjects.Add((PlaceableGridObjectSO) placeableObjectSO);
                break;

                case BuildingCategoryTypes.Props:
                    looseObjects.Add((PlaceableLooseObjectSO) placeableObjectSO);
                break;
            }
        }
    }

    public override void OnSelectButton(ButtonEntry buttonEntry)
    {
        DeselectAllButtons();
        ((SelectionButtonEntry) buttonEntry).SetIsSelectedWithSound(true);
    }

    private void DeselectAllButtons()
    {
        foreach(ButtonEntry buttonEntry in Buttons)
        {
            buttonEntry.SetIsSelected(false);
        }
    }

    [System.Serializable]
    public class SelectionButtonEntry : ButtonEntry
    {
        [SerializeField] GameObject screen;
        public GameObject Screen => screen;

        TextMeshProUGUI text;

        public SelectionButtonEntry(GameObject _button, GameObject _screen)
        {
            button = _button;
            screen = _screen;

            text = button.GetComponentInChildren<TextMeshProUGUI>();
        }

        public override void SetIsHovered(bool b)
        {
            base.SetIsHovered(b);

            if(b)
            {
                button.GetComponent<Image>().color = hoveredButtonColorStatic;
                PlayerController.Instance.SoundController.PlayerHoverButtonSound();
                text.transform.DOScale(hoveredScaleFactorStatic, scaleLengthStatic);
            }
            else
            {
                button.GetComponent<Image>().color = defaultButtonColorStatic;
                text.transform.DOScale(defaultScaleFactorStatic, scaleLengthStatic);
            }
        }

        public void SetIsSelectedWithSound(bool b)
        {
            base.SetIsSelected(b);

            text.transform.DOKill();

            text.transform.DOScale(defaultScaleFactorStatic, scaleLengthStatic);

            if(b)
            {
                button.GetComponent<Image>().color = selectedButtonColorStatic;
                screen.SetActive(true);
                PlayerController.Instance.SoundController.PlayClickBuildingCategoryButtonSound();
            }
            else
            {
                button.GetComponent<Image>().color = defaultButtonColorStatic;
                screen.SetActive(false);
            }
        }

        public override void SetIsSelected(bool b)
        {
            base.SetIsSelected(b);

            text.transform.DOKill();

            text.transform.DOScale(defaultScaleFactorStatic, scaleLengthStatic);

            if(b)
            {
                button.GetComponent<Image>().color = selectedButtonColorStatic;
                screen.SetActive(true);
            }
            else
            {
                button.GetComponent<Image>().color = defaultButtonColorStatic;
                screen.SetActive(false);
            }
        }
    }
}
