using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;



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

public static class TweenedUIComponentExtensions
{
    public static void TweenUIComponent(this TweenedUIComponent tweenedUIComponent, bool b, List<ETweenType> tweensToDo = null, bool setActiveGameObject = true)
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
                    case ETweenType.Rotate360:
                        sequence.Join(tweenedUIComponent.RectTransform.DORotate(new Vector3(0, 0, tween.TweenValues.Rotate360Values.EndRotation), tween.TweenDuration, RotateMode.FastBeyond360).SetEase(tween.Ease));
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
                        sequence.Join(tweenedUIComponent.RectTransform.DOScale(tween.TweenValues.ScaleValues.StartScale, tween.TweenDuration).SetEase(tween.UseInverseEaseForEndTween ? DOTweenExtensions.GetInverseEase(tween.Ease) : tween.Ease).OnComplete(() => 
                        {
                            if(setActiveGameObject)
                                tweenedUIComponent.GameObj.SetActive(b);
                        }));
                    break;
                    case ETweenType.MoveY:
                        sequence.Join(tweenedUIComponent.RectTransform.DOAnchorPosY(tween.TweenValues.MoveYValues.StartPosY, tween.TweenDuration).SetEase(tween.UseInverseEaseForEndTween ? DOTweenExtensions.GetInverseEase(tween.Ease) : tween.Ease).OnComplete(() => 
                        {
                            if(setActiveGameObject)
                                tweenedUIComponent.GameObj.SetActive(b);
                        }));
                    break;
                    case ETweenType.Fade:
                        sequence.Join(tween.TweenValues.CanvasGroup.DOFade(tween.TweenValues.FadeValues.StartAlpha, tween.TweenDuration).SetEase(tween.UseInverseEaseForEndTween ? DOTweenExtensions.GetInverseEase(tween.Ease) : tween.Ease).OnComplete(() => 
                        {
                            if(setActiveGameObject)
                                tweenedUIComponent.GameObj.SetActive(b);
                        }));
                    break;
                    case ETweenType.Rotate360:
                        sequence.Join(tweenedUIComponent.RectTransform.DORotate(new Vector3(0, 0, tween.TweenValues.Rotate360Values.StartRotation), tween.TweenDuration, RotateMode.FastBeyond360).SetEase(DOTweenExtensions.GetInverseEase(tween.Ease)).OnComplete(() => 
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

    [SerializeField] bool useInverseEaseForEndTween;
    public bool UseInverseEaseForEndTween => useInverseEaseForEndTween;
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

    [AllowNesting]
    [ShowIf("tweenType", ETweenType.Rotate360), SerializeField] Rotate360 rotate360Values;
    public Rotate360 Rotate360Values => rotate360Values;
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
public class Rotate360
{
    [SerializeField] float startRotation;
    public float StartRotation => startRotation;

    [SerializeField] float endRotation;
    public float EndRotation => endRotation;
}

[System.Serializable]
public enum ETweenType
{
    Scale,
    MoveY,
    Fade,
    Rotate360
}
