using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairColorSubsectionInterface : MonoBehaviour
{
    [SerializeField] FlexibleColorPicker colorPicker;
    [SerializeField] Color defaultColor;

    private void Awake()
    {
        colorPicker.startingColor = defaultColor;
        colorPicker.SetColor(defaultColor);
        CharacterCreationManager.Instance.PlayerInfoHolder.SetHairColor(colorPicker.color);
    }

    public void SwitchColor()
    {
        CharacterCreationManager.Instance.MaleInstancedCharacterMaterial.SetColor("_Color_Hair", colorPicker.color);
        CharacterCreationManager.Instance.FemaleInstancedCharacterMaterial.SetColor("_Color_Hair", colorPicker.color);
        CharacterCreationManager.Instance.PlayerInfoHolder.SetHairColor(colorPicker.color);
    }
}
