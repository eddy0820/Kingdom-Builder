using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
using NaughtyAttributes;

public abstract class StatUI
{
    [Header("Health UI")]
    [SerializeField] protected TweenedUIComponent healthHUDFade;
    public TweenedUIComponent HealthHUDFade => healthHUDFade;
    [Space(10)]
    [SerializeField] protected RectMask2D healthBarMask;
    [SerializeField] protected RectMask2D healthBarGhostMask;
    [SerializeField] protected TextMeshProUGUI healthText;
    public TextMeshProUGUI HealthText => healthText;
    [Space(10)]
    [SerializeField] protected float healthBarRightPaddingMin = 15;
    [SerializeField] protected float healthBarRightPaddingMax = 390;
    [Space(10)]
    [SerializeField] protected float numSecondsAfterShowToDoCallbackHealthChanged = 0.1f;
    [SerializeField] protected float numSecondsToWaitBeforeHidingHealthBar = 2f;
    protected Sequence currentHealthBarFadeSequence;

    [Header("Damage Popups")]
    [BoxGroup("Damage Popups"), SerializeField] protected float damagePopupMinThreshold = 1f;
    [Space(10)]
    [BoxGroup("Damage Popups"), SerializeField] protected DamageNumberMesh takeDamageNumberMesh;
    [BoxGroup("Damage Popups"), SerializeField] protected DamageNumberMesh healNumberMesh;
    [BoxGroup("Damage Popups"), SerializeField] protected DamageNumberMesh increaseMaxHealthNumberMesh;
    [BoxGroup("Damage Popups"), SerializeField] protected DamageNumberMesh decreaseMaxHealthNumberMesh;
    [BoxGroup("Damage Popups"), SerializeField] protected DamageNumberMesh healthRegenHealNumberMesh;
    protected float currentHealthWaitingToBeShown = 0;

    protected abstract Transform DamageNumberSpawnTransform { get; }
    protected abstract Vector3 DamageNumberSpawnPosition { get; }

    protected abstract Stat MaxHealthStat { get; }

    protected abstract CharacterStats CharacterStats { get; }
    protected abstract IDamageable IDamageable { get; }

    public virtual void OnHealthChanged(float currentHealth, float projectedHealth, float maxHealth, EHealthChangedOperation operation = EHealthChangedOperation.NoChange, float healthChangeAmount = 0)
    {
        DoDamagePopup(operation, healthChangeAmount);

        UpdateHealthBar(currentHealth, projectedHealth, maxHealth);
    }

    protected void UpdateHealthBar(float currentHealth, float projectedHealth, float maxHealth)
    {
        ShowThenHideFadeTweenUIComponentHealthBar(healthHUDFade, () =>
        {
            float projectedHealthPercentage = projectedHealth / maxHealth;
            float currentHealthPercentage = currentHealth / maxHealth;

            healthBarGhostMask.padding = new Vector4(healthBarGhostMask.padding.x, healthBarGhostMask.padding.y, Mathf.Lerp(healthBarRightPaddingMax, healthBarRightPaddingMin, projectedHealthPercentage), healthBarGhostMask.padding.w);
            healthBarMask.padding = new Vector4(healthBarMask.padding.x, healthBarMask.padding.y, Mathf.Lerp(healthBarRightPaddingMax, healthBarRightPaddingMin, currentHealthPercentage), healthBarMask.padding.w);
            
            UpdateText(currentHealth, maxHealth, healthText);
        });
    }

    public void OnStatModifierChangedHealthChanged(Stat stat, StatModifier statModifier, EStatModifierChangedOperation operation)
    {
        if(stat.type != MaxHealthStat.type) return;

        float maxHealthChangedAmount;

        switch(operation)
        {
            case EStatModifierChangedOperation.Added:
                maxHealthChangedAmount = statModifier.value;
            break;

            case EStatModifierChangedOperation.Removed:
            case EStatModifierChangedOperation.RemovedAllFromSource:
                maxHealthChangedAmount = -statModifier.value;
            break;

            default:
                return;
        }

        DoMaxHealthChangePopup(operation, maxHealthChangedAmount);
        
        OnHealthChanged(IDamageable.GetCurrentHealth(), IDamageable.GetProjectedHealth(), stat.Value);
    }

    protected virtual void ShowThenHideFadeTweenUIComponentHealthBar(TweenedUIComponent tweenedUIComponent, Action actionToDoOnShow)
    {
        Tween fadeTween = tweenedUIComponent.Tweens.Find(t => t.TweenValues.TweenType == ETweenType.Fade);
        if(fadeTween == null) return;

        currentHealthBarFadeSequence?.Kill();
        tweenedUIComponent.CurrentSequence?.Kill();
        fadeTween.TweenValues.CanvasGroup.DOKill();

        bool doActionBeforeFade = fadeTween.TweenValues.CanvasGroup.alpha == fadeTween.TweenValues.FadeValues.StartAlpha;

        if(doActionBeforeFade)
        {
            actionToDoOnShow?.Invoke();
        }

        fadeTween.TweenValues.CanvasGroup.alpha = fadeTween.TweenValues.FadeValues.StartAlpha;

        currentHealthBarFadeSequence = DOTween.Sequence();
        if(!doActionBeforeFade) 
        {   
            currentHealthBarFadeSequence.AppendInterval(numSecondsAfterShowToDoCallbackHealthChanged);
            currentHealthBarFadeSequence.AppendCallback(() => actionToDoOnShow?.Invoke());
        }

        float cooldown = numSecondsToWaitBeforeHidingHealthBar;

        if(CharacterStats.CharacterHasStats(CommonStatTypeNames.OutOfCombatHealthRegenCooldown, CommonStatTypeNames.HealthRegen))
        {
            if(IDamageable.GetCurrentHealth() != MaxHealthStat.Value)
                cooldown += CharacterStats.GetStatFromName[CommonStatTypeNames.OutOfCombatHealthRegenCooldown].Value;
        }

        currentHealthBarFadeSequence.AppendInterval(cooldown);
        currentHealthBarFadeSequence.AppendCallback(() => tweenedUIComponent.TweenUIComponent(true, new(){ETweenType.Fade}, false));
        currentHealthBarFadeSequence.Play();
    }

    protected void DoDamagePopup(EHealthChangedOperation eHealthChangedOperation, float healthChangeAmount)
    {
        DamageNumberMesh damageNumberMesh;
        
        switch(eHealthChangedOperation)
        {
            case EHealthChangedOperation.TakeDamage:
                damageNumberMesh = TakeDamageOperation(ref healthChangeAmount);
            break;

            case EHealthChangedOperation.Heal:
                damageNumberMesh = HealOperation(ref healthChangeAmount);
            break;

            case EHealthChangedOperation.HealthRegenHeal:
                damageNumberMesh = HealOverTimeOperation(ref healthChangeAmount);
            break;

            case EHealthChangedOperation.NoChange:
            default:
                return;
        }

        if(damageNumberMesh == null) return;

        damageNumberMesh.digitSettings.decimals = CharacterStatsRoundingHelper.GlobalNumDecimals;

        damageNumberMesh.Spawn(DamageNumberSpawnPosition, healthChangeAmount);
    }

    protected virtual DamageNumberMesh TakeDamageOperation(ref float healthChangeAmount)
    {           
        if(healthChangeAmount > 0)
            healthChangeAmount *= -1;

        return takeDamageNumberMesh;
    }  

    protected virtual DamageNumberMesh HealOperation(ref float healthChangeAmount)
    {
        currentHealthWaitingToBeShown += healthChangeAmount;
        currentHealthWaitingToBeShown = Mathf.Round(currentHealthWaitingToBeShown * 10f) / 10f;

        int threshold = (int)(MaxHealthStat.Value / 100);
        threshold = (int)Mathf.Clamp(threshold, damagePopupMinThreshold, 10);

        if(currentHealthWaitingToBeShown < threshold && IDamageable.GetCurrentHealth() != IDamageable.GetProjectedHealth())
            return null;

        float leftOverHealth = Mathf.Round(currentHealthWaitingToBeShown % damagePopupMinThreshold * 10f) / 10f;
        
        if(leftOverHealth > 0) currentHealthWaitingToBeShown -= leftOverHealth;

        healthChangeAmount = currentHealthWaitingToBeShown;

        currentHealthWaitingToBeShown = 0;

        if(leftOverHealth > 0) 
        {
            if(IDamageable.GetCurrentHealth() == IDamageable.GetProjectedHealth())
            {
                healthChangeAmount += leftOverHealth;
            }
            else
            {
                currentHealthWaitingToBeShown += leftOverHealth;
            }
        }

        return healNumberMesh;
    } 

    protected virtual DamageNumberMesh HealOverTimeOperation(ref float healthChangeAmount)
    {
        return healthRegenHealNumberMesh;
    }

    protected virtual void DoMaxHealthChangePopup(EStatModifierChangedOperation eStatModifierChangedOperation, float healthChangeAmount)
    {
        DamageNumberMesh damageNumberMesh;
        
        switch(eStatModifierChangedOperation)
        {
            case EStatModifierChangedOperation.Added:
                damageNumberMesh = increaseMaxHealthNumberMesh;
            break;

            case EStatModifierChangedOperation.Removed:
            case EStatModifierChangedOperation.RemovedAllFromSource:
                damageNumberMesh = decreaseMaxHealthNumberMesh;
            break;
            
            default:
                return;
        }

        damageNumberMesh.digitSettings.decimals = CharacterStatsRoundingHelper.GlobalNumDecimals;

        damageNumberMesh.Spawn(DamageNumberSpawnPosition, healthChangeAmount);
    }

    protected void UpdateText(float currentValue, float maxValue, TextMeshProUGUI text)
    {
        string maxValueString = maxValue % 1 == 0
        ? maxValue.ToString("F0")
        : maxValue.ToString(CharacterStatsRoundingHelper.GlobalValueString);

        string currentValueString = currentValue % 1 == 0
        ? currentValue.ToString("F0")
        : currentValue.ToString(CharacterStatsRoundingHelper.GlobalValueString);

        text.text = currentValueString + " / " + maxValueString;
    }
}
