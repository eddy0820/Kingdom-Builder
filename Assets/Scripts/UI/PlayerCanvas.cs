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
    
    [Header("")]
    [SerializeField] PlayerStatUI playerStatUI;
    public PlayerStatUI PlayerStatUI => playerStatUI;

    [HorizontalLine]

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

    BuildHotbarInterface buildHotbarInterface;
    public BuildHotbarInterface BuildHotbarInterface => buildHotbarInterface;

    IDamageable PlayerStatsDamageable => PlayerController.Instance.IDamageable;
    PlayerStats PlayerStats => PlayerController.Instance.Stats as PlayerStats;

    private void Awake()
    {
        buildHotbarInterface = buildHotbar.GameObj.GetComponent<BuildHotbarInterface>();

        buildMenu.GameObj.SetActive(false);
        buildHotbar.GameObj.SetActive(false);

        crosshair.GameObj.SetActive(false);
        crosshair.RectTransform.localScale = Vector3.zero;

        PlayerStatsDamageable.OnHealthChanged += playerStatUI.UpdateHealthBar;
        PlayerStats.OnStatModifierChanged += playerStatUI.OnStatModifierChanged;

        interactionCrosshair.GameObj.SetActive(false);
        HideInteractions();

        playerStatUI.SingleTargetHealthHUD.gameObject.SetActive(false);
    }

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

}

[Serializable]
public class PlayerStatUI : StatUI
{
    [BoxGroup("Damage Popups"), SerializeField] DamageNumberMesh healthRegenHealNumberMesh;

    [Header("Single Target Health Bar")]
    [SerializeField] Transform singleTargetHealthHUD;
    public Transform SingleTargetHealthHUD => singleTargetHealthHUD;
    [SerializeField] RectMask2D singleTargetHealthBarMask;
    [SerializeField] RectMask2D singleTargetHealthBarGhostMask;
    [SerializeField] TextMeshProUGUI singleTargetHealthText;
    [SerializeField] TextMeshProUGUI singleTargetNameText;

    [Space(10)]

    [SerializeField] float singleTargetHealthBarRightPaddingMin = 15;
    [SerializeField] float singleTargetHealthBarRightPaddingMax = 390;

    PlayerController PlayerController => PlayerController.Instance;
    KinematicCharacterMotor Motor => PlayerController.Character.Motor;
    PlayerCanvas PlayerCanvas => PlayerController.UICanvas;
    PlayerStats PlayerStats => PlayerController.Stats as PlayerStats;

    protected override IDamageable IDamageable => PlayerController.IDamageable;
    protected override Transform DamageNumberSpawnTransform => Motor.Transform;
    protected override Vector3 DamageNumberSpawnPosition => DamageNumberSpawnTransform.position + new Vector3(0f, Motor.Capsule.height, 0f);
    protected override Stat MaxHealthStat => PlayerStats.GetStatFromName[CommonStatTypeNames.MaxHealth];
    protected Stat OutOfCombatHealthRegenCooldownStat => PlayerStats.GetStatFromName[CommonStatTypeNames.OutOfCombatHealthRegenCooldown];

    public override void UpdateHealthBar(float currentHealth, float projectedHealth, float maxHealth, EHealthChangedOperation operation = EHealthChangedOperation.NoChange, float healthChangeAmount = 0)
    {
        base.UpdateHealthBar(currentHealth, projectedHealth, maxHealth, operation, healthChangeAmount);

        if(PlayerCanvas.BuildMenuEnabled)
            PlayerCanvas.ToggleBuildMenu();
    }

    protected override void ShowThenHideFadeTweenUIComponent(TweenedUIComponent tweenedUIComponent, Action actionToDoOnShow)
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

        float cooldown = numSecondsToWaitBeforeHidingHealthBar;
        if(IDamageable.GetRoundedCurrentHealth() != MaxHealthStat.Value)
            cooldown += OutOfCombatHealthRegenCooldownStat.Value;

        currentHealthBarFadeSequence.AppendInterval(cooldown);
        currentHealthBarFadeSequence.AppendCallback(() => tweenedUIComponent.TweenUIComponent(true, new(){ETweenType.Fade}, false));
        currentHealthBarFadeSequence.Play();
    }

    protected override DamageNumberMesh HealOverTimeOperation(ref float healthChangeAmount)
    {
        return healthRegenHealNumberMesh;
    }

    private void UpdateSingleTargetHealthBar(float currentHealth, float projectedHealth, float maxHealth, EHealthChangedOperation operation = EHealthChangedOperation.NoChange, float healthChangeAmount = 0)
    {
        float projectedHealthPercentage = projectedHealth / maxHealth;
        float currentHealthPercentage = currentHealth / maxHealth;

        singleTargetHealthBarGhostMask.padding = new Vector4(singleTargetHealthBarGhostMask.padding.x, singleTargetHealthBarGhostMask.padding.y, Mathf.Lerp(singleTargetHealthBarRightPaddingMax, singleTargetHealthBarRightPaddingMin, projectedHealthPercentage), singleTargetHealthBarGhostMask.padding.w);
        singleTargetHealthBarMask.padding = new Vector4(singleTargetHealthBarMask.padding.x, singleTargetHealthBarMask.padding.y, Mathf.Lerp(singleTargetHealthBarRightPaddingMax, singleTargetHealthBarRightPaddingMin, currentHealthPercentage), singleTargetHealthBarMask.padding.w);

        string maxHealthString = maxHealth % 1 == 0
        ? maxHealth.ToString("F0")
        : maxHealth.ToString(CharacterStatsRoundingHelper.GlobalValueString);

        string currentHealthString = currentHealth % 1 == 0
        ? currentHealth.ToString("F0")
        : currentHealth.ToString(CharacterStatsRoundingHelper.GlobalValueString);

        singleTargetHealthText.text = currentHealthString + " / " + maxHealthString;
    }

    private void OnSingleTargetStatModifierChanged(Stat stat, StatModifier statModifier, EStatModifierChangedOperation operation)
    {
        if(stat.type != MaxHealthStat.type) return;
        
        UpdateSingleTargetHealthBar(IDamageable.GetRoundedCurrentHealth(), IDamageable.GetProjectedHealth(), stat.Value);
    }

    public void ToggleSingleTargetHealthBar(bool b, CharacterStats stats, IDamageable damageable)
    {
        singleTargetHealthHUD.gameObject.SetActive(b);

        if(b)
        {
            singleTargetNameText.text = damageable.GetDamageableName();
            UpdateSingleTargetHealthBar(damageable.GetRoundedCurrentHealth(), damageable.GetProjectedHealth(), stats.GetStatFromName[CommonStatTypeNames.MaxHealth].Value);

            damageable.OnHealthChanged += UpdateSingleTargetHealthBar;
            stats.OnStatModifierChanged += OnSingleTargetStatModifierChanged;
        }
        else
        {
            damageable.OnHealthChanged -= UpdateSingleTargetHealthBar;
            stats.OnStatModifierChanged -= OnSingleTargetStatModifierChanged;
        }
    }
}
