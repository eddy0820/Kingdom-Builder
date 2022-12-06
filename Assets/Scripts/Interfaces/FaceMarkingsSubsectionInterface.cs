using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BattleDrakeStudios.ModularCharacters;

public class FaceMarkingsSubsectionInterface : MonoBehaviour
{
    [SerializeField] Slider slider;

    private void Awake()
    {
        SwitchFace();
    }

    public void SwitchFace()
    {
        for(int i = 0; i <= slider.maxValue; i++)
        {
            if(i == slider.value)
            {
                CharacterCreationManager.Instance.MaleModel.ActivatePart(ModularBodyPart.Head, i);
                CharacterCreationManager.Instance.FemaleModel.ActivatePart(ModularBodyPart.Head, i);
                CharacterCreationManager.Instance.PlayerInfoHolder.SetFace(i);
            }
        }
    }
}
