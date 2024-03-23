using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetWaveBurst : WaveBurst
{
    List<GameObject> enemies;

    public SetWaveBurst(CountHordeWaveSettings.WaveBurstEntry _waveBurstEntry) : base(_waveBurstEntry)
    {
        enemies =  _waveBurstEntry.Enemies.List;
    }
}
