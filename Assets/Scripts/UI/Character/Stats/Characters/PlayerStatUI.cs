using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using KinematicCharacterController;
using System;

public class PlayerStatUI : DamageableStatUI
{
    [Header("Stamina UI")]
    [SerializeField] TweenedUIComponent staminaHUDFade;
    [Space(10)]
    [SerializeField] RectMask2D staminaBarMask;
    [SerializeField] RectMask2D staminaBarGhostMask;
    [SerializeField] TextMeshProUGUI staminaText;
    [Space(10)]
    [SerializeField] float staminaBarRightPaddingMin = 15;
    [SerializeField] float staminaBarRightPaddingMax = 390;
    [Space(10)]
    [SerializeField] float numSecondsAfterShowToDoCallbackStaminaChanged = 0.1f;
    [SerializeField] float numSecondsToWaitBeforeHidingStaminaBar = 2f;
    Sequence currentStaminaBarFadeSequence;

    [Header("Single Target Health Bar")]
    [SerializeField] Transform singleTargetHealthHUD;
    [SerializeField] RectMask2D singleTargetHealthBarMask;
    [SerializeField] RectMask2D singleTargetHealthBarGhostMask;
    [SerializeField] TextMeshProUGUI singleTargetHealthText;
    [SerializeField] TextMeshProUGUI singleTargetNameText;

    [Space(10)]

    [SerializeField] float singleTargetHealthBarRightPaddingMin = 15;
    [SerializeField] float singleTargetHealthBarRightPaddingMax = 390;

    PlayerController PlayerController => PlayerController.Instance;
    PlayerCanvas PlayerCanvas => PlayerController.UICanvas;
    PlayerStats PlayerStats => PlayerController.PlayerStats;
    KinematicCharacterMotor Motor => PlayerController.Character.Motor;

    protected override CharacterStats CharacterStats => PlayerController.PlayerStats;
    protected override IDamageable IDamageable => PlayerController.PlayerStats;
    protected override Transform DamageNumberSpawnTransform => Motor.Transform;
    protected override Vector3 DamageNumberSpawnPosition => DamageNumberSpawnTransform.position + new Vector3(0f, Motor.Capsule.height, 0f);
    protected override Stat MaxHealthStat => PlayerStats.GetStatFromName[CommonStatTypeNames.MaxHealth];

    protected Stat OutOfCombatHealthRegenCooldownStat => PlayerStats.GetStatFromName[CommonStatTypeNames.OutOfCombatHealthRegenCooldown];

    IStamina IStamina => PlayerController.PlayerStats;
    Stat MaxStaminaStat => PlayerStats.GetStatFromName[CommonStatTypeNames.MaxStamina];
    Stat StaminaRegenStat => PlayerStats.GetStatFromName[CommonStatTypeNames.StaminaRegen];

    protected override void OnAwake()
    {
        PlayerStats.OnStaminaChanged += UpdateStaminaBar;
        PlayerStats.OnStatModifierChanged += OnStatModifierChangedStaminaChanged;

        singleTargetHealthHUD.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if(PlayerSpawner.Instance.ShowPlayerHealthAndStaminaText)
        {
            if(!healthBarUI.Text.gameObject.activeSelf)
                healthBarUI.Text.gameObject.SetActive(true);

            if(!staminaText.gameObject.activeSelf)
                staminaText.gameObject.SetActive(true);
        }
        else
        {
            if(healthBarUI.Text.gameObject.activeSelf)
                healthBarUI.Text.gameObject.SetActive(false);

            if(staminaText.gameObject.activeSelf)
                staminaText.gameObject.SetActive(false);
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

    private void UpdateSingleTargetHealthBar(float currentHealth, float projectedHealth, float maxHealth, EHealthChangedOperation operation = EHealthChangedOperation.NoChange, float healthChangeAmount = 0)
    {
        float projectedHealthPercentage = projectedHealth / maxHealth;
        float currentHealthPercentage = currentHealth / maxHealth;

        singleTargetHealthBarGhostMask.padding = new Vector4(singleTargetHealthBarGhostMask.padding.x, singleTargetHealthBarGhostMask.padding.y, Mathf.Lerp(singleTargetHealthBarRightPaddingMax, singleTargetHealthBarRightPaddingMin, projectedHealthPercentage), singleTargetHealthBarGhostMask.padding.w);
        singleTargetHealthBarMask.padding = new Vector4(singleTargetHealthBarMask.padding.x, singleTargetHealthBarMask.padding.y, Mathf.Lerp(singleTargetHealthBarRightPaddingMax, singleTargetHealthBarRightPaddingMin, currentHealthPercentage), singleTargetHealthBarMask.padding.w);

        UpdateText(currentHealth, maxHealth, singleTargetHealthText);
    }

    private void OnSingleTargetStatModifierChanged(Stat stat, StatModifier statModifier, EStatModifierChangedOperation operation)
    {
        if(stat.type != MaxHealthStat.type) return;
        
        UpdateSingleTargetHealthBar(IDamageable.GetCurrentHealth(), IDamageable.GetProjectedHealth(), stat.Value);
    }

    public void ToggleSingleTargetHealthBar(bool b, CharacterStats stats, IDamageable damageable)
    {
        singleTargetHealthHUD.gameObject.SetActive(b);

        if(b)
        {
            singleTargetNameText.text = damageable.GetDamageableName();
            UpdateSingleTargetHealthBar(damageable.GetCurrentHealth(), damageable.GetProjectedHealth(), stats.GetStatFromName[CommonStatTypeNames.MaxHealth].Value);

            damageable.OnHealthChanged += UpdateSingleTargetHealthBar;
            stats.OnStatModifierChanged += OnSingleTargetStatModifierChanged;
        }
        else
        {
            damageable.OnHealthChanged -= UpdateSingleTargetHealthBar;
            stats.OnStatModifierChanged -= OnSingleTargetStatModifierChanged;
        }
    }

#endregion

#region Stamina Stuff

    public void UpdateStaminaBar(float currentStamina, float projectedStamina, float maxStamina, EStaminaChangedOperation operation = EStaminaChangedOperation.NoChange, float staminaChangeAmount = 0)
    {
        ShowThenHideFadeTweenUIComponentStaminaBar(staminaHUDFade, () =>
        {
            float projectedStaminaPercentage = projectedStamina / maxStamina;
            float currentStaminaPercentage = currentStamina / maxStamina;

            staminaBarGhostMask.padding = new Vector4(staminaBarGhostMask.padding.x, staminaBarGhostMask.padding.y, Mathf.Lerp(staminaBarRightPaddingMax, staminaBarRightPaddingMin, projectedStaminaPercentage), staminaBarGhostMask.padding.w);
            staminaBarMask.padding = new Vector4(staminaBarMask.padding.x, staminaBarMask.padding.y, Mathf.Lerp(staminaBarRightPaddingMax, staminaBarRightPaddingMin, currentStaminaPercentage), staminaBarMask.padding.w);
            
            UpdateText(currentStamina, maxStamina, staminaText);
        });
    }

    private void ShowThenHideFadeTweenUIComponentStaminaBar(TweenedUIComponent tweenedUIComponent, Action actionToDoOnShow)
    {
        Tween fadeTween = tweenedUIComponent.Tweens.Find(t => t.TweenValues.TweenType == ETweenType.Fade);
        if(fadeTween == null) return;

        currentStaminaBarFadeSequence?.Kill();
        tweenedUIComponent.CurrentSequence?.Kill();
        fadeTween.TweenValues.CanvasGroup.DOKill();

        bool doActionBeforeFade = fadeTween.TweenValues.CanvasGroup.alpha == fadeTween.TweenValues.FadeValues.StartAlpha;

        if(doActionBeforeFade)
        {
            actionToDoOnShow?.Invoke();
        }

        fadeTween.TweenValues.CanvasGroup.alpha = fadeTween.TweenValues.FadeValues.StartAlpha;

        currentStaminaBarFadeSequence = DOTween.Sequence();
        if(!doActionBeforeFade) 
        {   
            currentStaminaBarFadeSequence.AppendInterval(numSecondsAfterShowToDoCallbackStaminaChanged);
            currentStaminaBarFadeSequence.AppendCallback(() => actionToDoOnShow?.Invoke());
        }

        float cooldown = numSecondsToWaitBeforeHidingStaminaBar;
        if(IStamina.GetCurrentStamina() != MaxStaminaStat.Value)
            cooldown += StaminaRegenStat.Value;

        currentStaminaBarFadeSequence.AppendInterval(cooldown);
        currentStaminaBarFadeSequence.AppendCallback(() => tweenedUIComponent.TweenUIComponent(true, new(){ETweenType.Fade}, false));
        currentStaminaBarFadeSequence.Play();
    }

    public void OnStatModifierChangedStaminaChanged(Stat stat, StatModifier statModifier, EStatModifierChangedOperation operation)
    {
        if(stat.type != MaxStaminaStat.type) return;

        UpdateStaminaBar(IStamina.GetCurrentStamina(), IStamina.GetProjectedStamina(), stat.Value);
    }

#endregion

}
