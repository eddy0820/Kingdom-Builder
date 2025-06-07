using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnderwearColorSubsectionInterface : ButtonInterface<UnderwearColorSubsectionInterface.UnderwearColorButtonEntry>
{
    [Header("Defaults")]
    [SerializeField] GameObject defaultSelectedColorButton;

    UnderwearColorButtonEntry currentlySelectedButton;
    public UnderwearColorButtonEntry CurrentlySelectedButton => currentlySelectedButton;

    private void Awake()
    {
        base.OnAwake();

        foreach(UnderwearColorButtonEntry buttonEntry in Buttons)
        {
            if(buttonEntry.Button == defaultSelectedColorButton)
            {
                OnSelectButton(buttonEntry);
            }
        }
    }

    public override void OnSelectButton(ButtonEntry buttonEntry)
    {
        DeselectAllButtons();
        CharacterCreationManager.Instance.MaleInstancedCharacterMaterial.SetColor("_Color_Leather_Secondary", ((UnderwearColorButtonEntry) buttonEntry).Color);
        CharacterCreationManager.Instance.FemaleInstancedCharacterMaterial.SetColor("_Color_Leather_Secondary", ((UnderwearColorButtonEntry) buttonEntry).Color);
        
        buttonEntry.Button.GetComponent<Image>().enabled = true;
        currentlySelectedButton = (UnderwearColorButtonEntry) buttonEntry;
        CharacterCreationManager.Instance.PlayerInfoHolder.SetUnderwearColor(((UnderwearColorButtonEntry) buttonEntry).Color);
    }

    private void DeselectAllButtons()
    {
        foreach(ButtonEntry button in Buttons)
        {
            button.Button.GetComponent<Image>().enabled = false;
        }
    }

    [System.Serializable]
    public class UnderwearColorButtonEntry : ButtonEntry 
    {
        [SerializeField] Color color;
        public Color Color => color;
    }
}
