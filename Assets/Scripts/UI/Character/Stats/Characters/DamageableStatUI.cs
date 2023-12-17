using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
using NaughtyAttributes;

public abstract class DamageableStatUI : MonoBehaviour
{   
    [Header("Health UI")]
    [SerializeField] protected BarUI healthBarUI;

    [Header("Health Damage Popups")]
    [SerializeField] protected float damagePopupMinThreshold = 1f;
    [Space(10)]
    [SerializeField] protected DamageNumberMesh takeDamageNumberMesh;
    [SerializeField] protected DamageNumberMesh healNumberMesh;
    [SerializeField] protected DamageNumberMesh increaseMaxHealthNumberMesh;
    [SerializeField] protected DamageNumberMesh decreaseMaxHealthNumberMesh;
    [SerializeField] protected DamageNumberMesh healthRegenHealNumberMesh;
    protected float currentHealthWaitingToBeShownHealOperation = 0;
    protected float currentHealthWaitingToBeShownHealthRegenOperation = 0;

    protected abstract Transform DamageNumberSpawnTransform { get; }
    protected abstract Vector3 DamageNumberSpawnPosition { get; }

    protected abstract Stat MaxHealthStat { get; }

    protected abstract CharacterStats CharacterStats { get; }
    protected abstract IDamageable IDamageable { get; }

    private void Awake()
    {
        healthBarUI.SetupBarUI(ReturnAdditionalHealthBarFadeCooldown);

        IDamageable.OnHealthChanged += OnHealthChanged;
        CharacterStats.OnStatModifierChanged += OnStatModifierChangedHealthChanged;
    
        OnAwake();
    }
    
    protected abstract void OnAwake();

    private float ReturnAdditionalHealthBarFadeCooldown()
    {
        if(CharacterStats.CharacterHasStats(CommonStatTypeNames.OutOfCombatHealthRegenCooldown, CommonStatTypeNames.HealthRegen))
        {
            if(IDamageable.GetCurrentHealth() != MaxHealthStat.Value)
                return CharacterStats.GetStatFromName[CommonStatTypeNames.OutOfCombatHealthRegenCooldown].Value;
        }

        return 0;
    }

    protected virtual void OnHealthChanged(float currentHealth, float projectedHealth, float maxHealth, EHealthChangedOperation operation = EHealthChangedOperation.NoChange, float healthChangeAmount = 0)
    {
        if(operation is not EHealthChangedOperation.HealthRegenHeal) currentHealthWaitingToBeShownHealthRegenOperation = 0;
        
        DoDamagePopup(operation, healthChangeAmount);

        healthBarUI.UpdateBar(currentHealth, projectedHealth, maxHealth);
    }

    protected void OnStatModifierChangedHealthChanged(Stat stat, StatModifier statModifier, EStatModifierChangedOperation operation)
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

    protected virtual void DoDamagePopup(EHealthChangedOperation eHealthChangedOperation, float healthChangeAmount)
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
                damageNumberMesh = HeathRegenOperation(ref healthChangeAmount);
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
        currentHealthWaitingToBeShownHealOperation += healthChangeAmount;

        float threshold = MaxHealthStat.Value / 100;
        threshold = Mathf.Clamp(threshold, damagePopupMinThreshold, Mathf.Infinity);
        
        if(IDamageable.GetCurrentHealth() != IDamageable.GetProjectedHealth())
        {
            if(currentHealthWaitingToBeShownHealOperation < threshold)
                return null;
        }
        else
        {
            currentHealthWaitingToBeShownHealOperation = CharacterStatsRoundingHelper.RoundValueUsingGlobalSettings(currentHealthWaitingToBeShownHealOperation);

            if(currentHealthWaitingToBeShownHealOperation == 0)
                return null;
        }

        float leftOverHealth = currentHealthWaitingToBeShownHealOperation % threshold;
        
        if(leftOverHealth > 0) currentHealthWaitingToBeShownHealOperation -= leftOverHealth;

        healthChangeAmount = currentHealthWaitingToBeShownHealOperation;

        currentHealthWaitingToBeShownHealOperation = 0;

        if(leftOverHealth > 0) 
        {
            if(IDamageable.GetCurrentHealth() == IDamageable.GetProjectedHealth())
            {
                healthChangeAmount += leftOverHealth;
            }
            else
            {
                currentHealthWaitingToBeShownHealOperation += leftOverHealth;
            }
        }

        return healNumberMesh;
    } 

    protected virtual DamageNumberMesh HeathRegenOperation(ref float healthChangeAmount)
    {
        currentHealthWaitingToBeShownHealthRegenOperation += healthChangeAmount;
        
        Stat healthRegenStat = CharacterStats.GetStatFromName[CommonStatTypeNames.HealthRegen];
        float threshold = healthRegenStat.Value;
        threshold = Mathf.Clamp(threshold, damagePopupMinThreshold, Mathf.Infinity);
        
        if(IDamageable.GetCurrentHealth() == MaxHealthStat.Value)
        { 
            currentHealthWaitingToBeShownHealthRegenOperation = CharacterStatsRoundingHelper.RoundValueUsingGlobalSettings(currentHealthWaitingToBeShownHealthRegenOperation);

            if(currentHealthWaitingToBeShownHealthRegenOperation == 0)
                return null;
        }
        else
        {
            if(currentHealthWaitingToBeShownHealthRegenOperation < threshold)
                return null;
        }

        float leftOverHealth = currentHealthWaitingToBeShownHealthRegenOperation % threshold;
        
        if(leftOverHealth > 0) currentHealthWaitingToBeShownHealthRegenOperation -= leftOverHealth;

        healthChangeAmount = currentHealthWaitingToBeShownHealthRegenOperation;

        currentHealthWaitingToBeShownHealthRegenOperation = 0;

        if(leftOverHealth > 0) 
        {
            if(IDamageable.GetCurrentHealth() == MaxHealthStat.Value)
            {
                healthChangeAmount += leftOverHealth;
            }
            else
            {
                currentHealthWaitingToBeShownHealthRegenOperation += leftOverHealth;
            }
        }

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
}


[Serializable]
public class BarUI
{
    [SerializeField] protected bool fadeBar;

    [AllowNesting]
    [SerializeField, ShowIf("fadeBar")] TweenedUIComponent barFade;
    public TweenedUIComponent BarFade => barFade;
    [Space(10)]
    [SerializeField] RectMask2D barMask;
    [SerializeField] RectMask2D barGhostMask;
    [SerializeField] TextMeshProUGUI text;
    public TextMeshProUGUI Text => text;
    [Space(10)]
    [SerializeField] float barRightPaddingMin = 15;
    [SerializeField] float barRightPaddingMax = 390;
    [Space(10)]
    [AllowNesting]
    [SerializeField, ShowIf("fadeBar")] float numSecondsAfterShowToDoCallbackValueChanged = 0.1f;
    [AllowNesting]
    [SerializeField, ShowIf("fadeBar")] float numSecondsToWaitBeforeHidingBar = 2f;

    protected Sequence currentBarFadeSequence;

    Func<float> ReturnAdditionalBarFadeCooldown;

    public void SetupBarUI(Func<float> returnAdditionalBarFadeCooldown)
    {
        ReturnAdditionalBarFadeCooldown = returnAdditionalBarFadeCooldown;
    }

    public void UpdateBar(float currentValue, float projectedValue, float maxValue)
    {
        if(fadeBar)
        {
            ShowThenHideFadeTweenUIComponent(barFade, () =>
            {
                SetMaskAndText(currentValue, projectedValue, maxValue);
            });
        }
        else
        {
            SetMaskAndText(currentValue, projectedValue, maxValue);
        }
    }

    private void SetMaskAndText(float currentValue, float projectedValue, float maxValue)
    {
        float projectedValuePercentage = projectedValue / maxValue;
        float currentValuePercentage = currentValue / maxValue;

        barGhostMask.padding = new Vector4(barGhostMask.padding.x, barGhostMask.padding.y, Mathf.Lerp(barRightPaddingMax, barRightPaddingMin, projectedValuePercentage), barGhostMask.padding.w);
        barMask.padding = new Vector4(barMask.padding.x, barMask.padding.y, Mathf.Lerp(barRightPaddingMax, barRightPaddingMin, currentValuePercentage), barMask.padding.w);

        UpdateText(currentValue, maxValue, text);
    }

    protected void ShowThenHideFadeTweenUIComponent(TweenedUIComponent tweenedUIComponent, Action actionToDoOnShow)
    {
        Tween fadeTween = tweenedUIComponent.Tweens.Find(t => t.TweenValues.TweenType == ETweenType.Fade);
        if(fadeTween == null) return;

        currentBarFadeSequence?.Kill();
        tweenedUIComponent.CurrentSequence?.Kill();
        fadeTween.TweenValues.CanvasGroup.DOKill();

        bool doActionBeforeFade = fadeTween.TweenValues.CanvasGroup.alpha == fadeTween.TweenValues.FadeValues.StartAlpha;

        if(doActionBeforeFade)
        {
            actionToDoOnShow?.Invoke();
        }

        fadeTween.TweenValues.CanvasGroup.alpha = fadeTween.TweenValues.FadeValues.StartAlpha;

        currentBarFadeSequence = DOTween.Sequence();
        if(!doActionBeforeFade) 
        {   
            currentBarFadeSequence.AppendInterval(numSecondsAfterShowToDoCallbackValueChanged);
            currentBarFadeSequence.AppendCallback(() => actionToDoOnShow?.Invoke());
        }

        float cooldown = numSecondsToWaitBeforeHidingBar;

        cooldown += ReturnAdditionalBarFadeCooldown?.Invoke() ?? 0;

        currentBarFadeSequence.AppendInterval(cooldown);
        currentBarFadeSequence.AppendCallback(() => tweenedUIComponent.TweenUIComponent(true, new(){ETweenType.Fade}, false));
        currentBarFadeSequence.Play();
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
