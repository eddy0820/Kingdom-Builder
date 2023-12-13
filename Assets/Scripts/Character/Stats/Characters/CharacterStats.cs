using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;

[System.Serializable]
public abstract class CharacterStats : MonoBehaviour
{
    [Expandable, SerializeField] protected BaseStatsSO baseStatsSO;

    protected Dictionary<string, Stat> getStatFromName = new();
    public Dictionary<string, Stat> GetStatFromName => getStatFromName;
    protected Dictionary<StatTypeSO, Stat> getStatFromType = new();
    public Dictionary<StatTypeSO, Stat> GetStatFromType => getStatFromType;
    protected Dictionary<string, StatTypeSO> getStatTypeFromName = new();
    public Dictionary<string, StatTypeSO> GetStatTypeFromName => getStatTypeFromName;

    public Action<Stat, StatModifier, EStatModifierChangedOperation> OnStatModifierChanged;

    private void Awake()
    {
        DoAwake();
    }

    private void Start()
    {
        OnStart();
    }

    protected virtual void DoAwake()
    {
        InitializeCharacterStats();
    }

    protected virtual void OnStart()
    {

    }

    protected virtual void InitializeCharacterStats()
    {
        foreach(BaseStatsSO.BaseStat baseStat in baseStatsSO.Stats)
        {
            Stat stat = new(baseStat.StatType, baseStat.Value);
            getStatFromName.Add(baseStat.StatType.Name, stat);
            getStatFromType.Add(baseStat.StatType, stat);
            getStatTypeFromName.Add(baseStat.StatType.Name, baseStat.StatType);
        }
    }

    public void ApplyStatModifier(StatModifier statModifier, string statTypeName) => ApplyStatModifier(getStatFromName[statTypeName], statModifier);
    public void ApplyStatModifier(StatModifier statModifier, StatTypeSO statType) => ApplyStatModifier(getStatFromType[statType], statModifier);
    private void ApplyStatModifier(Stat stat, StatModifier statModifier)
    {
        stat.AddModifier(statModifier);
        OnStatModifierChanged?.Invoke(stat, statModifier, EStatModifierChangedOperation.Added);
    }

    public void RemoveStatModifier(StatModifier statModifier, string statTypeName) => RemoveStatModifier(getStatFromName[statTypeName], statModifier);
    public void RemoveStatModifier(StatModifier statModifier, StatTypeSO statType) => RemoveStatModifier(getStatFromType[statType], statModifier);
    private void RemoveStatModifier(Stat stat, StatModifier statModifier)
    {
        stat.RemoveModifier(statModifier);
        OnStatModifierChanged?.Invoke(stat, statModifier, EStatModifierChangedOperation.Removed);
    }

    public void RemoveAllStatModifiersFromSource(object source) => getStatFromName.ToList().ForEach(stat => RemoveAllStatModifiersFromSource(stat.Value, source));
    public void RemoveAllStatModifiersFromSource(string statTypeName, object source) => RemoveAllStatModifiersFromSource(getStatFromName[statTypeName], source);
    public void RemoveAllStatModifiersFromSource(StatTypeSO statType, object source) => RemoveAllStatModifiersFromSource(getStatFromType[statType], source);
    private void RemoveAllStatModifiersFromSource(Stat stat, object source)
    {
        if(stat.RemoveAllModifiersFromSource(source, out List<StatModifier> removedModifiers))
        {
            foreach(StatModifier modifier in removedModifiers)
            {
                OnStatModifierChanged?.Invoke(stat, modifier, EStatModifierChangedOperation.RemovedAllFromSource);
            }
        }
    }
}
