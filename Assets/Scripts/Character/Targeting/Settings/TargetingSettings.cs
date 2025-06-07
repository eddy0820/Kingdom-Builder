using System;
using EddyLib.GameSettingsSystem;
using UnityEngine;

[Serializable]
public class TargetingSettings : SettingsCategory
{
    [SerializeField] bool enableTargeting;
    public bool EnableTargeting => enableTargeting;
}
