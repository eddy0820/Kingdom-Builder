using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BattleDrakeStudios.ModularCharacters;

public class EarSubsectionInterface : MonoBehaviour
{
    [SerializeField] Slider slider;

    private void Awake()
    {
        SwitchEar();
    }

    public void SwitchEar()
    {
        switch(slider.value)
        {
            case 0:
                CharacterCreationManager.Instance.MaleModel.DeactivatePart(ModularBodyPart.Ear);
                CharacterCreationManager.Instance.FemaleModel.DeactivatePart(ModularBodyPart.Ear);
                CharacterCreationManager.Instance.PlayerInfoHolder.SetEar(-1);
                break;
            case 1:
                CharacterCreationManager.Instance.MaleModel.ActivatePart(ModularBodyPart.Ear, 0);
                CharacterCreationManager.Instance.FemaleModel.ActivatePart(ModularBodyPart.Ear, 0);
                CharacterCreationManager.Instance.PlayerInfoHolder.SetEar(0);
                break;
            case 2:
                CharacterCreationManager.Instance.MaleModel.ActivatePart(ModularBodyPart.Ear, 1);
                CharacterCreationManager.Instance.FemaleModel.ActivatePart(ModularBodyPart.Ear, 1);
                CharacterCreationManager.Instance.PlayerInfoHolder.SetEar(1);
                break;
            case 3:
                CharacterCreationManager.Instance.MaleModel.ActivatePart(ModularBodyPart.Ear, 2);
                CharacterCreationManager.Instance.FemaleModel.ActivatePart(ModularBodyPart.Ear, 2);
                CharacterCreationManager.Instance.PlayerInfoHolder.SetEar(2);
                break;
        }
    }
}
