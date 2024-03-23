using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWaveBurst : WaveBurst
{
    int EnemiesCount => waveBurstEntry.EnemiesCount;

    List<EnemyWeightEntry> enemyWeights;

    public RandomWaveBurst(CountHordeWaveSettings.WaveBurstEntry _waveBurstEntry) : base(_waveBurstEntry)
    {
        enemyWeights = _waveBurstEntry.EnemyWeights.List;
    }
}
