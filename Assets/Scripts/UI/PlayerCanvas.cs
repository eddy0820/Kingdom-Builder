using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System;
using DamageNumbersPro;
using KinematicCharacterController;

public class PlayerCanvas : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] TweenedUIComponent healthHUD;
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
    [SerializeField] float numSecondsToWaitBeforeHidingHealthBarPadding = 2f;
    Sequence currentHealthBarFadeSequence;

    [HorizontalLine]

    [Header("Damage Popups")]

    [SerializeField] float damagePopupMinThreshold = 1f;
    [Space(10)]
    [SerializeField] DamageNumberMesh takeDamageNumberMesh;
    [SerializeField] DamageNumberMesh healNumberMesh;
    [SerializeField] DamageNumberMesh healthRegenHealNumberMesh;
    [SerializeField] DamageNumberMesh increaseMaxHealthNumberMesh;
    [SerializeField] DamageNumberMesh decreaseMaxHealthNumberMesh;

    float currentHealthWaitingToBeShown = 0;

    [Header("Interaction")]
    [SerializeField] TweenedUIComponent interactionCrosshair;
    [Space(10)]
    [SerializeField] List<InteractionEntry> interactionEntries;

    [HorizontalLine]

    [Header("Crosshair")]
    [SerializeField] TweenedUIComponent crosshair;

    [Header("Build Menu")]
    [SerializeField] TweenedUIComponent buildMenu;

    [Header("Build Hotbar")]
    [SerializeField] TweenedUIComponent buildHotbar;

    [Space(15)]

    [ReadOnly, SerializeField] bool buildMenuEnabled = false;
    public bool BuildMenuEnabled => buildMenuEnabled;

    [HorizontalLine]

    [Header("Single Target Health Bar")]
    [SerializeField] Transform singleTargetHealthHUD;
    [SerializeField] RectMask2D singleTargetHealthBarMask;
    [SerializeField] RectMask2D singleTargetHealthBarGhostMask;
    [SerializeField] TextMeshProUGUI singleTargetHealthText;
    [SerializeField] TextMeshProUGUI singleTargetNameText;

    [Space(10)]

    [SerializeField] float singleTargetHealthBarRightPaddingMin = 15;
    [SerializeField] float singleTargetHealthBarRightPaddingMax = 390;

    BuildHotbarInterface buildHotbarInterface;
    public BuildHotbarInterface BuildHotbarInterface => buildHotbarInterface;

    IDamageable playerStatsDamageable;
    PlayerStats playerStats;

    Stat MaxHealthStat => playerStats.GetStatFromName[CommonStatTypeNames.MaxHealth];
    Stat OutOfCombatHealthRegenCooldownStat => playerStats.GetStatFromName[CommonStatTypeNames.OutOfCombatHealthRegenCooldown];

    private void Awake()
    {
        buildHotbarInterface = buildHotbar.GameObj.GetComponent<BuildHotbarInterface>();

        buildMenu.GameObj.SetActive(false);
        buildHotbar.GameObj.SetActive(false);

        crosshair.GameObj.SetActive(false);
        crosshair.RectTransform.localScale = Vector3.zero;

        playerStats = PlayerController.Instance.Stats as PlayerStats;
        playerStatsDamageable = PlayerController.Instance.IDamageable;

        playerStatsDamageable.OnHealthChanged += UpdateHealthBar;
        playerStats.OnStatModifierChanged += OnStatModifierChanged;

        interactionCrosshair.GameObj.SetActive(false);
        HideInteractions();

        singleTargetHealthHUD.gameObject.SetActive(false);
    }

#region HUD

    public void UpdateHealthBar(float currentHealth, float projectedHealth, float maxHealth, EHealthChangedOperation operation = EHealthChangedOperation.NoChange, float healthChangeAmount = 0)
    {
        DoDamagePopup(operation, healthChangeAmount);

        ShowThenHideFadeTweenUIComponent(healthHUDFade, () =>
        {
            float projectedHealthPercentage = projectedHealth / maxHealth;
            float currentHealthPercentage = currentHealth / maxHealth;

            healthBarGhostMask.padding = new Vector4(healthBarGhostMask.padding.x, healthBarGhostMask.padding.y, Mathf.Lerp(healthBarRightPaddingMax, healthBarRightPaddingMin, projectedHealthPercentage), healthBarGhostMask.padding.w);
            healthBarMask.padding = new Vector4(healthBarMask.padding.x, healthBarMask.padding.y, Mathf.Lerp(healthBarRightPaddingMax, healthBarRightPaddingMin, currentHealthPercentage), healthBarMask.padding.w);
            
            healthText.text = currentHealth % 1 == 0
            ? currentHealth.ToString("F0") + " / " + maxHealth.ToString("F0")
            : currentHealth.ToString("F1") + " / " + maxHealth.ToString("F0");
        });

        if(buildMenuEnabled)
            ToggleBuildMenu();
    }

    public void OnStatModifierChanged(Stat stat, StatModifier statModifier, EStatModifierChangedOperation operation)
    {
        if(stat.type != MaxHealthStat.type) return;
        
        UpdateHealthBar(playerStatsDamageable.GetCurrentHealth(), playerStatsDamageable.GetProjectedHealth(), stat.Value);
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

        float cooldown = numSecondsToWaitBeforeHidingHealthBarPadding;
        if(playerStatsDamageable.GetCurrentHealth() != MaxHealthStat.Value)
            cooldown += OutOfCombatHealthRegenCooldownStat.Value;

        currentHealthBarFadeSequence.AppendInterval(cooldown);
        currentHealthBarFadeSequence.AppendCallback(() => tweenedUIComponent.TweenUIComponent(true, new(){ETweenType.Fade}, false));
        currentHealthBarFadeSequence.Play();
    }

#endregion

#region Interaction

    public Action GetInteractionEntryActionFromIndex(int index)
    {
        return interactionEntries[index].OnInteract;
    }
    public void ToggleInteractionCrosshair(bool b)
    {
        interactionCrosshair.TweenUIComponent(b);
    }

    public void ShowInteractions(IInteractable interactable, List<InteractionTypeSO> interactionTypes)
    {
        for(int i = 0; i < interactionTypes.Count; i++)
        {
            InteractionTypeSO interactionType = interactionTypes[i];
            InteractionEntry interactionEntry = interactionEntries[i];

            interactionEntry.SetInteraction(InputManager.Instance.GetEffectiveBindingPathForInteractionIndex(i), interactionType.Name, () => interactionType.Interact(interactable));
        }
    }

    public void HideInteractions()
    {
        interactionEntries.ForEach(entry => entry.HideInteraction());
    }

#endregion

#region Build Mode

    public void ToggleBuildMenu()
    {
        ToggleBuildMenu(!buildMenuEnabled);
        PlayerController.Instance.SoundController.PlayBuildMenuAppearanceSound(!buildMenuEnabled);
    }

    public void ToggleBuildMenu(bool b)
    {
        buildMenuEnabled = b;
        buildMenu.TweenUIComponent(b);
        ToggleBuildModeCrosshair(!b);
        healthHUD.GameObj.SetActive(!b);


        if(b) Cursor.lockState = CursorLockMode.None;
        else Cursor.lockState = CursorLockMode.Locked;
    }

    public void ToggleBuildHotbar(bool b)
    {
        buildHotbar.TweenUIComponent(b);
        healthHUD.TweenUIComponent(b, new(){ETweenType.MoveY}, false);
    }

    public void ToggleBuildModeCrosshair(bool b)
    {
        crosshair.TweenUIComponent(b);
    }

#endregion

#region Single Target Health Bar

    public void UpdateSingleTargetHealthBar(float currentHealth, float projectedHealth, float maxHealth, EHealthChangedOperation operation = EHealthChangedOperation.NoChange, float healthChangeAmount = 0)
    {
        float projectedHealthPercentage = projectedHealth / maxHealth;
        float currentHealthPercentage = currentHealth / maxHealth;

        singleTargetHealthBarGhostMask.padding = new Vector4(singleTargetHealthBarGhostMask.padding.x, singleTargetHealthBarGhostMask.padding.y, Mathf.Lerp(singleTargetHealthBarRightPaddingMax, singleTargetHealthBarRightPaddingMin, projectedHealthPercentage), singleTargetHealthBarGhostMask.padding.w);
        singleTargetHealthBarMask.padding = new Vector4(singleTargetHealthBarMask.padding.x, singleTargetHealthBarMask.padding.y, Mathf.Lerp(singleTargetHealthBarRightPaddingMax, singleTargetHealthBarRightPaddingMin, currentHealthPercentage), singleTargetHealthBarMask.padding.w);

        singleTargetHealthText.text = currentHealth % 1 == 0
        ? currentHealth.ToString("F0") + " / " + maxHealth.ToString("F0")
        : currentHealth.ToString("F1") + " / " + maxHealth.ToString("F0");
    }

    public void OnSingleTargetStatModifierChanged(Stat stat, StatModifier statModifier, EStatModifierChangedOperation operation)
    {
        if(stat.type != MaxHealthStat.type) return;
        
        UpdateSingleTargetHealthBar(playerStatsDamageable.GetCurrentHealth(), playerStatsDamageable.GetProjectedHealth(), stat.Value);
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

    private void DoDamagePopup(EHealthChangedOperation eHealthChangedOperation, float healthChangeAmount)
    {
        DamageNumberMesh damageNumberMesh;
        
        switch(eHealthChangedOperation)
        {
            case EHealthChangedOperation.TakeDamage:
                damageNumberMesh = takeDamageNumberMesh;
                
                if(healthChangeAmount > 0)
                    healthChangeAmount *= -1;
            break;

            case EHealthChangedOperation.Heal:

                currentHealthWaitingToBeShown += healthChangeAmount;
                currentHealthWaitingToBeShown = Mathf.Round(currentHealthWaitingToBeShown * 10f) / 10f;

                if(currentHealthWaitingToBeShown < damagePopupMinThreshold && playerStatsDamageable.GetRoundedCurrentHealth() != playerStatsDamageable.GetProjectedHealth())
                    return;

                float leftOverHealth = Mathf.Round(currentHealthWaitingToBeShown % damagePopupMinThreshold * 10f) / 10f;
                
                if(leftOverHealth > 0) currentHealthWaitingToBeShown -= leftOverHealth;

                healthChangeAmount = currentHealthWaitingToBeShown;

                currentHealthWaitingToBeShown = 0;

                if(leftOverHealth > 0) 
                {
                    if(playerStatsDamageable.GetRoundedCurrentHealth() == playerStatsDamageable.GetProjectedHealth())
                    {
                        healthChangeAmount += leftOverHealth;
                    }
                    else
                    {
                        currentHealthWaitingToBeShown += leftOverHealth;
                    }
                }

                damageNumberMesh = healNumberMesh;

            break;

            case EHealthChangedOperation.HealthRegenHeal:
                damageNumberMesh = healthRegenHealNumberMesh;
            break;

            case EHealthChangedOperation.NoChange:
            default:
                return;
        }

        KinematicCharacterMotor motor = PlayerController.Instance.Character.Motor;
        Transform damageNumberTransform = motor.Transform;
        damageNumberMesh.Spawn(damageNumberTransform.position + new Vector3(0f, motor.Capsule.height, 0f), healthChangeAmount);
    }

}
