using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleDrakeStudios.ModularCharacters;

public class CharacterCreationManager : MonoBehaviour
{
    public static CharacterCreationManager Instance {get; private set; } = null;

    [SerializeField] Material characterMaterial;
    Material maleCharacterMaterial;
    public Material MaleInstancedCharacterMaterial => maleCharacterMaterial;
    Material femaleCharacterMaterial;
    public Material FemaleInstancedCharacterMaterial => femaleCharacterMaterial;
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
        
        maleCharacterMaterial = Instantiate(characterMaterial);
        femaleCharacterMaterial = Instantiate(characterMaterial);
        maleModel.SetAllPartsMaterial(maleCharacterMaterial);
        femaleModel.SetAllPartsMaterial(femaleCharacterMaterial);
    }
}
