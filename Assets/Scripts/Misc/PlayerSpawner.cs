using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleDrakeStudios.ModularCharacters;

public class PlayerSpawner : MonoBehaviour
{
    [Scene]
    [SerializeField] string playerScene;

    [Space(15)]

    [SerializeField] Transform spawnPoint;
    public Transform SpawnPoint => spawnPoint;

    [Space(15)]

    [SerializeField] GameObject maleModel;
    public GameObject MaleModel => maleModel;
    [SerializeField] GameObject femaleModel;
    public GameObject FemaleModel => femaleModel; 

    [Space(15)]
    [SerializeField] Avatar avatar;
    [SerializeField] Material characterMaterial;

    PlayerInfoHolder playerInfo = null;
    PlayerCharacterController playerCharacter;

    private void Awake()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(playerScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    private void Start()
    {
        playerCharacter = FindObjectOfType<PlayerCharacterController>();
        playerCharacter.Motor.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

        CheckIfPreExistingCharacter();
    }

    private void CheckIfPreExistingCharacter()
    {
        try
        {
            playerInfo = GameObject.FindObjectOfType<PlayerInfoHolder>();
        }
        catch{}

        if(playerInfo != null)
        {
            Destroy(playerCharacter.transform.GetChild(0).GetChild(0).gameObject);
            GameObject obj;

            if(playerInfo.PlayerSex == PlayerInfoHolder.Sex.Male)
            {
                obj = Instantiate(maleModel, maleModel.transform.position, Quaternion.identity, playerCharacter.transform.GetChild(0));
            }
            else
            {
                obj = Instantiate(femaleModel, femaleModel.transform.position, Quaternion.identity, playerCharacter.transform.GetChild(0));
            }

            Material instancedCharacterMaterial = Instantiate<Material>(characterMaterial);

            ModularCharacterManager modularCharacter = obj.GetComponent<ModularCharacterManager>();

            modularCharacter.instancedMaterial = instancedCharacterMaterial;
            modularCharacter.SetAllPartsMaterial(modularCharacter.instancedMaterial);

            modularCharacter.instancedMaterial.SetColor("_Color_Skin", playerInfo.PlayerSkinColors.Skin);
            modularCharacter.instancedMaterial.SetColor("_Color_Stubble", playerInfo.PlayerSkinColors.Stubble);
            modularCharacter.instancedMaterial.SetColor("_Color_Scar", playerInfo.PlayerSkinColors.Scar);
            ActivatePartWithNegative(modularCharacter, ModularBodyPart.Ear, playerInfo.EarID);
            ActivatePartWithNegative(modularCharacter, ModularBodyPart.Head, playerInfo.FaceID);
            modularCharacter.instancedMaterial.SetColor("_Color_BodyArt", playerInfo.FacePaintColor);
            modularCharacter.instancedMaterial.SetColor("_Color_Eyes", playerInfo.EyeColor);
            ActivatePartWithNegative(modularCharacter, ModularBodyPart.Hair, playerInfo.HairID);
            ActivatePartWithNegative(modularCharacter, ModularBodyPart.Eyebrow, playerInfo.EyeBrowID);
            ActivatePartWithNegative(modularCharacter, ModularBodyPart.FacialHair, playerInfo.FacialHairID);
            modularCharacter.instancedMaterial.SetColor("_Color_Hair", playerInfo.HairColor);
            modularCharacter.instancedMaterial.SetColor("_Color_Leather_Secondary", playerInfo.UnderwearColor);

            Destroy(obj.GetComponent<Animator>());

            playerCharacter.GetComponent<Animator>().avatar = avatar;
        }
    }

    private void ActivatePartWithNegative(ModularCharacterManager character, ModularBodyPart part, int partID)
    {
        if(partID < 0)
        {
            character.DeactivatePart(part);
        }
        else
        {
            character.ActivatePart(part, partID);
        }
    }
}
