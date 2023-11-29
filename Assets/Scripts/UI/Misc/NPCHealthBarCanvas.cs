using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class NPCHealthBarCanvas : MonoBehaviour
{
    [SerializeField] TweenedUIComponent healthHUDFade;
    [Space(10)]
    [SerializeField] RectMask2D healthBarMask;
    [SerializeField] RectMask2D healthBarGhostMask;
    [SerializeField] TextMeshProUGUI healthText;
    [Space(10)]
    [SerializeField] float healthBarRightPaddingMin = 15;
    [SerializeField] float healthBarRightPaddingMax = 390;
    [Space(10)]
    [SerializeField] float numSecondsAfterShowToDoCallback = 0.25f;
    [SerializeField] float numSecondsToWaitBeforeHidingHealthBar = 2f;
    Sequence currentHealthBarFadeSequence;

    [Space(15)]

    [SerializeField] MonoBehaviour ITargetableMono;
    ITargetable ITargetable => ITargetableMono as ITargetable;
    [SerializeField] MonoBehaviour IHoldStatsMono;
    IHoldStats IHoldStats => IHoldStatsMono as IHoldStats;
    IDamageable IDamageable => IHoldStats.IDamageable;
    CharacterStats Stats => IHoldStats.Stats;

    LockedOnCharacterControllerState lockedOnCharacterControllerState;
    PlayerController PlayerController => PlayerController.Instance;
    PlayerCanvas PlayerCanvas => PlayerController.UICanvas;

    [Space(15)]

    [SerializeField] LayerMask raycastLayerMask;

    bool doRaycast = true;

    private void Start()
    {
        IDamageable.OnHealthChanged += UpdateHealthBar;
        Stats.OnStatModifierChanged += OnStatModifierChanged;

        healthHUDFade.Tweens.Find(t => t.TweenValues.TweenType == ETweenType.Fade).TweenValues.CanvasGroup.alpha = 0;

        PlayerController.StateMachine.GetState(out lockedOnCharacterControllerState);

        lockedOnCharacterControllerState.OnAcquiredTarget += OnAquiredTargetLockedOnState;
        lockedOnCharacterControllerState.OnLostTarget += OnLostTargetLockedOnState;
    }

    private void FixedUpdate()
    {
        if(!doRaycast) return;

        Ray ray = new(transform.position, Camera.main.transform.position - transform.position);
        
        if(Physics.Raycast(ray, out RaycastHit hit, Vector3.Distance(transform.position, Camera.main.transform.position), raycastLayerMask))
        {
            if(hit.collider.gameObject != null)
            {
                if(healthHUDFade.GameObj.activeSelf) healthHUDFade.GameObj.SetActive(false);
            }
        }
        else
        {
            if(!healthHUDFade.GameObj.activeSelf) healthHUDFade.GameObj.SetActive(true);
        }
    }

    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform.position);
        transform.Rotate(0, 180, 0);
    }

    private void OnAquiredTargetLockedOnState(ITargetable aquiuredTarget)
    {
        if(aquiuredTarget != ITargetable) return;

        doRaycast = false;
        healthHUDFade.GameObj.SetActive(false);

        PlayerCanvas.ToggleSingleTargetHealthBar(true, Stats, IDamageable);
    }

    private void OnLostTargetLockedOnState(ITargetable lostTarget)
    {
        if(lostTarget != ITargetable) return;

        doRaycast = true;
        healthHUDFade.GameObj.SetActive(true);

        PlayerCanvas.ToggleSingleTargetHealthBar(false, Stats, IDamageable);
    }

    public void UpdateHealthBar(float currentHealth, float projectedHealth, float maxHealth, EHealthChangedOperation operation = EHealthChangedOperation.NoChange, float healthChangeAmount = 0)
    {
        ShowThenHideFadeTweenUIComponent(healthHUDFade, () =>
        {
            float projectedHealthPercentage = projectedHealth / maxHealth;
            float currentHealthPercentage = currentHealth / maxHealth;

            healthBarGhostMask.padding = new Vector4(healthBarGhostMask.padding.x, healthBarGhostMask.padding.y, Mathf.Lerp(healthBarRightPaddingMax, healthBarRightPaddingMin, projectedHealthPercentage), healthBarGhostMask.padding.w);
            healthBarMask.padding = new Vector4(healthBarMask.padding.x, healthBarMask.padding.y, Mathf.Lerp(healthBarRightPaddingMax, healthBarRightPaddingMin, currentHealthPercentage), healthBarMask.padding.w);
            healthText.text = currentHealth.ToString("F1") + " / " + maxHealth.ToString("F0");

            healthText.text = currentHealth % 1 == 0
            ? currentHealth.ToString("F0") + " / " + maxHealth.ToString("F0")
            : currentHealth.ToString("F1") + " / " + maxHealth.ToString("F0");
        });
    }

    public void OnStatModifierChanged(Stat stat, StatModifier statModifier, EStatModifierChangedOperation operation)
    {
        if(stat.type != Stats.GetStatTypeFromName[CommonStatTypeNames.MaxHealth]) return;
        
        UpdateHealthBar(IDamageable.GetRoundedCurrentHealth(), IDamageable.GetProjectedHealth(), stat.Value);
    }

    public void ShowThenHideFadeTweenUIComponent(TweenedUIComponent tweenedUIComponent, Action actionToDoOnShow)
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
            currentHealthBarFadeSequence.AppendInterval(numSecondsAfterShowToDoCallback);
            currentHealthBarFadeSequence.AppendCallback(() => actionToDoOnShow?.Invoke());
        }
        currentHealthBarFadeSequence.AppendInterval(numSecondsToWaitBeforeHidingHealthBar);
        currentHealthBarFadeSequence.AppendCallback(() => tweenedUIComponent.TweenUIComponent(true, new(){ETweenType.Fade}, false));
        currentHealthBarFadeSequence.Play();
    }
}
