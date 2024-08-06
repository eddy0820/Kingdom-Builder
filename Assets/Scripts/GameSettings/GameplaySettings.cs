using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using EddyLib.GameSettingsSystem;

[Serializable]
public class GameplaySettings : SettingsCategory
{
    [SerializeField] Vector3 gravity = new(0, -30f, 0);
    public Vector3 Gravity => gravity;
}
