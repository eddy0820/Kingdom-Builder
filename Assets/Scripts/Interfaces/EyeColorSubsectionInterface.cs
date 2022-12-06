using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeColorSubsectionInterface : MonoBehaviour
{
    [SerializeField] FlexibleColorPicker colorPicker;
    [SerializeField] Color defaultColor;

    private void Start()
    {
        colorPicker.startingColor = defaultColor;
        colorPicker.SetColor(defaultColor);

        CharacterCreationManager.Instance.PlayerInfoHolder.SetEyeColor(colorPicker.color);
    }

    public void SwitchColor()
    {
        CharacterCreationManager.Instance.MaleModel.instancedMaterial.SetColor("_Color_Eyes", colorPicker.color);
        CharacterCreationManager.Instance.FemaleModel.instancedMaterial.SetColor("_Color_Eyes", colorPicker.color);

        CharacterCreationManager.Instance.PlayerInfoHolder.SetEyeColor(colorPicker.color);
    }
}
