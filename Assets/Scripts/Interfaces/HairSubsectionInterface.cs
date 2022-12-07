using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BattleDrakeStudios.ModularCharacters;

public class HairSubsectionInterface : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] int defaultMaleHair;
    [SerializeField] int defaultFemaleHair;

    private void Awake()
    {
        CharacterCreationManager.Instance.MaleModel.ActivatePart(ModularBodyPart.Hair, defaultMaleHair);
        CharacterCreationManager.Instance.FemaleModel.ActivatePart(ModularBodyPart.Hair, defaultFemaleHair);

        switch(CharacterCreationManager.Instance.PlayerInfoHolder.PlayerSex)
        {
            case PlayerInfoHolder.Sex.Male: 

                CharacterCreationManager.Instance.PlayerInfoHolder.SetHair(defaultMaleHair);

                break;
            case PlayerInfoHolder.Sex.Female:
                
                CharacterCreationManager.Instance.PlayerInfoHolder.SetHair(defaultFemaleHair);
                
                break;
        }        
    }

    private void OnEnable()
    {
        switch(CharacterCreationManager.Instance.PlayerInfoHolder.PlayerSex)
        {
            case PlayerInfoHolder.Sex.Male: 

                CharacterCreationManager.Instance.MaleModel.ActiveParts.TryGetValue(ModularBodyPart.Hair, out int hairMale);
                slider.SetValueWithoutNotify(hairMale);

                break;
            case PlayerInfoHolder.Sex.Female:
                
                CharacterCreationManager.Instance.FemaleModel.ActiveParts.TryGetValue(ModularBodyPart.Hair, out int hairFemale);
                slider.SetValueWithoutNotify(hairFemale);
                
                break;
        }
    }

    public void SwitchHair()
    {
        if(slider.value == -1)
        {
            CharacterCreationManager.Instance.MaleModel.DeactivatePart(ModularBodyPart.Hair);
            CharacterCreationManager.Instance.FemaleModel.DeactivatePart(ModularBodyPart.Hair);
            CharacterCreationManager.Instance.PlayerInfoHolder.SetHair(-1);
            return;
        }

        for(int i = 0; i <= slider.maxValue; i++)
        {
            if(i == slider.value)
            {
                CharacterCreationManager.Instance.MaleModel.ActivatePart(ModularBodyPart.Hair, i);
                CharacterCreationManager.Instance.FemaleModel.ActivatePart(ModularBodyPart.Hair, i);
                CharacterCreationManager.Instance.PlayerInfoHolder.SetHair(i);
            }
        }
    }
}
