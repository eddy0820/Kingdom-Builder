using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EddyLib.GameSettingsSystem;

[Serializable]
public class StatsSettings : SettingsCategory
{
    [SerializeField] EStatsRoundingType statsRoundingType = EStatsRoundingType.Tenths;
    public EStatsRoundingType StatsRoundingType => statsRoundingType;

    [Space(10)]

    [SerializeField] bool showNonPlayerDamagePopups = true;
    public bool ShowNonPlayerDamagePopups => showNonPlayerDamagePopups;
    [SerializeField] bool showNonPlayerHealthAndStaminaTexts = true;
    public bool ShowNonPlayerHealthAndStaminaTexts => showNonPlayerHealthAndStaminaTexts;

    [Space(10)]

    [SerializeField] bool enableHealthDebugMessages;
    public bool EnableHealthDebugMessages => enableHealthDebugMessages;
    [SerializeField] bool enableStaminaDebugMessages;
    public bool EnableStaminaDebugMessages => enableStaminaDebugMessages;
}
