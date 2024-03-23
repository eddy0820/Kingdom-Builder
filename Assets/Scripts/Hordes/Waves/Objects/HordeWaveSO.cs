using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "New Horde Wave", menuName = "Hordes/Wave")]
public class HordeWaveSO : ScriptableObject
{
    [SerializeField] EHordeWaveType waveType;
    public EHordeWaveType WaveType => waveType;

    [ShowIf("waveType", EHordeWaveType.Count), SerializeField] CountHordeWaveSettings countSettings;
    public CountHordeWaveSettings CountSettings => countSettings;
    [ShowIf("waveType", EHordeWaveType.Timed), SerializeField] TimeHordeWaveSettings timeSettings;
    public TimeHordeWaveSettings TimeSettings => timeSettings;
}
