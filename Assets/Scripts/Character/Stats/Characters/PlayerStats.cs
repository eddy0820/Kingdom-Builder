using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStats : CharacterStats, IDamageable
{
    float currentHealth;

    public Action<float, float> OnHealthChanged { get => OnHealthChangedInternal; set => OnHealthChangedInternal = value; }
    private Action<float, float> OnHealthChangedInternal;

    public PlayerStats(BaseStatsSO _baseStatsSO) : base(_baseStatsSO) {}

    public void SetHealth(float amount)
    {
        currentHealth = amount;
        OnHealthChanged?.Invoke(currentHealth, getStatFromName[CommonStatTypeNames.MaxHealth].Value);
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

        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth, getStatFromName[CommonStatTypeNames.MaxHealth].Value);

        if(currentHealth <= 0)
            Die();
    }

    public void Heal(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("This would deal damage, just use 'TakeDamage()'");
            return;
        }

        Stat maxHealthStat = getStatFromName[CommonStatTypeNames.MaxHealth];

        if(amount > maxHealthStat.Value - currentHealth)
            amount = maxHealthStat.Value - currentHealth;

        currentHealth += amount;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealthStat.Value);
        OnHealthChanged?.Invoke(currentHealth, maxHealthStat.Value);
    }

    public void Die()
    {
        Debug.Log("Player died");
    } 
}
