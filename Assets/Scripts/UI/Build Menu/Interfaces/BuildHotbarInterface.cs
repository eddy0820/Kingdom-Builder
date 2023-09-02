using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class BuildHotbarInterface : ButtonInterface<BuildHotbarInterface.BuildHotBarEntry>
{
    PlaceableObjectSO currentPlaceableObjectSO;
    public PlaceableObjectSO CurrentPlaceableObjectSO => currentPlaceableObjectSO;

    protected override void OnAwake()
    {
        InputManager.Instance.OnNumberKeyPressed.AddListener(OnNumberKeyPressedCallback);
    }

    protected override void Initialize()
    {
        UnsetAllEntries();
        DeselectAllEntries();
    }

    private void UnsetAllEntries()
    {
        foreach(BuildHotBarEntry entry in buttons)
        {
            entry.SetPlaceableObjectSO(null);
        }
    }

    public void DeselectAllEntries()
    {
        foreach(ButtonEntry entry in buttons)
        {
            entry.SetIsSelected(false);
        }
    }

    public BuildHotBarEntry GetEntryFromNumberKey(int key)
    {
        foreach(BuildHotBarEntry buildHotBarEntry in buttons)
        {
            if(buildHotBarEntry.KeyCodeInt == key)
            {
                return buildHotBarEntry;
            }
        }

        return null;
    }

    private void OnNumberKeyPressedCallback(int key)
    {
        if(PlayerController.Instance.BuildModeEnabled && !PlayerController.Instance.UICanvas.BuildMenuEnabled)
        {
            foreach(BuildHotBarEntry buildHotBarEntry in buttons)
            {
                if(key == buildHotBarEntry.KeyCodeInt)
                {
                    DeselectAllEntries();
                    buildHotBarEntry.SetIsSelected(true);
                    currentPlaceableObjectSO = buildHotBarEntry.PlaceableObjectSO;
                    GridBuildingManager.Instance.SelectPlaceableObject(currentPlaceableObjectSO);
                    PlayerController.Instance.SoundController.PlaySelectFromHotbarSound();
                }
            }
        }
        
    }

    protected override void OnClick(PointerEventData data, ButtonEntry buttonEntry) {}

    [System.Serializable]
    public class BuildHotBarEntry : ButtonEntry 
    {
        [SerializeField] int keyCodeInt;
        public int KeyCodeInt => keyCodeInt;

        PlaceableObjectSO placeableObjectSO;
        public PlaceableObjectSO PlaceableObjectSO => placeableObjectSO;

        public void SetPlaceableObjectSO(PlaceableObjectSO _placeableObjectSO)
        {
            placeableObjectSO = _placeableObjectSO;

            GameObject subButton = button.transform.GetChild(0).GetChild(0).gameObject;
            
            if(placeableObjectSO != null)
            {
                subButton.GetComponent<Image>().sprite = placeableObjectSO.MainIcon;
                subButton.SetActive(true);

                if(IsSelected)
                {
                    GridBuildingManager.Instance.SelectPlaceableObject(placeableObjectSO);
                }    
            }
            else
            {
                subButton.GetComponent<Image>().sprite = null;
                subButton.SetActive(false);

                if(IsSelected)
                {
                    GridBuildingManager.Instance.SelectPlaceableObject(null);
                } 
            }
        }

        public override void SetIsSelected(bool b)
        {
            base.SetIsSelected(b);

            button.transform.GetChild(0).GetComponent<Image>().enabled = b;

            if(b)
            {
                Transform transformToScale = button.transform.GetChild(0).GetChild(0);
                transformToScale.DOKill();
                transformToScale.DOPunchScale(Vector3.one * 0.2f, 0.1f, 10, 1);
            }
        }
    }
}
