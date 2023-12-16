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
    [Header("Single Target HUD")]
    [SerializeField] RectTransform singleTargetHUDTransform;
    [SerializeField] BarUI singleTargetHealthBarUI;
    [SerializeField] BarUI singleTargetStaminaBarUI;
    [SerializeField] TextMeshProUGUI singleTargetNameText;
    [SerializeField] RectTransform staminaBarUITransform;

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

        singleTargetHUDTransform.gameObject.SetActive(false);
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

#region Single Target HUD Stuff

    private void OnSingleTargetHealthChanged(float currentHealth, float projectedHealth, float maxHealth, EHealthChangedOperation operation = EHealthChangedOperation.NoChange, float healthChangeAmount = 0)
    {
        singleTargetHealthBarUI.UpdateBar(currentHealth, projectedHealth, maxHealth);
    }

    private void OnSingleTargetHealthStatModifierChanged(Stat stat, StatModifier statModifier, EStatModifierChangedOperation operation)
    {
        if(stat.type != MaxHealthStat.type) return;
        
        OnSingleTargetHealthChanged(IDamageable.GetCurrentHealth(), IDamageable.GetProjectedHealth(), stat.Value);
    }

    private void OnSingleTargetStaminaChanged(float currentStamina, float projectedStamina, float maxStamina, EStaminaChangedOperation operation = EStaminaChangedOperation.NoChange, float staminaChangeAmount = 0)
    {
        singleTargetStaminaBarUI.UpdateBar(currentStamina, projectedStamina, maxStamina);
    }

    private void OnSingleTargetStaminaStatModifierChanged(Stat stat, StatModifier statModifier, EStatModifierChangedOperation operation)
    {
        if(stat.type != MaxStaminaStat.type) return;

        OnSingleTargetStaminaChanged(IStamina.GetCurrentStamina(), IStamina.GetProjectedStamina(), stat.Value);
    }

    public void ToggleSingleTargetHealthBar(bool b, CharacterStats stats)
    {
        if(stats is not IDamageable damageable) return;

        singleTargetHUDTransform.gameObject.SetActive(b);

        bool updateStaminaBar = false;
        IStamina stamina = null;

        if(stats is IStamina istamina)
        {
            stamina = istamina;
            updateStaminaBar = true;
        }

        if(b)
        {
            singleTargetNameText.text = damageable.GetDamageableName();
            OnSingleTargetHealthChanged(damageable.GetCurrentHealth(), damageable.GetProjectedHealth(), stats.GetStatFromName[CommonStatTypeNames.MaxHealth].Value);

            damageable.OnHealthChanged += OnSingleTargetHealthChanged;
            stats.OnStatModifierChanged += OnSingleTargetHealthStatModifierChanged;

            if(updateStaminaBar)
            {
                staminaBarUITransform.gameObject.SetActive(true);

                OnSingleTargetStaminaChanged(stamina.GetCurrentStamina(), stamina.GetProjectedStamina(), stats.GetStatFromName[CommonStatTypeNames.MaxStamina].Value);

                stamina.OnStaminaChanged += OnSingleTargetStaminaChanged;
                stats.OnStatModifierChanged += OnSingleTargetStaminaStatModifierChanged;
            }
            else
            {
                staminaBarUITransform.gameObject.SetActive(false);
            }
        }
        else
        {
            damageable.OnHealthChanged -= OnSingleTargetHealthChanged;
            stats.OnStatModifierChanged -= OnSingleTargetHealthStatModifierChanged;

            if(updateStaminaBar)
            {
                stamina.OnStaminaChanged -= OnSingleTargetStaminaChanged;
                stats.OnStatModifierChanged -= OnSingleTargetStaminaStatModifierChanged;
            }
        }
    }

#endregion
}
