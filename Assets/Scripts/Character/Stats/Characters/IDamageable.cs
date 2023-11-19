using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void SetHealth(float amount);
    public float GetHealth();
    public void TakeDamage(float damage);
    public void Heal(float amount);
    public void Die();

    public Action<float, float, float, EHealthChangedOperation> OnHealthChanged { get; set; }
}
