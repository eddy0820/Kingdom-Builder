using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleDrakeStudios.ModularCharacters;
using NaughtyAttributes;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner Instance { get; private set; }

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

    GridBuildingInfo gridBuildingInfo;
    public GridBuildingInfo GridBuildingInfo => gridBuildingInfo;

    PlayerInfoHolder playerInfo = null;
    PlayerCharacterController playerCharacter;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        gridBuildingInfo = GetComponent<GridBuildingInfo>();

        UnityEngine.SceneManagement.SceneManager.LoadScene(playerScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);

        if(gridBuildingInfo.EnableBuilding)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(gridBuildingInfo.GridBuildingScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }
    }

    private void Start()
    {
        playerCharacter = FindObjectOfType<PlayerCharacterController>();
        playerCharacter.Motor.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

        CheckIfPreExistingCharacter();

        if(gridBuildingInfo.EnableBuilding)
        {
            GridBuildingManager.Instance.Init
                (
                    gridBuildingInfo.GridWidth, 
                    gridBuildingInfo.GridLength,
                    gridBuildingInfo.CellSize,
                    gridBuildingInfo.GridHeight,
                    gridBuildingInfo.GridVerticalCount,
                    gridBuildingInfo.MaxBuildDistance,
                    gridBuildingInfo.EdgeColliderLayerMask,
                    gridBuildingInfo.PlaceableObjectsColliderLayerMask,
                    gridBuildingInfo.UIIconAnimationDelay,
                    gridBuildingInfo.UIIconAnimationSpeed,
                    gridBuildingInfo.Debug,
                    gridBuildingInfo.EnableGridDebug,
                    gridBuildingInfo.DebugFontSize,
                    gridBuildingInfo.EnableMouse3DDebug,
                    gridBuildingInfo.Mouse3DDebugMaterial,
                    gridBuildingInfo.EnableFakeVisualDebug,
                    gridBuildingInfo.FakeVisualMaterial,
                    gridBuildingInfo.PlaceableObjectTypesFakeVisualBlacklist,
                    gridBuildingInfo.BuildingTypesFakeVisualBlacklist,
                    gridBuildingInfo.EnableVisualAnchorDebug,
                    gridBuildingInfo.VisualAnchorDebugMaterial,
                    gridBuildingInfo.IdentifierTag
                );

            GridBuildingManager.Instance.Setup(gridBuildingInfo.GridOriginPoint.position);
        }
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
