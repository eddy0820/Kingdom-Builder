using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleDrakeStudios.ModularCharacters;

public class CharacterCreationManager : MonoBehaviour
{
    public static CharacterCreationManager Instance {get; private set; } = null;

    [SerializeField] Material characterMaterial;
    [SerializeField] ModularCharacterManager maleModel;
    public ModularCharacterManager MaleModel => maleModel;
    [SerializeField] ModularCharacterManager femaleModel;
    public ModularCharacterManager FemaleModel => femaleModel;

    [Space(10)]

    [SerializeField] PlayerInfoHolder playerInfoHolder;

    public PlayerInfoHolder PlayerInfoHolder => playerInfoHolder;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        
        Material instancedCharacterMaterial = Instantiate<Material>(characterMaterial);
        maleModel.instancedMaterial = instancedCharacterMaterial;
        maleModel.SetAllPartsMaterial(maleModel.instancedMaterial);
        femaleModel.instancedMaterial = instancedCharacterMaterial;
        femaleModel.SetAllPartsMaterial(femaleModel.instancedMaterial);
    }
}
