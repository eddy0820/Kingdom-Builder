using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Building Type Object", menuName = "Building System/Building Type")]
public class BuildingTypesSO : ScriptableObject
{
    [SerializeField] new string name;
    public string Name => name;
    [SerializeField] BuildingTypes buildingType;
    public BuildingTypes BuildingType => buildingType;
    [SerializeField] List<SubBuildingTypes> possibleSubTypes;
    public List<SubBuildingTypes> PossibleSubTypes => possibleSubTypes;
}
