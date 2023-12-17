using System.Collections;
using System.Collections.Generic;
using DamageNumbersPro;
using UnityEngine;

public abstract class StaminaDamageableStatUI : DamageableStatUI
{
    [Header("Stamina UI")]
    [SerializeField] protected BarUI staminaBarUI;

    [Header("Stamina Damage Popups")]
    [SerializeField] protected DamageNumberMesh increaseMaxStaminaNumberMesh;
    [SerializeField] protected DamageNumberMesh decreaseMaxStaminaNumberMesh;

    protected abstract Stat MaxStaminaStat { get; }
    protected abstract IStamina IStamina { get; }

    protected override void OnAwake()
    {
        staminaBarUI.SetupBarUI(ReturnAdditionalStaminaBarFadeCooldown);

        IStamina.OnStaminaChanged += OnStaminaChanged;
        CharacterStats.OnStatModifierChanged += OnStatModifierChangedStaminaChanged;
    }

    private float ReturnAdditionalStaminaBarFadeCooldown()
    {
        if(CharacterStats.CharacterHasStats(CommonStatTypeNames.StaminaRegenCooldown, CommonStatTypeNames.StaminaRegen))
        {
            if(IStamina.GetCurrentStamina() != MaxStaminaStat.Value)
                return CharacterStats.GetStatFromName[CommonStatTypeNames.StaminaRegenCooldown].Value;
        }

        return 0;
    }

    protected virtual void OnStaminaChanged(float currentStamina, float projectedStamina, float maxStamina, EStaminaChangedOperation operation = EStaminaChangedOperation.NoChange, float staminaChangeAmount = 0)
    {
        staminaBarUI.UpdateBar(currentStamina, projectedStamina, maxStamina);
    }

    protected void OnStatModifierChangedStaminaChanged(Stat stat, StatModifier statModifier, EStatModifierChangedOperation operation)
    {
        if(stat.type != MaxStaminaStat.type) return;

        float maxStaminaChangeAmount;

        switch(operation)
        {
            case EStatModifierChangedOperation.Added:
                maxStaminaChangeAmount = statModifier.value;
                break;
            case EStatModifierChangedOperation.Removed:
            case EStatModifierChangedOperation.RemovedAllFromSource:
                maxStaminaChangeAmount = -statModifier.value;
                break;
            default:
                return;
        }

        DoMaxStaminaChangePopup(operation, maxStaminaChangeAmount);

        OnStaminaChanged(IStamina.GetCurrentStamina(), IStamina.GetProjectedStamina(), stat.Value);
    }

    protected virtual void DoMaxStaminaChangePopup(EStatModifierChangedOperation eStatModifierChangedOperation, float staminaChangeAmount)
    {
        DamageNumberMesh damageNumberMesh;

        switch(eStatModifierChangedOperation)
        {
            case EStatModifierChangedOperation.Added:
                damageNumberMesh = increaseMaxStaminaNumberMesh;
            break;

            case EStatModifierChangedOperation.Removed:
            case EStatModifierChangedOperation.RemovedAllFromSource:
                damageNumberMesh = decreaseMaxStaminaNumberMesh;
            break;

            default:
                return;
        }

        damageNumberMesh.digitSettings.decimals = CharacterStatsRoundingHelper.GlobalNumDecimals;

        damageNumberMesh.Spawn(DamageNumberSpawnPosition, staminaChangeAmount);
    }
    
}
