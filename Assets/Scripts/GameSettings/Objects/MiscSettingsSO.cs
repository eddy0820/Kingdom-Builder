using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Misc Settings", menuName = "GameSettings/Misc/Misc Settings")]
public class MiscSettingsSO : ScriptableObject
{
    [SerializeField] CharacterStatsRoundingType characterStatsRoundingType = CharacterStatsRoundingType.Tenths;
    public CharacterStatsRoundingType CharacterStatsRoundingType => characterStatsRoundingType;
}

public enum CharacterStatsRoundingType
{
    Tenths,
    Hundredths,
    Thousandths,
    
}
