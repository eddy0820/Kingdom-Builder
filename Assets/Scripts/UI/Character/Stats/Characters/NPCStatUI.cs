using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class NPCStatUI : StaminaDamageableStatUI
{
    [Space(15)]

    [SerializeField] Targetable targetable;
    [SerializeField] StaminaDamageableCharacterStats characterStats;
    protected override CharacterStats CharacterStats => characterStats;
    protected override IDamageable IDamageable => characterStats;
    protected override Transform DamageNumberSpawnTransform => transform;
    protected override Vector3 DamageNumberSpawnPosition => DamageNumberSpawnTransform.position;
    protected override Stat MaxHealthStat => CharacterStats.GetStatFromName[CommonStatTypeNames.MaxHealth];

    protected override Stat MaxStaminaStat => CharacterStats.GetStatFromName[CommonStatTypeNames.MaxStamina];
    protected override IStamina IStamina => characterStats;

    LockedOnCharacterControllerState lockedOnCharacterControllerState;
    PlayerController PlayerController => PlayerController.Instance;
    PlayerCanvas PlayerCanvas => PlayerController.UICanvas;

    [Space(15)]

    [SerializeField] LayerMask raycastLayerMask;

    bool doRaycast = true;

    GameObject HealthBarFadeGameObj => healthBarUI.BarFade.GameObj;
    GameObject HealthBarTextGameObj => healthBarUI.Text.gameObject;

    protected override void OnAwake()
    {
        healthBarUI.BarFade.Tweens.Find(t => t.TweenValues.TweenType == ETweenType.Fade).TweenValues.CanvasGroup.alpha = 0;
    }

    private void Start()
    {
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
                if(HealthBarFadeGameObj.activeSelf) HealthBarFadeGameObj.SetActive(false);
            }
        }
        else
        {
            if(!HealthBarFadeGameObj.activeSelf) HealthBarFadeGameObj.SetActive(true);
        }
    }

    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform.position);
        transform.Rotate(0, 180, 0);

        if(GameSettings.Instance.ShowNonPlayerHealthAndStaminaText)
        {
            if(!HealthBarTextGameObj.activeSelf)
                HealthBarTextGameObj.SetActive(true);
        }
        else
        {
            if(HealthBarTextGameObj.activeSelf)
                HealthBarTextGameObj.SetActive(false);
        }
    }

    private void OnAquiredTargetLockedOnState(Targetable aquiuredTarget)
    {
        if(aquiuredTarget != targetable) return;

        doRaycast = false;
        HealthBarFadeGameObj.SetActive(false);

        PlayerCanvas.PlayerStatUI.ToggleSingleTargetHealthBar(true, characterStats);
    }

    private void OnLostTargetLockedOnState(Targetable lostTarget)
    {
        if(lostTarget != targetable) return;

        doRaycast = true;
        HealthBarFadeGameObj.SetActive(true);

        PlayerCanvas.PlayerStatUI.ToggleSingleTargetHealthBar(false, characterStats);
    }

    protected override void OnHealthChanged(float currentHealth, float projectedHealth, float maxHealth, EHealthChangedOperation operation = EHealthChangedOperation.NoChange, float healthChangeAmount = 0)
    {
        if(operation is not EHealthChangedOperation.HealthRegenHeal) currentHealthWaitingToBeShownHealthRegenOperation = 0;

        DoDamagePopup(operation, healthChangeAmount);

        if(operation is EHealthChangedOperation.NoChange) return;

        healthBarUI.UpdateBar(currentHealth, projectedHealth, maxHealth);
    }

#region Damage Popup Stuff

    protected override void DoDamagePopup(EHealthChangedOperation eHealthChangedOperation, float healthChangeAmount)
    {
        if(!GameSettings.Instance.ShowNonPlayerDamagePopups) return;

        base.DoDamagePopup(eHealthChangedOperation, healthChangeAmount);
    }

    protected override void DoMaxHealthChangePopup(EStatModifierChangedOperation eStatModifierChangedOperation, float healthChangeAmount)
    {
        if(!GameSettings.Instance.ShowNonPlayerDamagePopups) return;

        base.DoMaxHealthChangePopup(eStatModifierChangedOperation, healthChangeAmount);
    }

    protected override void DoMaxStaminaChangePopup(EStatModifierChangedOperation eStatModifierChangedOperation, float staminaChangeAmount)
    {
        if(!GameSettings.Instance.ShowNonPlayerDamagePopups) return;

        base.DoMaxStaminaChangePopup(eStatModifierChangedOperation, staminaChangeAmount);
    }

#endregion

}
