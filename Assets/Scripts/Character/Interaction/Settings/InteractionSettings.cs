using System;
using EddyLib.GameSettingsSystem;
using UnityEngine;

[Serializable]
public class InteractionSettings : SettingsCategory 
{
    [SerializeField] bool enableInteraction;
    public bool EnableInteraction => enableInteraction;
}
