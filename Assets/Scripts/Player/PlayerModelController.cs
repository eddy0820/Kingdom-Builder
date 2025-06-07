using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleDrakeStudios.ModularCharacters;

public class PlayerModelController : MonoBehaviour, IListenToPlayerSpawn
{
    [SerializeField] Transform modelParent;

    [Space(10)]

    [SerializeField] GameObject defaultCharacter;

    [Space(10)]

    [SerializeField] ModularCharacterManager maleModularCharacterPrefab;
    [SerializeField] ModularCharacterManager femaleModularCharacterPrefab;

    Material characterMaterial;

    public void OnPlayerSpawned(Transform spawnTransform)
    {
        PlayerInfoHolder playerInfo = FindObjectOfType<PlayerInfoHolder>();

        if(playerInfo != null)
        {
            Destroy(defaultCharacter);
            LoadCharacter(playerInfo);
        }
    }

    private void LoadCharacter(PlayerInfoHolder playerInfo)
    {
        ModularCharacterManager modularCharacterToInstantiate = playerInfo.PlayerSex == PlayerInfoHolder.Sex.Male ? maleModularCharacterPrefab : femaleModularCharacterPrefab;

        ModularCharacterManager modularCharacter = Instantiate(modularCharacterToInstantiate, modelParent);
        characterMaterial = Instantiate(modularCharacter.CharacterMaterial);

        modularCharacter.SetAllPartsMaterial(characterMaterial);

        characterMaterial.SetColor("_Color_Skin", playerInfo.PlayerSkinColors.Skin);
        characterMaterial.SetColor("_Color_Stubble", playerInfo.PlayerSkinColors.Stubble);
        characterMaterial.SetColor("_Color_Scar", playerInfo.PlayerSkinColors.Scar);
        ActivatePartWithNegative(modularCharacter, ModularBodyPart.Ear, playerInfo.EarID);
        ActivatePartWithNegative(modularCharacter, ModularBodyPart.Head, playerInfo.FaceID);
        characterMaterial.SetColor("_Color_BodyArt", playerInfo.FacePaintColor);
        characterMaterial.SetColor("_Color_Eyes", playerInfo.EyeColor);
        ActivatePartWithNegative(modularCharacter, ModularBodyPart.Hair, playerInfo.HairID);
        ActivatePartWithNegative(modularCharacter, ModularBodyPart.Eyebrow, playerInfo.EyeBrowID);
        ActivatePartWithNegative(modularCharacter, ModularBodyPart.FacialHair, playerInfo.FacialHairID);
        characterMaterial.SetColor("_Color_Hair", playerInfo.HairColor);
        characterMaterial.SetColor("_Color_Leather_Secondary", playerInfo.UnderwearColor);
    }

    private void ActivatePartWithNegative(ModularCharacterManager character, ModularBodyPart part, int partID)
    {
        if(partID < 0)
            character.DeactivatePart(part);
        else
            character.ActivatePart(part, partID);
    }

    public void Toggle3rdPersonModel(bool toggle)
    {
        modelParent.gameObject.SetActive(toggle);
    }
}
