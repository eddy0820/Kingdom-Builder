using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building Type Database", menuName = "Databases/Building Type Database")]
public class BuildingTypesDatabaseSO : ScriptableObject
{
    [SerializeField] List<BuildingTypesSO> buildingTypes;
    public List<BuildingTypesSO> BuildingTypes => buildingTypes;
}
