using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountHordeWave : HordeWave
{
    Queue<WaveBurst> waveBursts;

    public CountHordeWave(HordeWaveSO _waveSO) : base(_waveSO)
    {
        waveBursts = new Queue<WaveBurst>();

        _waveSO.CountSettings.WaveBursts.ForEach(waveBurstEntry =>
        {
            switch(waveBurstEntry.BurstType)
            {
                case CountHordeWaveSettings.EWaveBurstType.Set:
                    waveBursts.Enqueue(new SetWaveBurst(waveBurstEntry));
                    break;

                case CountHordeWaveSettings.EWaveBurstType.Random:
                    waveBursts.Enqueue(new RandomWaveBurst(waveBurstEntry));
                    break;
            }
        });
    }
}

