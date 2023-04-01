using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BattleDrakeStudios.ModularCharacters;

public class FacialHairSubsectionInterface : MonoBehaviour
{
    [SerializeField] Slider slider;

    private void Awake()
    {
        SwitchFacialHair();
    }

    public void SwitchFacialHair()
    {
        if(slider.value == -1)
        {
            CharacterCreationManager.Instance.MaleModel.DeactivatePart(ModularBodyPart.FacialHair);
            CharacterCreationManager.Instance.FemaleModel.DeactivatePart(ModularBodyPart.FacialHair);
            CharacterCreationManager.Instance.PlayerInfoHolder.SetFacialHair(-1);
            return;
        }

        for(int i = 0; i <= slider.maxValue; i++)
        {
            if(i == slider.value)
            {
                CharacterCreationManager.Instance.MaleModel.ActivatePart(ModularBodyPart.FacialHair, i);
                CharacterCreationManager.Instance.FemaleModel.ActivatePart(ModularBodyPart.FacialHair, i);
                CharacterCreationManager.Instance.PlayerInfoHolder.SetFacialHair(i);
            }
        }
    }
}
