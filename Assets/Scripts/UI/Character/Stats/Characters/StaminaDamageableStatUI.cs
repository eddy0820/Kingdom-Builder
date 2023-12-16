using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StaminaDamageableStatUI : DamageableStatUI
{
    [Header("Stamina UI")]
    [SerializeField] protected BarUI staminaBarUI;

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

        OnStaminaChanged(IStamina.GetCurrentStamina(), IStamina.GetProjectedStamina(), stat.Value);
    }
}
