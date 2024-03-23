using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HordeWave
{
    protected HordeWaveSO waveSO;

    public HordeWave(HordeWaveSO _waveSO)
    {
        waveSO = _waveSO;
    }
}
