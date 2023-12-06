using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using DamageNumbersPro;

[RequireComponent(typeof(Collider))]
public class Targetable : MonoBehaviour, ITargetable, IInteractable, IHoldStats
{
    [SerializeField] Transform lockOnLocation;
    public Transform LockOnLocation => lockOnLocation;

    [SerializeField] List<InteractionTypeSO> interactionTypes;
    public List<InteractionTypeSO> InteractionTypes => interactionTypes;

    [Expandable, SerializeField] BaseStatsSO baseStatsSO;
    public BaseStatsSO BaseStatsSO => baseStatsSO;

    [SerializeField] DamageNumberMesh damageNumberMesh;

    TargetableStats targetableStats;
    public IDamageable IDamageable => targetableStats;
    public CharacterStats Stats => targetableStats;
    public IStamina IStamina => null;

    private void Awake()
    {
        targetableStats = new TargetableStats(baseStatsSO);

        StartCoroutine(targetableStats.HealOverTimeCoroutine());
        targetableStats.SetHealth(targetableStats.GetStatFromName[CommonStatTypeNames.MaxHealth].Value);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightBracket))
        {
            DamageNumber damageNumber = damageNumberMesh.Spawn(new Vector3(0, 2, 0));
        }
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
