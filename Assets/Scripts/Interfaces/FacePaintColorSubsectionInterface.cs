using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePaintColorSubsectionInterface : MonoBehaviour
{
    [SerializeField] FlexibleColorPicker colorPicker;
    [SerializeField] Color defaultColor;

    private void Start()
    {
        colorPicker.startingColor = defaultColor;
        colorPicker.SetColor(defaultColor);
        
        CharacterCreationManager.Instance.PlayerInfoHolder.SetFacePaintColor(colorPicker.color);
    }

    public void SwitchColor()
    {
        CharacterCreationManager.Instance.MaleModel.instancedMaterial.SetColor("_Color_BodyArt", colorPicker.color);
        CharacterCreationManager.Instance.FemaleModel.instancedMaterial.SetColor("_Color_BodyArt", colorPicker.color);

        CharacterCreationManager.Instance.PlayerInfoHolder.SetFacePaintColor(colorPicker.color);
    }
}
