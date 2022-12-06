using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterCreationSectionInterface : ButtonInterface<CharacterCreationSectionInterface.SelectionButtonEntry>
{
    [Header("Defaults")]
    [SerializeField] GameObject defaultSelectedButton;

    [Header("Colors")]
    [SerializeField] Color defaultButtonColor;
    public Color DefaultButtonColor => defaultButtonColor;
    [SerializeField] Color selectedButtonColor;

    protected override void OnAwake()
    {
        base.OnAwake();

        foreach(SelectionButtonEntry buttonEntry in Buttons)
        {
            buttonEntry.Button.GetComponent<Image>().color = defaultButtonColor;
        }

        foreach(SelectionButtonEntry buttonEntry in Buttons)
        {
            if(buttonEntry.Button == defaultSelectedButton)
            {
                OnSelectButton(buttonEntry);
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
    }
}




