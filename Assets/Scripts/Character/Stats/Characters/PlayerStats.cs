using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class PlayerStats : DamageableCharacterStats
{
    Stat HealthRegenStat => getStatFromName[CommonStatTypeNames.HealthRegen];
    Stat OutOfCombatHealthRegenCooldownStat => getStatFromName[CommonStatTypeNames.OutOfCombatHealthRegenCooldown];

    public PlayerStats(BaseStatsSO _baseStatsSO) : base(_baseStatsSO) {}

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
}
