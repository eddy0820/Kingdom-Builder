using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Targetable : MonoBehaviour, ITargetable, IInteractable
{
    [SerializeField] Transform lockOnLocation;
    public Transform LockOnLocation => lockOnLocation;

    [SerializeField] List<InteractionTypeSO> interactionTypes;
    public List<InteractionTypeSO> InteractionTypes => interactionTypes;

    TargetableStats targetableStats;
    public IDamageable IDamageable => targetableStats;

    

    private void Awake()
    {
        targetableStats = new TargetableStats(null);
    }
}

public class TargetableStats : CharacterStats, IDamageable
{
    public TargetableStats(BaseStatsSO _baseStatsSO) : base(_baseStatsSO)
    {
    }

    public Action<float, float, float> OnHealthChanged { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void Die()
    {
        throw new NotImplementedException();
    }

    public float GetCurrentHealth()
    {
        throw new NotImplementedException();
    }

    public float GetProjectedHealth()
    {
        throw new NotImplementedException();
    }

    public void HealInstant(float amount)
    {
        throw new NotImplementedException();
    }

    public void HealOverTime(float percentAmount, float duration)
    {
        throw new NotImplementedException();
    }

    public IEnumerator HealOverTimeCoroutine()
    {
        throw new NotImplementedException();
    }

    public void HealOverTimeToPercent(float percent, float duration)
    {
        throw new NotImplementedException();
    }

    public bool IsDead()
    {
        throw new NotImplementedException();
    }

    public void SetHealth(float amount)
    {
        throw new NotImplementedException();
    }

    public void TakeDamageInstant(float damage)
    {
        throw new NotImplementedException();
    }
}
