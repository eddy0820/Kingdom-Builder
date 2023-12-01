using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterStatsRoundingHelper
{
    public static float RoundValueUsingGlobalSettings(float value)
    {
        return RoundValue(value, GlobalGameplaySettings.Instance.CharacterStatsRoundingType);
    }

    public static float RoundValue(float value, CharacterStatsRoundingType roundingType)
    {
        float roundNumber = 0;

        switch (roundingType)
        {
            case CharacterStatsRoundingType.Tenths:
                roundNumber = 10;
            break;
            case CharacterStatsRoundingType.Hundredths:
                roundNumber = 100;
            break;
            case CharacterStatsRoundingType.Thousandths:
                roundNumber = 1000;
            break;
        }
        
        return Mathf.Round(value * roundNumber) / roundNumber;
    }

    public static string GlobalValueString => GetValueStringFromGlobalSettings();

    private static string GetValueStringFromGlobalSettings()
    {
        return GetValueString(GlobalGameplaySettings.Instance.CharacterStatsRoundingType);
    }

    public static string GetValueString(CharacterStatsRoundingType roundingType)
    {
        string valueString = "";

        switch (roundingType)
        {
            case CharacterStatsRoundingType.Tenths:
                valueString = "F1";
            break;
            case CharacterStatsRoundingType.Hundredths:
                valueString = "F2";
            break;
            case CharacterStatsRoundingType.Thousandths:
                valueString = "F3";
            break;
        }
        
        return valueString;
    }

    public static int GlobalNumDecimals => GetNumDecimalPlacesFromGlobalSettings();

    private static int GetNumDecimalPlacesFromGlobalSettings()
    {
        return GetNumDecimalPlaces(GlobalGameplaySettings.Instance.CharacterStatsRoundingType);
    }

    public static int GetNumDecimalPlaces(CharacterStatsRoundingType roundingType)
    {
        int numDecimalPlaces = 0;

        switch (roundingType)
        {
            case CharacterStatsRoundingType.Tenths:
                numDecimalPlaces = 1;
            break;
            case CharacterStatsRoundingType.Hundredths:
                numDecimalPlaces = 2;
            break;
            case CharacterStatsRoundingType.Thousandths:
                numDecimalPlaces = 3;
            break;
        }
        
        return numDecimalPlaces;
    }
}
