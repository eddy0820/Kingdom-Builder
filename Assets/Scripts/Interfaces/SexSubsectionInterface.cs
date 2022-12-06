using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SexSubsectionInterface : ButtonInterface<SexSubsectionInterface.BodySectionButtonEntry>
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

        foreach(BodySectionButtonEntry buttonEntry in Buttons)
        {
            buttonEntry.Button.GetComponent<Image>().color = defaultButtonColor;
        }

        foreach(BodySectionButtonEntry buttonEntry in Buttons)
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
        base.OnSelectButton(buttonEntry);
        buttonEntry.Button.GetComponent<Image>().color = selectedButtonColor;
    }

    private void DeselectAllButtons()
    {
        foreach(ButtonEntry buttonEntry in Buttons)
        {
            buttonEntry.Button.GetComponent<Image>().color = defaultButtonColor;
        }
    }

    public void ChangeSex(int sex)
    {
        if(sex == 0)
        {
            CharacterCreationManager.Instance.FemaleModel.gameObject.SetActive(false);
            CharacterCreationManager.Instance.MaleModel.gameObject.SetActive(true);
            CharacterCreationManager.Instance.PlayerInfoHolder.SetSex(PlayerInfoHolder.Sex.Male);
        }
        else
        {
            CharacterCreationManager.Instance.MaleModel.gameObject.SetActive(false);
            CharacterCreationManager.Instance.FemaleModel.gameObject.SetActive(true);
            CharacterCreationManager.Instance.PlayerInfoHolder.SetSex(PlayerInfoHolder.Sex.Female);
        }
    }

    [System.Serializable]
    public class BodySectionButtonEntry : ButtonEntry {}
}


