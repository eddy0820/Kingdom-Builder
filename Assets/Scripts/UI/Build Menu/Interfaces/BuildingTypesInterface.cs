using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using NaughtyAttributes;

public class BuildingTypesInterface : ButtonInterface<BuildingTypesInterface.SelectionButtonEntry>
{
    [Header("Colors")]
    [SerializeField] Color defaultButtonColor;
    [SerializeField] Color selectedButtonColor;
    [SerializeField] Color hoveredButtonColor;
    static Color defaultButtonColorStatic;
    static Color selectedButtonColorStatic;
    static Color hoveredButtonColorStatic;

    [Header("Prefabs")]
    [SerializeField] GameObject buildingTypeButtonPrefab;
    [SerializeField] GameObject buildingTypeScreenPrefab;

    [Header("Settings")]
    [SerializeField] float defaultScaleFactor = 1.0f;
    [SerializeField] float hoveredScaleFactor = 1.2f;
    [SerializeField] float scaleLength = 0.2f;
    static float defaultScaleFactorStatic;
    static float hoveredScaleFactorStatic;
    static float scaleLengthStatic;


    public void Init(List<PlaceableGridObjectSO> placeableObjects, RectTransform subScreensParent)
    {
        defaultButtonColorStatic = defaultButtonColor;
        selectedButtonColorStatic = selectedButtonColor;
        hoveredButtonColorStatic = hoveredButtonColor;

        defaultScaleFactorStatic = defaultScaleFactor;
        hoveredScaleFactorStatic = hoveredScaleFactor;
        scaleLengthStatic = scaleLength;

        foreach(BuildingTypesSO buildingTypeSO in PlayerSpawner.Instance.GridBuildingInfo.BuildingTypesDatabase.BuildingTypes)
        {
            GameObject button = Instantiate(buildingTypeButtonPrefab, buildingTypeButtonPrefab.transform.position, buildingTypeButtonPrefab.transform.rotation, transform);
            GameObject screen = Instantiate(buildingTypeScreenPrefab, subScreensParent.position, buildingTypeScreenPrefab.transform.rotation, subScreensParent);

            buttons.Add(new SelectionButtonEntry(button, screen, buildingTypeSO.BuildingType, buildingTypeSO.Icon));

            screen.GetComponent<BuildingTypeScreen>().Init(buildingTypeSO.BuildingType, placeableObjects);
        }

        base.OnAwake();

        DeselectAllButtons();
        Buttons[0].SetIsSelected(true);
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
            ((SelectionButtonEntry) buttonEntry).SetIsSelected(false);
        }
    }


    [System.Serializable]
    public class SelectionButtonEntry : ButtonEntry
    {
        [SerializeField] GameObject screen;
        public GameObject Screen => screen;

        [SerializeField] BuildingTypes buildingType;
        public BuildingTypes BuildingType => buildingType;

        Image iconImage;
        Image buttonImage;

        public SelectionButtonEntry(GameObject _button, GameObject _screen, BuildingTypes _buildingType, Sprite _icon)
        {
            button = _button;
            screen = _screen;
            buildingType = _buildingType;

            iconImage = button.transform.GetChild(0).GetComponent<Image>();
            iconImage.sprite = _icon;

            buttonImage = button.GetComponent<Image>();
        }

        public override void SetIsHovered(bool b)
        {
            base.SetIsHovered(b);

            iconImage.transform.DOKill();

            if(b)
            {
                iconImage.transform.DOScale(hoveredScaleFactorStatic, scaleLengthStatic);
                PlayerController.Instance.SoundController.PlayerHoverButtonSound();
                buttonImage.color = hoveredButtonColorStatic;
            }
            else
            {
                iconImage.transform.DOScale(defaultScaleFactorStatic, scaleLengthStatic);
                buttonImage.color = defaultButtonColorStatic;
            }
        }

        public void SetIsSelectedWithSound(bool b)
        {
            base.SetIsSelected(b);

            iconImage.transform.DOKill();

            iconImage.transform.DOScale(defaultScaleFactorStatic, scaleLengthStatic);

            if(b) 
            {
                screen.SetActive(true);
                PlayerController.Instance.SoundController.PlayClickBuildingTypeButtonSound();
                buttonImage.color = selectedButtonColorStatic;
            }
            else
            {
                screen.SetActive(false);
                buttonImage.color = defaultButtonColorStatic;
            }
        }

        public override void SetIsSelected(bool b)
        {
            base.SetIsSelected(b);

            iconImage.transform.DOKill();

            iconImage.transform.DOScale(defaultScaleFactorStatic, scaleLengthStatic);

            if(b) 
            {
                screen.SetActive(true);
                buttonImage.color = selectedButtonColorStatic;
            }
            else
            {
                screen.SetActive(false);
                buttonImage.color = defaultButtonColorStatic;
            }
        }
    }
}
