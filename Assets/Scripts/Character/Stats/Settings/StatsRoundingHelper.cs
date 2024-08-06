using UnityEngine;
using EddyLib.GameSettingsSystem;

public static class StatsRoundingHelper
{
    static StatsSettings MiscSettings => GameSettings.GetSettings<StatsSettings>();
    public static float RoundValue(float value)
    {
        return RoundValue(value, MiscSettings.StatsRoundingType);
    }

    public static float RoundValue(float value, EStatsRoundingType roundingType)
    {
        float roundNumber = 0;

        switch (roundingType)
        {
            case EStatsRoundingType.Tenths:
                roundNumber = 10;
            break;
            case EStatsRoundingType.Hundredths:
                roundNumber = 100;
            break;
            case EStatsRoundingType.Thousandths:
                roundNumber = 1000;
            break;
        }
        
        return Mathf.Round(value * roundNumber) / roundNumber;
    }

    public static string ValueString => GetValueString(MiscSettings.StatsRoundingType);

    public static string GetValueString(EStatsRoundingType roundingType)
    {
        string valueString = "";

        switch(roundingType)
        {
            case EStatsRoundingType.Tenths:
                valueString = "F1";
            break;
            case EStatsRoundingType.Hundredths:
                valueString = "F2";
            break;
            case EStatsRoundingType.Thousandths:
                valueString = "F3";
            break;
        }
        
        return valueString;
    }

    public static int NumDecimalPlaces => GetNumDecimalPlaces(MiscSettings.StatsRoundingType);

    public static int GetNumDecimalPlaces(EStatsRoundingType roundingType)
    {
        int numDecimalPlaces = 0;

        switch (roundingType)
        {
            case EStatsRoundingType.Tenths:
                numDecimalPlaces = 1;
            break;
            case EStatsRoundingType.Hundredths:
                numDecimalPlaces = 2;
            break;
            case EStatsRoundingType.Thousandths:
                numDecimalPlaces = 3;
            break;
        }
        
        return numDecimalPlaces;
    }
}
