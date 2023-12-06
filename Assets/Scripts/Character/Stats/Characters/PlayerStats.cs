using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class PlayerStats : DamageableCharacterStats, IStamina
{
    public PlayerStats(BaseStatsSO _baseStatsSO) : base(_baseStatsSO) {}
    string playerName = "Player";

#region Health Stuff 
    Stat HealthRegenStat => getStatFromName[CommonStatTypeNames.HealthRegen];
    Stat OutOfCombatHealthRegenCooldownStat => getStatFromName[CommonStatTypeNames.OutOfCombatHealthRegenCooldown];

    public IEnumerator HealthRegenCoroutine()
    {
        while(!isDead)
        {
            if(currentHealth == MaxHealthStat.Value || Time.time - lastTimeCurrentHealthActivelyChanged < OutOfCombatHealthRegenCooldownStat.Value)
            {
                yield return null;
                continue;
            }

            float amount = MaxHealthStat.Value * (HealthRegenStat.Value / 100);

            if(amount > MaxHealthStat.Value - projectedHealth)
                amount = MaxHealthStat.Value - projectedHealth;

            if(amount < 0) continue;

            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealthStat.Value);
            projectedHealth = currentHealth;

            InvokeOnHealthChanged(EHealthChangedOperation.HealthRegenHeal, amount);

            yield return new WaitForSeconds(1);
        }
    }

    protected override string GetDamageableNameInternal()
    {
        return playerName;
    }

#endregion

#region Stamina Stuff

    float m_currentStamina;
    protected float currentStamina
    {
        get
        {
            if(m_currentStamina > MaxStaminaStat.Value)
                m_currentStamina = MaxStaminaStat.Value;

            return m_currentStamina;
        }
        set
        {
            m_currentStamina = value;
        }
    }

    float m_projectedStamina;
    protected float projectedStamina
    {
        get
        {
            if(m_projectedStamina > MaxStaminaStat.Value)
                m_projectedStamina = MaxStaminaStat.Value;

            return m_projectedStamina;
        }
        set
        {
            m_projectedStamina = value;
        }
    }

    protected float currentPercentPerSecondStaminaOverTime;
    protected float lastTimeCurrentStaminaActivelyChanged;

    Stat MaxStaminaStat => getStatFromName[CommonStatTypeNames.MaxStamina];
    Stat StaminaRegenStat => getStatFromName[CommonStatTypeNames.StaminaRegen];
    Stat StaminaRegenCooldownStat => getStatFromName[CommonStatTypeNames.StaminaRegenCooldown];
    
    public Action<float, float, float, EStaminaChangedOperation, float> OnStaminaChanged { get => OnStaminaChangedInternal; set => OnStaminaChangedInternal = value; }
    Action<float, float, float, EStaminaChangedOperation, float> OnStaminaChangedInternal;

    public void SetStamina(float amount, bool setAsNoChange = false)
    {
        if(AssertIsDead("Can't set stamina when dead.")) return;

        if(amount <= 0)
        {
            Debug.LogWarning("Can't set stamina to 0 or less, just use 'DepleteStaminaInstant()'");
            return;
        }

        float oldCurrentStamina = currentStamina;

        projectedStamina = amount;
        currentStamina = amount;

        EStaminaChangedOperation operation;

        if(currentStamina > oldCurrentStamina && !setAsNoChange)
            operation = EStaminaChangedOperation.Gain;
        else if(currentStamina < oldCurrentStamina && !setAsNoChange)
            operation = EStaminaChangedOperation.Deplete;
        else
            operation = EStaminaChangedOperation.NoChange;

        InvokeOnStaminaChanged(operation, Mathf.Abs(currentStamina - oldCurrentStamina));
    }

    public float GetCurrentStamina()
    {
        return currentStamina;
    }

    public float GetRoundedCurrentStamina()
    {
        return CharacterStatsRoundingHelper.RoundValueUsingGlobalSettings(currentStamina);
    }

    public float GetProjectedStamina()
    {
        return projectedStamina;
    }

    public void DepleteStaminaInstant(float amount)
    {
        if(AssertIsDead("Can't deplete stamina when dead.")) return;

        if(amount < 0)
        {
            Debug.LogWarning("This would gain stamina, just use 'GainStaminaInstant()'");
            return;
        }

        projectedStamina -= amount;

        if(projectedStamina <= currentStamina)
        {
            currentStamina = projectedStamina;
            lastTimeCurrentStaminaActivelyChanged = Time.time;
        }

        InvokeOnStaminaChanged(EStaminaChangedOperation.Deplete, amount);
    }

    public void GainStaminaInstant(float amount)
    {
        if(AssertIsDead("Can't gain stamina when dead.")) return;

        if(amount < 0)
        {
            Debug.LogWarning("This would deplete stamina, just use 'DepleteStaminaInstant()'");
            return;
        }

        if(amount > MaxStaminaStat.Value - projectedStamina)
            amount = MaxStaminaStat.Value - projectedStamina;

        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, MaxStaminaStat.Value);
        lastTimeCurrentStaminaActivelyChanged = Time.time;

        if(projectedStamina <= currentStamina)
            projectedStamina = currentStamina;

        InvokeOnStaminaChanged(EStaminaChangedOperation.Gain, amount);
    }

    public void GainStaminaOverTime(float percentAmount, float percentPerSecond)
    {
        if(AssertIsDead("Can't gain stamina when dead.")) return;

        if(percentAmount < 0)
        {
            Debug.LogWarning("This would deplete stamina, just use 'DepleteStaminaInstant()'");
            return;
        }

        float amount = MaxStaminaStat.Value * (percentAmount / 100);
        if(amount > MaxStaminaStat.Value - projectedStamina)
            amount = MaxStaminaStat.Value - projectedStamina;

        projectedStamina += amount;
        projectedStamina = Mathf.Clamp(projectedStamina, 0, MaxStaminaStat.Value);

        currentPercentPerSecondStaminaOverTime = percentPerSecond;
    }

    public void GainStaminaOverTimeToPercent(float percent, float percentPerSecond)
    {
        if(AssertIsDead("Can't gain stamina when dead.")) return;

        if(percent < 0)
        {
            Debug.LogWarning("This would deplete stamina, just use 'DepleteStaminaInstant()'");
            return;
        }

        float oldProjectedStamina = projectedStamina;

        float newProjectedStamina = MaxStaminaStat.Value * (percent / 100);
        newProjectedStamina = Mathf.Clamp(newProjectedStamina, 0, MaxStaminaStat.Value);

        if(newProjectedStamina < oldProjectedStamina)
        {
            Debug.LogWarning("Stamina is already higher than the percent you're trying to heal to.");
            return;
        }

        projectedStamina = newProjectedStamina;
        currentPercentPerSecondStaminaOverTime = percentPerSecond;
    }

    public IEnumerator GainStaminaOverTimeCoroutine()
    {
        while(!isDead)
        {
            if(currentStamina == projectedStamina)
            {
                yield return null;
                continue;
            }

            float lastCurrentStamina = currentHealth;
            lastCurrentStamina = CharacterStatsRoundingHelper.RoundValueUsingGlobalSettings(lastCurrentStamina);

            currentStamina = Mathf.MoveTowards(currentStamina, projectedStamina, MaxStaminaStat.Value * (currentPercentPerSecondStaminaOverTime / 100) * Time.deltaTime);

            lastTimeCurrentStaminaActivelyChanged = Time.time;

            EStaminaChangedOperation operation;

            if(currentStamina > lastCurrentStamina)
                operation = EStaminaChangedOperation.Gain;
            else
                operation = EStaminaChangedOperation.NoChange;

            float newCurrentStamina = currentStamina;
            newCurrentStamina = CharacterStatsRoundingHelper.RoundValueUsingGlobalSettings(newCurrentStamina);

            InvokeOnStaminaChanged(operation, newCurrentStamina - lastCurrentStamina);

            yield return null;
        }
    }

    protected void InvokeOnStaminaChanged(EStaminaChangedOperation operation, float staminaChangeAmount)
    {
        if(PlayerSpawner.Instance.EnableStaminaDebugMessages)
            Debug.Log($"Name: {GetStaminaName()} | Current stamina: {currentStamina}, Projected stamina: {projectedStamina}, Max stamina: {MaxStaminaStat.Value}");
    
        staminaChangeAmount = CharacterStatsRoundingHelper.RoundValueUsingGlobalSettings(staminaChangeAmount);
        OnStaminaChanged?.Invoke(currentStamina, projectedStamina, MaxStaminaStat.Value, operation, staminaChangeAmount);
    }

    public string GetStaminaName()
    {
        return playerName;
    }

    public IEnumerator StaminaRegenCoroutine()
    {
        while(!isDead)
        {
            if(currentStamina == MaxStaminaStat.Value || Time.time - lastTimeCurrentStaminaActivelyChanged < StaminaRegenCooldownStat.Value)
            {
                yield return null;
                continue;
            }

            float amount = MaxStaminaStat.Value * (StaminaRegenStat.Value / 100);

            if(amount > MaxStaminaStat.Value - projectedStamina)
                amount = MaxStaminaStat.Value - projectedStamina;

            if(amount < 0) continue;

            currentStamina += amount;
            currentStamina = Mathf.Clamp(currentStamina, 0, MaxStaminaStat.Value);
            projectedStamina = currentStamina;

            InvokeOnStaminaChanged(EStaminaChangedOperation.StaminaRegenGain, amount);

            yield return new WaitForSeconds(1);
        }
    }

#endregion

}
