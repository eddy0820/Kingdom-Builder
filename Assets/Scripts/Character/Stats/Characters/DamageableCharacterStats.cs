using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[System.Serializable]
public abstract class DamageableCharacterStats : CharacterStats, IDamageable
{
    float m_currentHealth;
    protected float currentHealth
    {
        get => m_currentHealth;
        set
        {
            //m_currentHealth = Mathf.Round(value * 10) / 10f;
            m_currentHealth = value;
            if(m_currentHealth <= 0)
                Die();
        }
    }
    
    protected float projectedHealth;
    protected float currentPercentPerSecond;
    protected float lastTimeCurrentHealthActivelyChanged;

    protected Stat MaxHealthStat => getStatFromName[CommonStatTypeNames.MaxHealth];

    public Action<float, float, float, EHealthChangedOperation, float> OnHealthChanged { get => OnHealthChangedInternal; set => OnHealthChangedInternal = value; }
    private Action<float, float, float, EHealthChangedOperation, float> OnHealthChangedInternal;

    protected bool isDead = false;

    public DamageableCharacterStats(BaseStatsSO _baseStatsSO) : base(_baseStatsSO) {}

    public void SetHealth(float amount)
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

        if(currentHealth > oldCurrentHealth)
            operation = EHealthChangedOperation.Heal;
        else if(currentHealth < oldCurrentHealth)
            operation = EHealthChangedOperation.TakeDamage;
        else
            operation = EHealthChangedOperation.NoChange;

        InvokeOnHealthChanged(operation, Mathf.Abs(currentHealth - oldCurrentHealth));
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetRoundedCurrentHealth()
    {
        return Mathf.Round(currentHealth * 10) / 10f;
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

            float lastCurrentHealth = currentHealth;
            lastCurrentHealth = Mathf.Round(lastCurrentHealth * 10) / 10f;

            currentHealth = Mathf.MoveTowards(currentHealth, projectedHealth, MaxHealthStat.Value * (currentPercentPerSecond / 100) * Time.deltaTime);

            lastTimeCurrentHealthActivelyChanged = Time.time;

            EHealthChangedOperation operation;

            if(currentHealth > lastCurrentHealth)
                operation = EHealthChangedOperation.Heal;
            else
                operation = EHealthChangedOperation.NoChange;

            float newCurrentHealth = currentHealth;
            newCurrentHealth = Mathf.Round(newCurrentHealth * 10) / 10f;

            InvokeOnHealthChanged(operation, newCurrentHealth - lastCurrentHealth);

            yield return null;
        }
    }

    protected void InvokeOnHealthChanged(EHealthChangedOperation operation, float healthChangeAmount)
    {
        if(PlayerSpawner.Instance.EnableHealthDebugMessages)
            Debug.Log($"Name: {GetDamageableName()} | Current health: {currentHealth}, Projected health: {projectedHealth}, Max health: {MaxHealthStat.Value}");
        
        Debug.Log(healthChangeAmount);
        healthChangeAmount = Mathf.Round(healthChangeAmount * 10) / 10f;
        Debug.Log("new " + healthChangeAmount);
        OnHealthChanged?.Invoke(currentHealth, projectedHealth, MaxHealthStat.Value, operation, healthChangeAmount);
    }

    public void Die()
    {
        isDead = true;

        if(PlayerSpawner.Instance.EnableHealthDebugMessages)
            Debug.Log($"{GetDamageableName()} died");
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

    public string GetDamageableName()
    {
        return GetDamageableNameInternal();
    }

    protected abstract string GetDamageableNameInternal();
}
