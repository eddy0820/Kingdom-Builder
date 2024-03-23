using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[System.Serializable]
public class TimeHordeWaveSettings
{
    [SerializeField] float totalWaveTime;
    public float TotalWaveTime => totalWaveTime;

    [Space(10)]

    [CurveRange(0, 0, 1, 1, EColor.Green)]
    [SerializeField] AnimationCurve enemySpawnRateCurve;
    public AnimationCurve EnemySpawnRateCurve => enemySpawnRateCurve;
    [MinMaxSlider(1, 50), SerializeField] Vector2 enemySpawnRateMinMax;
    public Vector2 EnemySpawnRateMinMax => enemySpawnRateMinMax;

    [Space(10)]

    [CurveRange(0, 0, 1, 1, EColor.Blue)]
    [SerializeField] AnimationCurve enemyCountPerSpawnCurve;
    public AnimationCurve EnemyCountPerSpawnCurve => enemyCountPerSpawnCurve;
    [MinMaxSlider(1, 50), SerializeField] Vector2 enemyCountPerSpawnMinMax;
    public Vector2 EnemyCountPerSpawnMinMax => enemyCountPerSpawnMinMax;

    [Space(10)]

    [CurveRange(0, 0, 1, 1, EColor.Red)]
    [SerializeField] AnimationCurve severityCurve;
    public AnimationCurve SeverityCurve => severityCurve;

    [ArrayElementTitle("severity")]
    [SerializeField] List<EnemySeverityEntry> enemySeverities;
    public List<EnemySeverityEntry> EnemySeverities => enemySeverities;

    [System.Serializable]
    public class EnemySeverityEntry
    {
        [SerializeField] int severity;
        public int Severity => severity;

        [ArrayElementTitle("enemy")]
        [SerializeField] List<EnemyWeightEntry> enemyPool;
        public List<EnemyWeightEntry> EnemyPool => enemyPool;
    }
}
