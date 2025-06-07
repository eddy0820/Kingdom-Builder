using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinSubsectionInterface : ButtonInterface<SkinSubsectionInterface.SkinColorButtonEntry>
{
    [Header("Defaults")]
    [SerializeField] GameObject defaultSelectedColorButton;

    [Range(0, 1)]
    [SerializeField] float stubbleColorSubtractionValue;
    [SerializeField] ScarIntensitySubsectionInterface scarIntensitySubsectionInterface;

    SkinColorButtonEntry currentlySelectedButton;
    public SkinColorButtonEntry CurrentlySelectedButton => currentlySelectedButton;

    private void Awake()
    {
        base.OnAwake();

        foreach(SkinColorButtonEntry buttonEntry in Buttons)
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

        Color stubbleColor = new(((SkinColorButtonEntry) buttonEntry).Color.r - stubbleColorSubtractionValue, ((SkinColorButtonEntry) buttonEntry).Color.g - stubbleColorSubtractionValue, ((SkinColorButtonEntry) buttonEntry).Color.b - stubbleColorSubtractionValue, 255);
        Color scarColor = new(((SkinColorButtonEntry) buttonEntry).Color.r - scarIntensitySubsectionInterface.ScarColorSubtractionValue, ((SkinColorButtonEntry) buttonEntry).Color.g - scarIntensitySubsectionInterface.ScarColorSubtractionValue, ((SkinColorButtonEntry) buttonEntry).Color.b - scarIntensitySubsectionInterface.ScarColorSubtractionValue, 255);

        CharacterCreationManager.Instance.MaleInstancedCharacterMaterial.SetColor("_Color_Skin", ((SkinColorButtonEntry) buttonEntry).Color);
        CharacterCreationManager.Instance.MaleInstancedCharacterMaterial.SetColor("_Color_Stubble", stubbleColor);
        CharacterCreationManager.Instance.MaleInstancedCharacterMaterial.SetColor("_Color_Scar", scarColor);

        CharacterCreationManager.Instance.FemaleInstancedCharacterMaterial.SetColor("_Color_Skin", ((SkinColorButtonEntry) buttonEntry).Color);
        CharacterCreationManager.Instance.FemaleInstancedCharacterMaterial.SetColor("_Color_Scar", scarColor);

        if(CharacterCreationManager.Instance.PlayerInfoHolder.PlayerSex == PlayerInfoHolder.Sex.Male)
        {
            CharacterCreationManager.Instance.PlayerInfoHolder.PlayerSkinColors.SetSkinColors(((SkinColorButtonEntry) buttonEntry).Color, stubbleColor, scarColor);
        }
        else
        {
            CharacterCreationManager.Instance.PlayerInfoHolder.PlayerSkinColors.SetSkinColors(((SkinColorButtonEntry) buttonEntry).Color, CharacterCreationManager.Instance.FemaleInstancedCharacterMaterial.GetColor("_Color_Stubble"), scarColor);
        }
        

        buttonEntry.Button.GetComponent<Image>().enabled = true;
        currentlySelectedButton = (SkinColorButtonEntry) buttonEntry;


    }

    private void DeselectAllButtons()
    {
        foreach(ButtonEntry button in Buttons)
        {
            button.Button.GetComponent<Image>().enabled = false;
        }
    }

    [System.Serializable]
    public class SkinColorButtonEntry : ButtonEntry 
    {
        [SerializeField] Color color;
        public Color Color => color;
    }
}
