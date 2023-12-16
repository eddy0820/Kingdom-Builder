using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using KinematicCharacterController;
using System;

public class PlayerStatUI : StaminaDamageableStatUI
{
    [Header("Single Target Health Bar")]
    [SerializeField] BarUI singleTargetHealthBarUI;
    [SerializeField] TextMeshProUGUI singleTargetNameText;

    PlayerController PlayerController => PlayerController.Instance;
    PlayerCanvas PlayerCanvas => PlayerController.UICanvas;
    PlayerStats PlayerStats => PlayerController.PlayerStats;
    KinematicCharacterMotor Motor => PlayerController.Character.Motor;

    protected override CharacterStats CharacterStats => PlayerStats;
    protected override IDamageable IDamageable => PlayerStats;
    protected override Transform DamageNumberSpawnTransform => Motor.Transform;
    protected override Vector3 DamageNumberSpawnPosition => DamageNumberSpawnTransform.position + new Vector3(0f, Motor.Capsule.height, 0f);
    protected override Stat MaxHealthStat => PlayerStats.GetStatFromName[CommonStatTypeNames.MaxHealth];

    protected override Stat MaxStaminaStat => PlayerStats.GetStatFromName[CommonStatTypeNames.MaxStamina];
    protected override IStamina IStamina => PlayerStats;

    protected override void OnAwake()
    {
        base.OnAwake();

        singleTargetHealthBarUI.BarTransform.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if(PlayerSpawner.Instance.ShowPlayerHealthAndStaminaText)
        {
            if(!healthBarUI.Text.gameObject.activeSelf)
                healthBarUI.Text.gameObject.SetActive(true);

            if(!staminaBarUI.Text.gameObject.activeSelf)
                staminaBarUI.Text.gameObject.SetActive(true);
        }
        else
        {
            if(healthBarUI.Text.gameObject.activeSelf)
                healthBarUI.Text.gameObject.SetActive(false);

            if(staminaBarUI.Text.gameObject.activeSelf)
                staminaBarUI.Text.gameObject.SetActive(false);
        }
    }

#region Health Stuff

    protected override void OnHealthChanged(float currentHealth, float projectedHealth, float maxHealth, EHealthChangedOperation operation = EHealthChangedOperation.NoChange, float healthChangeAmount = 0)
    {
        base.OnHealthChanged(currentHealth, projectedHealth, maxHealth, operation, healthChangeAmount);

        if(PlayerCanvas.BuildMenuEnabled)
            PlayerCanvas.ToggleBuildMenu();
    }

#endregion

#region Single Target Health Bar Stuff

    private void OnSingleTargetHealthChanged(float currentHealth, float projectedHealth, float maxHealth, EHealthChangedOperation operation = EHealthChangedOperation.NoChange, float healthChangeAmount = 0)
    {
        singleTargetHealthBarUI.UpdateBar(currentHealth, projectedHealth, maxHealth);
    }

    private void OnSingleTargetStatModifierChanged(Stat stat, StatModifier statModifier, EStatModifierChangedOperation operation)
    {
        if(stat.type != MaxHealthStat.type) return;
        
        OnSingleTargetHealthChanged(IDamageable.GetCurrentHealth(), IDamageable.GetProjectedHealth(), stat.Value);
    }

    public void ToggleSingleTargetHealthBar(bool b, CharacterStats stats, IDamageable damageable)
    {
        singleTargetHealthBarUI.BarTransform.gameObject.SetActive(b);

        if(b)
        {
            singleTargetNameText.text = damageable.GetDamageableName();
            OnSingleTargetHealthChanged(damageable.GetCurrentHealth(), damageable.GetProjectedHealth(), stats.GetStatFromName[CommonStatTypeNames.MaxHealth].Value);

            damageable.OnHealthChanged += OnSingleTargetHealthChanged;
            stats.OnStatModifierChanged += OnSingleTargetStatModifierChanged;
        }
        else
        {
            damageable.OnHealthChanged -= OnSingleTargetHealthChanged;
            stats.OnStatModifierChanged -= OnSingleTargetStatModifierChanged;
        }
    }

#endregion
}
