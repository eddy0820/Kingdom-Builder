using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairColorSubsectionInterface : MonoBehaviour
{
    [SerializeField] FlexibleColorPicker colorPicker;
    [SerializeField] Color defaultColor;

    private void Start()
    {
        colorPicker.startingColor = defaultColor;
        colorPicker.SetColor(defaultColor);
        CharacterCreationManager.Instance.PlayerInfoHolder.SetHairColor(colorPicker.color);
    }

    public void SwitchColor()
    {
        CharacterCreationManager.Instance.MaleModel.instancedMaterial.SetColor("_Color_Hair", colorPicker.color);
        CharacterCreationManager.Instance.FemaleModel.instancedMaterial.SetColor("_Color_Hair", colorPicker.color);
        CharacterCreationManager.Instance.PlayerInfoHolder.SetHairColor(colorPicker.color);
    }
}
