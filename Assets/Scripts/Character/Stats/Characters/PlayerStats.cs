using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class PlayerStats : CharacterStats, IDamageable
{
    float m_currentHealth;
    float currentHealth
    {
        get => m_currentHealth;
        set
        {
            m_currentHealth = value;
            if(m_currentHealth <= 0)
                Die();
        }
    }
    
    float projectedHealth;
    float currentPercentPerSecond;
    float lastTimeCurrentHealthActivelyChanged;

    Stat MaxHealthStat => getStatFromName[CommonStatTypeNames.MaxHealth];
    Stat HealthRegenStat => getStatFromName[CommonStatTypeNames.HealthRegen];
    Stat OutOfCombatHealthRegenCooldownStat => getStatFromName[CommonStatTypeNames.OutOfCombatHealthRegenCooldown];

    public Action<float, float, float> OnHealthChanged { get => OnHealthChangedInternal; set => OnHealthChangedInternal = value; }
    private Action<float, float, float> OnHealthChangedInternal;

    bool isDead = false;

    public PlayerStats(BaseStatsSO _baseStatsSO) : base(_baseStatsSO) {}

    public void SetHealth(float amount)
    {
        if(AssertIsDead("Can't set health when dead.")) return;

        if(amount <= 0)
        {
            Debug.LogWarning("Can't set health to 0 or less, just use 'Die()'");
            return;
        }

        projectedHealth = amount;
        currentHealth = amount;

        InvokeOnHealthChanged();
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetProjectedHealth()
    {
        return projectedHealth;
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

        InvokeOnHealthChanged();
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

        InvokeOnHealthChanged();
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

        currentPercentPerSecond = percentPerSecond;
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

        if(projectedHealth < oldProjectedHealth)
        {
            Debug.LogWarning("Health is already higher than the percent you're trying to heal to.");
            return;
        }

        projectedHealth = newProjectedHealth;
        currentPercentPerSecond = percentPerSecond;
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

            currentHealth = Mathf.MoveTowards(currentHealth, projectedHealth, MaxHealthStat.Value * (currentPercentPerSecond / 100) * Time.deltaTime);
            lastTimeCurrentHealthActivelyChanged = Time.time;
            InvokeOnHealthChanged();

            yield return null;
        }
    }

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

            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealthStat.Value);
            projectedHealth = currentHealth;

            InvokeOnHealthChanged();

            yield return new WaitForSeconds(1);
        }
    }

    private void InvokeOnHealthChanged()
    {
        Debug.Log($"Current health: {currentHealth}, Projected health: {projectedHealth}, Max health: {MaxHealthStat.Value}");
        OnHealthChanged?.Invoke(currentHealth, projectedHealth, MaxHealthStat.Value);
    }

    public void Die()
    {
        isDead = true;
        Debug.Log("Player died");
    } 

    public bool IsDead()
    {
        return isDead;
    }

    private bool AssertIsDead(string debugMessage)
    {
        if(isDead)
        {
            Debug.LogWarning(debugMessage);
            return true;
        }

        return false;
    }
}
