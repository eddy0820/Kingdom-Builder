using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WaveBurst
{
    protected CountHordeWaveSettings.WaveBurstEntry waveBurstEntry;
    protected float TimeTillNextBurst => waveBurstEntry.TimeTillNextBurst;

    public WaveBurst(CountHordeWaveSettings.WaveBurstEntry _waveBurstEntry)
    {
        waveBurstEntry = _waveBurstEntry;
    }
}
