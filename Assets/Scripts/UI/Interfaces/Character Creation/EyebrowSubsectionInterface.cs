using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BattleDrakeStudios.ModularCharacters;

public class EyebrowSubsectionInterface : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] int maleMaxParts = 9;
    [SerializeField] int femaleMaxParts = 6;

    private void Awake()
    {
        SwitchEyebrow();
    }

    private void OnEnable()
    {
        switch(CharacterCreationManager.Instance.PlayerInfoHolder.PlayerSex)
        {
            case PlayerInfoHolder.Sex.Male: 

                slider.maxValue = maleMaxParts;

                break;
            case PlayerInfoHolder.Sex.Female:
                
                slider.maxValue = femaleMaxParts;
                
                break;
        }
    }

    public void SwitchEyebrow()
    {
        if(slider.value == -1)
        {
            CharacterCreationManager.Instance.MaleModel.DeactivatePart(ModularBodyPart.Eyebrow);
            CharacterCreationManager.Instance.FemaleModel.DeactivatePart(ModularBodyPart.Eyebrow);
            CharacterCreationManager.Instance.PlayerInfoHolder.SetEyeBrow(-1);
            return;
        }

        for(int i = 0; i <= slider.maxValue; i++)
        {
            if(i == slider.value)
            {
                if(i < 7)
                {
                    CharacterCreationManager.Instance.FemaleModel.ActivatePart(ModularBodyPart.Eyebrow, i);
                }

                CharacterCreationManager.Instance.MaleModel.ActivatePart(ModularBodyPart.Eyebrow, i);

                if(i >= 7 && CharacterCreationManager.Instance.PlayerInfoHolder.PlayerSex == PlayerInfoHolder.Sex.Female)
                {
                    CharacterCreationManager.Instance.PlayerInfoHolder.SetEyeBrow(6);
                }
                else
                {
                    CharacterCreationManager.Instance.PlayerInfoHolder.SetEyeBrow(i);
                }
                
            }
        }
    }
}
