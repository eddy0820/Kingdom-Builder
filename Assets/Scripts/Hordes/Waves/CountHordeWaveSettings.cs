using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using EddyLib.Util.DrawerAttributes;

[System.Serializable]
public class CountHordeWaveSettings
{
    [SerializeField] List<WaveBurstEntry> waveBursts;
    public List<WaveBurstEntry> WaveBursts => waveBursts;


    [System.Serializable]
    public class WaveBurstEntry
    {
        [SerializeField] EWaveBurstType burstType;
        public EWaveBurstType BurstType => burstType;

        [SerializeField] float timeTillNextBurst;
        public float TimeTillNextBurst => timeTillNextBurst;

        [AllowNesting, ShowIf("burstType", EWaveBurstType.Random), SerializeField] int enemiesCount;
        public int EnemiesCount => enemiesCount;

        [AllowNesting, HideIf("burstType", EWaveBurstType.Random), SerializeField] EnemiesList enemies;
        public EnemiesList Enemies => enemies;

        [AllowNesting, ShowIf("burstType", EWaveBurstType.Random), SerializeField] EnemyWeightEntryList enemyWeights;
        public EnemyWeightEntryList EnemyWeights => enemyWeights;
    }

    [System.Serializable]
    public class EnemiesList
    {
        [SerializeField] List<GameObject> list;
        public List<GameObject> List => list;
    }

    [System.Serializable]
    public class EnemyWeightEntryList
    {
        [ElementTitle("enemy")]
        [SerializeField] List<EnemyWeightEntry> list;
        public List<EnemyWeightEntry> List => list;
    }

    public enum EWaveBurstType
    {
        Set,
        Random
    }
    
}

[System.Serializable]
public class EnemyWeightEntry
{
    [SerializeField] GameObject enemy;
    public GameObject Enemy => enemy;

    [SerializeField] float weight;
    public float Weight => weight;
}
