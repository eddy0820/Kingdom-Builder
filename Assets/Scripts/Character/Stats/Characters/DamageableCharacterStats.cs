using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Burst.Intrinsics;


[Serializable]
public abstract class DamageableCharacterStats : CharacterStats, IDamageable
{   
    float m_currentHealth;
    protected float currentHealth
    {
        get
        {
            if(m_currentHealth > MaxHealthStat.Value)
                m_currentHealth = MaxHealthStat.Value;

            return m_currentHealth;
        }
        set
        {
            m_currentHealth = value;
            if(m_currentHealth <= 0)
                Die();
        }
    }

    float m_projectedHealth;
    
    protected float projectedHealth
    {
        get 
        {
            if(m_projectedHealth > MaxHealthStat.Value)
                m_projectedHealth = MaxHealthStat.Value;

            return m_projectedHealth;
        }
        set
        {
            m_projectedHealth = value;
        }
    }
    protected float currentPercentPerSecondHealthOverTime;
    protected float lastTimeCurrentHealthActivelyChanged;

    protected Stat MaxHealthStat => getStatFromName[CommonStatTypeNames.MaxHealth];

    public Action<float, float, float, EHealthChangedOperation, float> OnHealthChanged { get => OnHealthChangedInternal; set => OnHealthChangedInternal = value; }
    private Action<float, float, float, EHealthChangedOperation, float> OnHealthChangedInternal;

    protected bool isDead = false;

    protected override void OnStart()
    {
        StartCoroutine(HealOverTimeCoroutine());
        StartCoroutine(HealthRegenCoroutine());
        SetHealth(MaxHealthStat.Value, true);
    }

    public void SetHealth(float amount, bool setAsNoChange = false)
    {
        if(AssertIsDead("Can't set health when dead.")) return;

        if(amount <= 0)
        {
            Debug.LogWarning("Can't set health to 0 or less, just use 'Die()'");
            return;
        }

        float oldCurrentHealth = currentHealth;

        projectedHealth = amount;
        currentHealth = amount;

        EHealthChangedOperation operation;

        if(currentHealth > oldCurrentHealth && !setAsNoChange)
            operation = EHealthChangedOperation.Heal;
        else if(currentHealth < oldCurrentHealth && !setAsNoChange)
            operation = EHealthChangedOperation.TakeDamage;
        else
            operation = EHealthChangedOperation.NoChange;

        InvokeOnHealthChanged(operation, Mathf.Abs(currentHealth - oldCurrentHealth));
    }

    public float GetCurrentHealth()
    {
        return CharacterStatsRoundingHelper.RoundValueUsingGlobalSettings(currentHealth);
    }

    public float GetProjectedHealth()
    {
        return CharacterStatsRoundingHelper.RoundValueUsingGlobalSettings(projectedHealth);
    }

    public void TakeDamageInstant(float damage)
    {
        if(AssertIsDead("Can't take damage when dead.")) return;

        if(damage < 0)
        {
            Debug.LogWarning("This would heal, just use 'Heal()'");
            return;
        }

        projectedHealth -= damage;

        if(projectedHealth <= currentHealth)
        {   
            currentHealth = projectedHealth;
            lastTimeCurrentHealthActivelyChanged = Time.time;
        }    

        InvokeOnHealthChanged(EHealthChangedOperation.TakeDamage, damage);
    }

    public void HealInstant(float amount)
    {
        if(AssertIsDead("Can't heal when dead.")) return;

        if(amount < 0)
        {
            Debug.LogWarning("This would deal damage, just use 'TakeDamage()'");
            return;
        }

        if(amount > MaxHealthStat.Value - projectedHealth)
            amount = MaxHealthStat.Value - projectedHealth;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealthStat.Value);
        lastTimeCurrentHealthActivelyChanged = Time.time;

        if(projectedHealth <= currentHealth)
            projectedHealth = currentHealth;

        InvokeOnHealthChanged(EHealthChangedOperation.Heal, amount);
    }

    public void HealOverTime(float percentAmount, float percentPerSecond)
    {
        if(AssertIsDead("Can't heal when dead.")) return;

        if(percentAmount < 0)
        {
            Debug.LogWarning("This would deal damage, just use 'TakeDamage()'");
            return;
        }

        float amount = MaxHealthStat.Value * (percentAmount / 100);
        if(amount > MaxHealthStat.Value - projectedHealth)
            amount = MaxHealthStat.Value - projectedHealth;

        projectedHealth += amount;
        projectedHealth = Mathf.Clamp(projectedHealth, 0, MaxHealthStat.Value);

        currentPercentPerSecondHealthOverTime = percentPerSecond;
    }

    public void HealOverTimeToPercent(float percent, float percentPerSecond)
    {
        if(AssertIsDead("Can't heal when dead.")) return;

        if(percent < 0)
        {
            Debug.LogWarning("This would deal damage, just use 'TakeDamage()'");
            return;
        }

        float oldProjectedHealth = projectedHealth;

        float newProjectedHealth = MaxHealthStat.Value * (percent / 100);
        newProjectedHealth = Mathf.Clamp(newProjectedHealth, 0, MaxHealthStat.Value);

        if(newProjectedHealth < oldProjectedHealth)
        {
            Debug.LogWarning("Health is already higher than the percent you're trying to heal to.");
            return;
        }

        projectedHealth = newProjectedHealth;
        currentPercentPerSecondHealthOverTime = percentPerSecond;
    }

    public IEnumerator HealOverTimeCoroutine()
    {
        while(!isDead)
        {
            if(currentHealth == projectedHealth)
            {
                yield return null;
                continue;
            }

            float lastCurrentHealth = GetCurrentHealth();

            currentHealth = Mathf.MoveTowards(currentHealth, projectedHealth, MaxHealthStat.Value * (currentPercentPerSecondHealthOverTime / 100) * Time.deltaTime);

            lastTimeCurrentHealthActivelyChanged = Time.time;

            EHealthChangedOperation operation;

            if(currentHealth > lastCurrentHealth)
                operation = EHealthChangedOperation.Heal;
            else
                operation = EHealthChangedOperation.NoChange;

            float newCurrentHealth = currentHealth;
            newCurrentHealth = CharacterStatsRoundingHelper.RoundValueUsingGlobalSettings(newCurrentHealth);

            InvokeOnHealthChanged(operation, newCurrentHealth - lastCurrentHealth);

            yield return null;
        }
    }

    public IEnumerator HealthRegenCoroutine()
    {
        while (!isDead)
        {
            if(!CharacterHasStats(CommonStatTypeNames.HealthRegen, CommonStatTypeNames.OutOfCombatHealthRegenCooldown))
            {
                yield return null;
                continue;
            }

            Stat HealthRegenStat = getStatFromName[CommonStatTypeNames.HealthRegen];
            Stat OutOfCombatHealthRegenCooldownStat = getStatFromName[CommonStatTypeNames.OutOfCombatHealthRegenCooldown];

            if(currentHealth == MaxHealthStat.Value || Time.time - lastTimeCurrentHealthActivelyChanged < OutOfCombatHealthRegenCooldownStat.Value)
            {
                yield return null;
                continue;
            }

            float amount = HealthRegenStat.Value * Time.deltaTime;

            if(amount > MaxHealthStat.Value - projectedHealth)
                amount = MaxHealthStat.Value - projectedHealth;

            if(amount < 0) continue;

            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealthStat.Value);
            projectedHealth = currentHealth;

            InvokeOnHealthChanged(EHealthChangedOperation.HealthRegenHeal, amount);

            yield return null; 
        }
    }

    protected void InvokeOnHealthChanged(EHealthChangedOperation operation, float healthChangeAmount)
    {
        if(GameSettings.Instance.EnableHealthDebugMessages)
            Debug.Log($"Name: {GetDamageableName()} | Current health: {GetCurrentHealth()}, Projected health: {GetProjectedHealth()}, Max health: {MaxHealthStat.Value}");
        
        OnHealthChanged?.Invoke(currentHealth, projectedHealth, MaxHealthStat.Value, operation, healthChangeAmount);
    }

    public void Die()
    {
        isDead = true;

        if(GameSettings.Instance.EnableHealthDebugMessages)
            Debug.Log($"{GetDamageableName()} died");
    } 

    public bool IsDead()
    {
        return isDead;
    }

    protected bool AssertIsDead(string debugMessage)
    {
        if(isDead)
        {
            Debug.LogWarning(debugMessage);
            return true;
        }

        return false;
    }

    public string GetDamageableName()
    {
        return gameObject.name;
    }
}
