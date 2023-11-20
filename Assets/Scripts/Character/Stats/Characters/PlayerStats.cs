using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Xml;
using DG.Tweening.Core;

[Serializable]
public class PlayerStats : CharacterStats, IDamageable
{
    float currentHealth;

    public Action<float, float, float, EHealthChangedOperation> OnHealthChanged { get => OnHealthChangedInternal; set => OnHealthChangedInternal = value; }
    private Action<float, float, float, EHealthChangedOperation> OnHealthChangedInternal;

    Tweener healthOverTimeTweener;

    public PlayerStats(BaseStatsSO _baseStatsSO) : base(_baseStatsSO) {}

    public void SetHealth(float amount)
    {
        EHealthChangedOperation operation;

        if(amount > currentHealth)
            operation = EHealthChangedOperation.HealInstant;
        else if(amount < currentHealth)
            operation = EHealthChangedOperation.Damage;
        else
            operation = EHealthChangedOperation.NoChange;


        float oldHealth = currentHealth;
        currentHealth = amount;

        OnHealthChanged?.Invoke(oldHealth, currentHealth, getStatFromName[CommonStatTypeNames.MaxHealth].Value, operation);
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(float damage)
    {
        if(damage < 0)
        {
            Debug.LogWarning("This would heal, just use 'Heal()'");
            return;
        }
        
        float newHealth = currentHealth - damage;
        float oldHealth = currentHealth;

        EHealthChangedOperation operation = EHealthChangedOperation.Damage;
        if(oldHealth == newHealth) operation = EHealthChangedOperation.NoChange;

        currentHealth = newHealth;

        OnHealthChanged?.Invoke(oldHealth, currentHealth, getStatFromName[CommonStatTypeNames.MaxHealth].Value, operation);

        if(currentHealth <= 0)
            Die();
    }

    public void HealInstant(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("This would deal damage, just use 'TakeDamage()'");
            return;
        }

        Stat maxHealthStat = getStatFromName[CommonStatTypeNames.MaxHealth];

        if(amount > maxHealthStat.Value - currentHealth)
            amount = maxHealthStat.Value - currentHealth;

        float newHealth = currentHealth + amount;
        newHealth = Mathf.Clamp(newHealth, 0, maxHealthStat.Value);
        float oldHealth = currentHealth;

        EHealthChangedOperation operation = EHealthChangedOperation.HealInstant;
        if(oldHealth == newHealth) operation = EHealthChangedOperation.NoChange;

        currentHealth = newHealth;
        
        OnHealthChanged?.Invoke(oldHealth, currentHealth, maxHealthStat.Value, operation);
    }

    public void HealOverTime(float percentAmount, float duration)
    {
        if(percentAmount < 0)
        {
            Debug.LogWarning("This would deal damage, just use 'TakeDamage()'");
            return;
        }

        Stat maxHealthStat = getStatFromName[CommonStatTypeNames.MaxHealth];

        float amount = maxHealthStat.Value * (percentAmount / 100);

        if(amount > maxHealthStat.Value - currentHealth)
            amount = maxHealthStat.Value - currentHealth;

        float newHealth = currentHealth + amount;
        newHealth = Mathf.Clamp(newHealth, 0, maxHealthStat.Value);
        float oldHealth = currentHealth;

        EHealthChangedOperation operation = EHealthChangedOperation.HealOverTime;
        if(oldHealth == newHealth) operation = EHealthChangedOperation.NoChange;
        
        healthOverTimeTweener?.Kill();

        healthOverTimeTweener = DOTween.To(() => currentHealth, x => 
        {
            OnHealthChanged?.Invoke(x, newHealth, maxHealthStat.Value, operation);
            currentHealth = x;
        }, newHealth, duration).SetEase(Ease.Linear).OnComplete(() => currentHealth = newHealth);

        healthOverTimeTweener.Play();
    }

    public void HealOverTimeToPercent(float percent, float duration)
    {
        if(percent < 0)
        {
            Debug.LogWarning("This would deal damage, just use 'TakeDamage()'");
            return;
        }

        Stat maxHealthStat = getStatFromName[CommonStatTypeNames.MaxHealth];

        float amount = maxHealthStat.Value * (percent / 100);

        if(amount > maxHealthStat.Value - currentHealth)
            amount = maxHealthStat.Value - currentHealth;

        float newHealth = amount;
        newHealth = Mathf.Clamp(newHealth, 0, maxHealthStat.Value);
        float oldHealth = currentHealth;

        if(newHealth < oldHealth)
        {
            Debug.LogWarning("This would deal damage, just use 'TakeDamage()'");
            return;
        }

        EHealthChangedOperation operation = EHealthChangedOperation.HealOverTime;
        if(oldHealth == newHealth) operation = EHealthChangedOperation.NoChange;
        
        healthOverTimeTweener?.Kill();

        healthOverTimeTweener = DOTween.To(() => currentHealth, x => 
        {
            OnHealthChanged?.Invoke(x, newHealth, maxHealthStat.Value, operation);
            currentHealth = x;
        }, newHealth, duration).SetEase(Ease.Linear).OnComplete(() => currentHealth = newHealth);

        healthOverTimeTweener.Play();
    }

    public void Die()
    {
        Debug.Log("Player died");
    } 
}
