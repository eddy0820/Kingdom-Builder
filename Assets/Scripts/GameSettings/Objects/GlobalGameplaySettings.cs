using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New Global Gameplay Settings", menuName = "GameSettings/Global Gameplay Settings")]
public class GlobalGameplaySettings : SingletonScriptableObject<GlobalGameplaySettings>
{
    [Expandable, SerializeField] GameplaySettingsSO defaultGameplaySettings;
    public Vector3 GlobalGravity => defaultGameplaySettings.GlobalGravity;
    [Expandable, SerializeField] MiscSettingsSO defaultMiscSettings;
    public CharacterStatsRoundingType CharacterStatsRoundingType => defaultMiscSettings.CharacterStatsRoundingType;
}
