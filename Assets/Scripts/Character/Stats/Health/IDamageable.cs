using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;

public interface IDamageable
{
    public void SetHealth(float amount, bool setAsNoChange = false);

    public float GetCurrentHealth();
    public float GetRoundedCurrentHealth();
    public float GetProjectedHealth();

    public void TakeDamageInstant(float damage);

    public void HealInstant(float amount);
    public void HealOverTime(float percentAmount, float duration); 
    public void HealOverTimeToPercent(float percent, float duration);
    public IEnumerator HealOverTimeCoroutine();
    
    public void Die();
    public bool IsDead();

    public Action<float, float, float, EHealthChangedOperation, float> OnHealthChanged { get; set; }

    public string GetDamageableName();
}
