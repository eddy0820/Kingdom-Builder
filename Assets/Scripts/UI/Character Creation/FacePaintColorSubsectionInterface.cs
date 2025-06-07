using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePaintColorSubsectionInterface : MonoBehaviour
{
    [SerializeField] FlexibleColorPicker colorPicker;
    [SerializeField] Color defaultColor;

    private void Awake()
    {
        colorPicker.startingColor = defaultColor;
        colorPicker.SetColor(defaultColor);
        
        CharacterCreationManager.Instance.PlayerInfoHolder.SetFacePaintColor(colorPicker.color);
    }

    public void SwitchColor()
    {
        CharacterCreationManager.Instance.MaleInstancedCharacterMaterial.SetColor("_Color_BodyArt", colorPicker.color);
        CharacterCreationManager.Instance.FemaleInstancedCharacterMaterial.SetColor("_Color_BodyArt", colorPicker.color);

        CharacterCreationManager.Instance.PlayerInfoHolder.SetFacePaintColor(colorPicker.color);
    }
}
