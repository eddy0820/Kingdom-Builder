using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void SetHealth(float amount);
    public float GetHealth();
    public void TakeDamage(float damage);
    public void HealInstant(float amount);
    public void HealOverTime(float percentAmount, float duration); 
    public void HealOverTimeToPercent(float percent, float duration);
    public void Die();

    public Action<float, float, float, EHealthChangedOperation> OnHealthChanged { get; set; }
}
