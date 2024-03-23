using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horde
{
    HordePresetSO preset;

    List<HordeWave> completedWaves = new();
    HordeWave activeWave;

    Queue<HordeWave> waves;
    Queue<float> timeBetweenWaves;

    public Horde(HordePresetSO _preset)
    {   
        preset = _preset;
        CreateHorde();
    }

    private void CreateHorde()
    {
        waves = new Queue<HordeWave>();

        preset.HordeWaves.ForEach(wave =>
        {
            switch(wave.WaveType)
            {
                case EHordeWaveType.Count:
                    waves.Enqueue(new CountHordeWave(wave));
                    break;
                case EHordeWaveType.Timed:
                    waves.Enqueue(new TimeHordeWave(wave));
                    break;
            }
        });

        timeBetweenWaves = GetTimeBetweenWaves(waves.Count, preset.TimeBetweenWavesMinMax, preset.TimeBetweenWavesCurve);   
    }

    public Queue<float> GetTimeBetweenWaves(int numberOfWaves, Vector2 timeBetweenWavesMinMax, AnimationCurve timeBetweenWavesCurve)
    {
        Queue<float> timeBetweenWavesQueue = new();

        for(int i = 0; i < numberOfWaves - 1; i++)
        {
            float timeBetweenWaves = Mathf.Lerp(timeBetweenWavesMinMax.x, timeBetweenWavesMinMax.y, timeBetweenWavesCurve.Evaluate(Mathf.InverseLerp(1, numberOfWaves - 1, i + 1)));
            
            timeBetweenWaves = Mathf.Round(timeBetweenWaves);

            timeBetweenWavesQueue.Enqueue(timeBetweenWaves);
        }

        return timeBetweenWavesQueue;
    }
}
