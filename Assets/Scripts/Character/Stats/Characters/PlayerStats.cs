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

    Stat maxHealthStat => getStatFromName[CommonStatTypeNames.MaxHealth];

    public Action<float, float, float> OnHealthChanged { get => OnHealthChangedInternal; set => OnHealthChangedInternal = value; }
    private Action<float, float, float> OnHealthChangedInternal;

    Tweener healthOverTimeTweener;

    public PlayerStats(BaseStatsSO _baseStatsSO) : base(_baseStatsSO) {}

    public void SetHealth(float amount)
    {
        projectedHealth = amount;
        SetCurrentHealthToProjected();
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
        if(damage < 0)
        {
            Debug.LogWarning("This would heal, just use 'Heal()'");
            return;
        }
        
        projectedHealth -= damage;
        
        SetCurrentHealthToProjected();
    }

    public void HealInstant(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("This would deal damage, just use 'TakeDamage()'");
            return;
        }

        if(amount > maxHealthStat.Value - projectedHealth)
            amount = maxHealthStat.Value - projectedHealth;

        projectedHealth += amount;
        projectedHealth = Mathf.Clamp(projectedHealth, 0, maxHealthStat.Value);

        SetCurrentHealthToProjected();
    }

    public void HealOverTime(float percentAmount, float percentPerSecond)
    {
        if(percentAmount < 0)
        {
            Debug.LogWarning("This would deal damage, just use 'TakeDamage()'");
            return;
        }

        float amount = maxHealthStat.Value * (percentAmount / 100);
        if(amount > maxHealthStat.Value - projectedHealth)
            amount = maxHealthStat.Value - projectedHealth;

        projectedHealth += amount;
        projectedHealth = Mathf.Clamp(projectedHealth, 0, maxHealthStat.Value);

        float duration = percentAmount / percentPerSecond;

        SetCurrentHealthToProjected(duration);
    }

    public void HealOverTimeToPercent(float percent, float percentPerSecond)
    {
        if(percent < 0)
        {
            Debug.LogWarning("This would deal damage, just use 'TakeDamage()'");
            return;
        }

        float oldProjectedHealth = projectedHealth;

        projectedHealth = maxHealthStat.Value * (percent / 100);
        projectedHealth = Mathf.Clamp(projectedHealth, 0, maxHealthStat.Value);

        if(projectedHealth < oldProjectedHealth)
        {
            Debug.LogWarning("Health is already higher than the percent you're trying to heal to.");
            return;
        }

        float duration = percent / percentPerSecond;

        SetCurrentHealthToProjected(duration);
    }

    private void SetCurrentHealthToProjected(float duration = 0)
    {
        if(healthOverTimeTweener.IsActive() && healthOverTimeTweener.IsPlaying())
        {
            

            healthOverTimeTweener.ChangeEndValue(projectedHealth, true);
            return;
        }

        oldProjectedHealth = projectedHealth;

        healthOverTimeTweener = DOTween.To(() => currentHealth, x => 
        {
            currentHealth = x;
            DebugHealth();
            OnHealthChangedInternal?.Invoke(currentHealth, projectedHealth, maxHealthStat.Value);
        }, projectedHealth, duration).SetEase(Ease.Linear).OnComplete(() => currentHealth = projectedHealth);

        healthOverTimeTweener.Play();
    }

    float oldProjectedHealth;


    private void DebugHealth()
    {
        Debug.Log($"Current health: {currentHealth}, Projected health: {projectedHealth}, Max health: {maxHealthStat.Value}");
    }

    public void Die()
    {
        healthOverTimeTweener?.Kill();
        Debug.Log("Player died");
    } 
}
