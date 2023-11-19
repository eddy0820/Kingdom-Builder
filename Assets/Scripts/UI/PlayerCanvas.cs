using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerCanvas : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] TweenedUIComponent healthHUD;
    [SerializeField] TweenedUIComponent healthHUDFade;
    [Space(10)]
    [SerializeField] RectMask2D healthBarMask;
    [SerializeField] TextMeshProUGUI healthText;
    [Space(10)]
    [SerializeField] float healthBarRightPaddingMin = 15;
    [SerializeField] float healthBarRightPaddingMax = 390;
    [Space(10)]
    [SerializeField] float numSecondsAfterShowToDoCallback = 0.25f;
    [SerializeField] float numSecondsToWaitBeforeHidingHealthBar = 2f;

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

    IDamageable playerStatsDamageable;
    PlayerStats playerStats;

    private void Awake()
    {
        buildHotbarInterface = buildHotbar.GameObj.GetComponent<BuildHotbarInterface>();

        buildMenu.GameObj.SetActive(false);
        buildHotbar.GameObj.SetActive(false);

        crosshair.GameObj.SetActive(false);
        crosshair.RectTransform.localScale = Vector3.zero;

        playerStats = PlayerController.Instance.PlayerStats;
        playerStatsDamageable = PlayerController.Instance.PlayerStatsDamageable;

        playerStatsDamageable.OnHealthChanged += UpdateHealthBar;
        playerStats.OnStatModifierChanged += OnStatModifierChanged;
    }

#region HUD

    public void UpdateHealthBar(float oldHealth, float currentHealth, float maxHealth, EHealthChangedOperation operation)
    {
        ShowThenHideFadeTweenUIComponent(healthHUDFade, () =>
        {
            float healthPercentage = currentHealth / maxHealth;
            healthBarMask.padding = new Vector4(healthBarMask.padding.x, healthBarMask.padding.y, Mathf.Lerp(healthBarRightPaddingMax, healthBarRightPaddingMin, healthPercentage), healthBarMask.padding.w);
            healthText.text = currentHealth.ToString("F0") + " / " + maxHealth.ToString("F0");
        });

        if(buildMenuEnabled)
            ToggleBuildMenu();
    }

    public void OnStatModifierChanged(Stat stat, StatModifier statModifier, EStatModifierChangedOperation operation)
    {
        if(stat.type != playerStats.GetStatTypeFromName[CommonStatTypeNames.MaxHealth]) return;
        
        UpdateHealthBar(playerStatsDamageable.GetHealth(), playerStatsDamageable.GetHealth(), stat.Value, EHealthChangedOperation.NoChange);
    }

    public void ShowThenHideFadeTweenUIComponent(TweenedUIComponent tweenedUIComponent, Action actionToDoOnShow)
    {
        Tween fadeTween = tweenedUIComponent.Tweens.Find(t => t.TweenValues.TweenType == ETweenType.Fade);
        if(fadeTween == null) return;

        fadeTween.TweenValues.CanvasGroup.alpha = fadeTween.TweenValues.FadeValues.StartAlpha;

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(numSecondsAfterShowToDoCallback);
        sequence.AppendCallback(() => actionToDoOnShow?.Invoke());
        sequence.AppendInterval(numSecondsToWaitBeforeHidingHealthBar);
        sequence.AppendCallback(() => TweenUIComponent(true, tweenedUIComponent, new(){ETweenType.Fade}, false));
        sequence.Play();
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
        TweenUIComponent(b, buildMenu);
        ToggleCrosshair(!b);
        healthHUD.GameObj.SetActive(!b);


        if(b) Cursor.lockState = CursorLockMode.None;
        else Cursor.lockState = CursorLockMode.Locked;
    }

    public void ToggleBuildHotbar(bool b)
    {
        TweenUIComponent(b, buildHotbar);
        TweenUIComponent(b, healthHUD, new(){ETweenType.MoveY}, false);
    }

    public void ToggleCrosshair(bool b)
    {
        TweenUIComponent(b, crosshair);
    }

#endregion

#region Tween UI Component

    private void TweenUIComponent(bool b, TweenedUIComponent tweenedUIComponent, List<ETweenType> tweensToDo = null, bool setActiveGameObject = true)
    {
        tweenedUIComponent.RectTransform.DOKill();
        
        tweenedUIComponent.CurrentSequence?.Kill();
        Sequence sequence = DOTween.Sequence();

        if(b)
        {
            if(setActiveGameObject)
                tweenedUIComponent.GameObj.SetActive(b);

            foreach(Tween tween in tweenedUIComponent.Tweens)
            {
                if(tween.TweenValues.CanvasGroup != null)
                    tween.TweenValues.CanvasGroup.DOKill();

                if(tweensToDo != null && !tweensToDo.Contains(tween.TweenValues.TweenType)) continue;

                switch(tween.TweenValues.TweenType)
                {
                    case ETweenType.Scale:
                        sequence.Join(tweenedUIComponent.RectTransform.DOScale(tween.TweenValues.ScaleValues.EndScale, tween.TweenDuration).SetEase(tween.Ease));
                    break;
                    case ETweenType.MoveY:
                        sequence.Join(tweenedUIComponent.RectTransform.DOAnchorPosY(tween.TweenValues.MoveYValues.EndPosY, tween.TweenDuration).SetEase(tween.Ease));
                    break;
                    case ETweenType.Fade:
                        sequence.Join(tween.TweenValues.CanvasGroup.DOFade(tween.TweenValues.FadeValues.EndAlpha, tween.TweenDuration).SetEase(tween.Ease));
                    break;
                    default:
                    break;
                }
            }
        }
        else
        {
            foreach(Tween tween in tweenedUIComponent.Tweens)
            {
                if(tween.TweenValues.CanvasGroup != null)
                    tween.TweenValues.CanvasGroup.DOKill();

                if(tweensToDo != null && !tweensToDo.Contains(tween.TweenValues.TweenType)) continue;

                switch(tween.TweenValues.TweenType)
                {
                    case ETweenType.Scale:
                        sequence.Join(tweenedUIComponent.RectTransform.DOScale(tween.TweenValues.ScaleValues.StartScale, tween.TweenDuration).SetEase(tween.Ease).OnComplete(() => 
                        {
                            if(setActiveGameObject)
                                tweenedUIComponent.GameObj.SetActive(b);
                        }));
                    break;
                    case ETweenType.MoveY:
                        sequence.Join(tweenedUIComponent.RectTransform.DOAnchorPosY(tween.TweenValues.MoveYValues.StartPosY, tween.TweenDuration).SetEase(tween.Ease).OnComplete(() => 
                        {
                            if(setActiveGameObject)
                                tweenedUIComponent.GameObj.SetActive(b);
                        }));
                    break;
                    case ETweenType.Fade:
                        sequence.Join(tween.TweenValues.CanvasGroup.DOFade(tween.TweenValues.FadeValues.StartAlpha, tween.TweenDuration).SetEase(tween.Ease).OnComplete(() => 
                        {
                            if(setActiveGameObject)
                                tweenedUIComponent.GameObj.SetActive(b);
                        }));
                    break;
                    default:
                    break;
                }
            }
        }

        tweenedUIComponent.SetCurrentSequence(sequence);
        tweenedUIComponent.CurrentSequence.Play();
    }

    [System.Serializable]
    public class TweenedUIComponent
    {
        [SerializeField] GameObject gameObj;
        public GameObject GameObj => gameObj;
        public RectTransform RectTransform => gameObj.GetComponent<RectTransform>();

        [SerializeField] List<Tween> tweens;
        public List<Tween> Tweens => tweens;

        Sequence currentSequence;
        public Sequence CurrentSequence => currentSequence;

        public void SetCurrentSequence(Sequence sequence) => currentSequence = sequence;
    }

    [System.Serializable]
    public class Tween
    {
        [SerializeField] TweenValues tweenValues;
        public TweenValues TweenValues => tweenValues;

        [SerializeField] float tweenDuration;
        public float TweenDuration => tweenDuration;

        [SerializeField] Ease ease;
        public Ease Ease => ease;
    }

    [System.Serializable]
    public class TweenValues 
    {
        [SerializeField] ETweenType tweenType;
        public ETweenType TweenType => tweenType;

        [AllowNesting]
        [ShowIf("tweenType", ETweenType.MoveY), SerializeField]  MoveY moveYValues;
        public MoveY MoveYValues => moveYValues;

        [AllowNesting]
        [ShowIf("tweenType", ETweenType.Scale), SerializeField] Scale scaleValues;
        public Scale ScaleValues => scaleValues;
        
        [AllowNesting]
        [ShowIf("tweenType", ETweenType.Fade), SerializeField] CanvasGroup canvasGroup;
        public CanvasGroup CanvasGroup => canvasGroup;

        [AllowNesting]
        [ShowIf("tweenType", ETweenType.Fade), SerializeField] Fade fadeValues;
        public Fade FadeValues => fadeValues;
    }

    [System.Serializable]
    public class MoveY
    {
        [SerializeField] float startPosY;
        public float StartPosY => startPosY;

        [SerializeField] float endPosY;
        public float EndPosY => endPosY;
    }

    [System.Serializable]
    public class Scale
    {
        [SerializeField] float startScale;
        public float StartScale => startScale;

        [SerializeField] float endScale;
        public float EndScale => endScale;
    }

    [System.Serializable]
    public class Fade
    {
        [SerializeField] float startAlpha;
        public float StartAlpha => startAlpha;

        [SerializeField] float endAlpha;
        public float EndAlpha => endAlpha;
    }

    [System.Serializable]
    public enum ETweenType
    {
        Scale,
        MoveY,
        Fade
    }

#endregion
}
