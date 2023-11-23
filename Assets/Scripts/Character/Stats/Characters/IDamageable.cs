using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void SetHealth(float amount);

    public float GetCurrentHealth();
    public float GetProjectedHealth();

    public void TakeDamageInstant(float damage);

    public void HealInstant(float amount);
    public void HealOverTime(float percentAmount, float duration); 
    public void HealOverTimeToPercent(float percent, float duration);
    public IEnumerator HealOverTimeCoroutine();
    
    public void Die();
    public bool IsDead();

    public Action<float, float, float> OnHealthChanged { get; set; }
}
