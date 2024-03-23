using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "New Horde Preset", menuName = "Hordes/Preset")]
public class HordePresetSO : ScriptableObject
{
    [Expandable, SerializeField] List<HordeWaveSO> hordeWaves;
    public List<HordeWaveSO> HordeWaves => hordeWaves;
    [Space(10)]
    [CurveRange(0, 0, 1, 1, EColor.Green)]
    [SerializeField] AnimationCurve timeBetweenWavesCurve;
    public AnimationCurve TimeBetweenWavesCurve => timeBetweenWavesCurve;
    [MinMaxSlider(1, 50), SerializeField] Vector2 timeBetweenWavesMinMax;
    public Vector2 TimeBetweenWavesMinMax => timeBetweenWavesMinMax;
}
