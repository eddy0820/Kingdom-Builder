using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingTypesInterface : ButtonInterface<BuildingTypesInterface.SelectionButtonEntry>
{
    [Header("Colors")]
    [SerializeField] Color defaultButtonColor;
    public Color DefaultButtonColor => defaultButtonColor;
    [SerializeField] Color selectedButtonColor;

    public void Init(GameObject buildingTypeButtonPrefab, GameObject buildingTypeScreenPrefab, RectTransform subScreensParent)
    {
        foreach(BuildingTypesSO buildingTypeSO in PlayerSpawner.Instance.GridBuildingInfo.BuildingTypesDatabase.BuildingTypes)
        {
            GameObject button = Instantiate(buildingTypeButtonPrefab, buildingTypeButtonPrefab.transform.position, buildingTypeButtonPrefab.transform.rotation, transform);
            GameObject screen = Instantiate(buildingTypeScreenPrefab, subScreensParent.position, buildingTypeScreenPrefab.transform.rotation, subScreensParent);

            button.GetComponentInChildren<TextMeshProUGUI>().text = buildingTypeSO.Name;
            screen.GetComponentInChildren<TextMeshProUGUI>().text = buildingTypeSO.Name;

            buttons.Add(new SelectionButtonEntry(button, screen));
        }

        base.OnAwake();

        foreach(SelectionButtonEntry buttonEntry in Buttons)
        {
            buttonEntry.Button.GetComponent<Image>().color = defaultButtonColor;
            buttonEntry.Screen.SetActive(false);
        }

        OnSelectButton(Buttons[0]);
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
