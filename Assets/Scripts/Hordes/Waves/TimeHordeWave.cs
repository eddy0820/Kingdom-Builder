using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeHordeWave : HordeWave
{
    float TotalWaveTime => waveSO.TimeSettings.TotalWaveTime;

    Queue<float> enemySpawnRateChangeTimes;
    Queue<float> enemyCountPerSpawnChangeTimes;
    Queue<float> severityChangeTimes;

    // you are here

    public TimeHordeWave(HordeWaveSO _waveSO) : base(_waveSO)
    {
        
    }
}

