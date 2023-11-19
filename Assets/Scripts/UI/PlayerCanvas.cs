using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class PlayerCanvas : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] RectMask2D healthBarMask;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] float healthBarRightPaddingMin = 15;
    [SerializeField] float healthBarRightPaddingMax = 390;

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

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        float healthPercentage = currentHealth / maxHealth;
        healthBarMask.padding = new Vector4(healthBarMask.padding.x, healthBarMask.padding.y, Mathf.Lerp(healthBarRightPaddingMax, healthBarRightPaddingMin, healthPercentage), healthBarMask.padding.w);
        healthText.text = currentHealth.ToString("F0") + " / " + maxHealth.ToString("F0");
    }

    public void OnStatModifierChanged(Stat stat, StatModifier statModifier, EStatModifierChangedOperation operation)
    {
        if(stat.type != playerStats.GetStatTypeFromName[CommonStatTypeNames.MaxHealth]) return;
        
        UpdateHealthBar(playerStatsDamageable.GetHealth(), stat.Value);
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

        if(b) Cursor.lockState = CursorLockMode.None;
        else Cursor.lockState = CursorLockMode.Locked;
    }

    public void ToggleBuildHotbar(bool b)
    {
        TweenUIComponent(b, buildHotbar);
    }

    public void ToggleCrosshair(bool b)
    {
        TweenUIComponent(b, crosshair);
    }

#endregion

#region Tween UI Component

    private void TweenUIComponent(bool b, TweenedUIComponent tweenedUIComponent)
    {
        DOTween.Kill(tweenedUIComponent.RectTransform);

        if(b)
        {
            tweenedUIComponent.GameObj.SetActive(b);
            
            switch(tweenedUIComponent.TweenValues.TweenType)
            {
                case ETweenType.Scale:
                    tweenedUIComponent.RectTransform.DOScale(tweenedUIComponent.TweenValues.ScaleValues.EndScale, tweenedUIComponent.TweenDuration).SetEase(tweenedUIComponent.Ease);
                    break;
                case ETweenType.MoveY:
                    tweenedUIComponent.RectTransform.DOAnchorPosY(tweenedUIComponent.TweenValues.MoveYValues.EndPosY, tweenedUIComponent.TweenDuration).SetEase(tweenedUIComponent.Ease);
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch(tweenedUIComponent.TweenValues.TweenType)
            {
                case ETweenType.Scale:
                    tweenedUIComponent.RectTransform.DOScale(tweenedUIComponent.TweenValues.ScaleValues.StartScale, tweenedUIComponent.TweenDuration).SetEase(tweenedUIComponent.Ease).OnComplete(() => tweenedUIComponent.GameObj.SetActive(b));
                    break;
                case ETweenType.MoveY:
                    tweenedUIComponent.RectTransform.DOAnchorPosY(tweenedUIComponent.TweenValues.MoveYValues.StartPosY, tweenedUIComponent.TweenDuration).SetEase(tweenedUIComponent.Ease).OnComplete(() => tweenedUIComponent.GameObj.SetActive(b));
                    break;
                default:
                    break;
            }
        }
    }

    [System.Serializable]
    public class TweenedUIComponent
    {
        [SerializeField] GameObject gameObj;
        public GameObject GameObj => gameObj;
        public RectTransform RectTransform => gameObj.GetComponent<RectTransform>();

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
    public enum ETweenType
    {
        Scale,
        MoveY
    }

#endregion
}
