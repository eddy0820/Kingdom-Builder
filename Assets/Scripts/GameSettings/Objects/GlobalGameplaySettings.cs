using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Global Gameplay Settings", menuName = "GameSettings/Gameplay/Global Gameplay Settings")]
public class GlobalGameplaySettings : SingletonScriptableObject<GlobalGameplaySettings>
{
    [SerializeField] GameplaySettingsSO defaultGameplaySettings;
    public Vector3 GlobalGravity => defaultGameplaySettings.GlobalGravity;
}
