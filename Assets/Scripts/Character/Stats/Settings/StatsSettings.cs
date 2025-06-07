using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EddyLib.GameSettingsSystem;
using UnityEngine.InputSystem;

[Serializable]
public class StatsSettings : SettingsCategory
{
    [Header("General")]
    [SerializeField] EStatsRoundingType statsRoundingType = EStatsRoundingType.Tenths;
    public EStatsRoundingType StatsRoundingType => statsRoundingType;

    [Header("Player")]
    [SerializeField] bool playerConsumesStamina;
    public bool PlayerConsumesStamina => playerConsumesStamina;

    [Space(10)]

    [SerializeField] bool showPlayerDamagePopups = true;
    public bool ShowPlayerDamagePopups => showPlayerDamagePopups;
    [SerializeField] bool showPlayerHealthAndStaminaTexts = true;
    public bool ShowPlayerHealthAndStaminaTexts => showPlayerHealthAndStaminaTexts;

    [Header("Non-Player")]
    [SerializeField] bool showNonPlayerDamagePopups = true;
    public bool ShowNonPlayerDamagePopups => showNonPlayerDamagePopups;
    [SerializeField] bool showNonPlayerHealthAndStaminaTexts = true;
    public bool ShowNonPlayerHealthAndStaminaTexts => showNonPlayerHealthAndStaminaTexts;

    [Header("Debug Messages")]
    [SerializeField] bool enableHealthDebugMessages;
    public bool EnableHealthDebugMessages => enableHealthDebugMessages;
    [SerializeField] bool enableStaminaDebugMessages;
    public bool EnableStaminaDebugMessages => enableStaminaDebugMessages;
 }
