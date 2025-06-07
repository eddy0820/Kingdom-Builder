using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class ScarIntensitySubsectionInterface : MonoBehaviour
{
    [SerializeField] Slider slider;

    [ReadOnly, SerializeField] float scarColorSubtractionValue;
    public float ScarColorSubtractionValue => scarColorSubtractionValue;

    [SerializeField] SkinSubsectionInterface skinSubsectionInterface;

    public void UpdateScarIntensity()
    {
        scarColorSubtractionValue = slider.value;
        Color scarColor = new(skinSubsectionInterface.CurrentlySelectedButton.Color.r - scarColorSubtractionValue, skinSubsectionInterface.CurrentlySelectedButton.Color.g - scarColorSubtractionValue, skinSubsectionInterface.CurrentlySelectedButton.Color.b - scarColorSubtractionValue, 255);

        CharacterCreationManager.Instance.MaleInstancedCharacterMaterial.SetColor("_Color_Scar", scarColor);
        CharacterCreationManager.Instance.FemaleInstancedCharacterMaterial.SetColor("_Color_Scar", scarColor);

        CharacterCreationManager.Instance.PlayerInfoHolder.PlayerSkinColors.SetScarColor(scarColor);
    }
}
