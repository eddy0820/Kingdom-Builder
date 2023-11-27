using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Targetable : MonoBehaviour, ITargetable, IInteractable, IHoldStats
{
    [SerializeField] Transform lockOnLocation;
    public Transform LockOnLocation => lockOnLocation;

    [SerializeField] List<InteractionTypeSO> interactionTypes;
    public List<InteractionTypeSO> InteractionTypes => interactionTypes;

    [Expandable, SerializeField] BaseStatsSO baseStatsSO;
    public BaseStatsSO BaseStatsSO => baseStatsSO;

    TargetableStats targetableStats;
    public IDamageable IDamageable => targetableStats;
    public CharacterStats Stats => targetableStats;

    private void Awake()
    {
        targetableStats = new TargetableStats(baseStatsSO);

        StartCoroutine(targetableStats.HealOverTimeCoroutine());
        targetableStats.SetHealth(targetableStats.GetStatFromName[CommonStatTypeNames.MaxHealth].Value);
    }

    public string GetDamageableName()
    {
        return targetableStats.GetDamageableName();
    }
}

public class TargetableStats : DamageableCharacterStats
{
    public TargetableStats(BaseStatsSO _baseStatsSO) : base(_baseStatsSO) {}

    protected override string GetDamageableNameInternal()
    {
        return "Targetable";
    }
}
