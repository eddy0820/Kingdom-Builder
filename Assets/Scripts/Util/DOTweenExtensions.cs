using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public static class DOTweenExtensions
{
    public static Ease GetInverseEase(Ease ease)
    {
        switch(ease)
        {
            case Ease.InBack:
                return Ease.OutBack;
            case Ease.InBounce:
                return Ease.OutBounce;
            case Ease.InCirc:
                return Ease.OutCirc;
            case Ease.InCubic:
                return Ease.OutCubic;
            case Ease.InElastic:
                return Ease.OutElastic;
            case Ease.InExpo:
                return Ease.OutExpo;
            case Ease.InFlash:
                return Ease.OutFlash;
            case Ease.InQuad:
                return Ease.OutQuad;
            case Ease.InQuart:
                return Ease.OutQuart;
            case Ease.InQuint:
                return Ease.OutQuint;
            case Ease.InSine:
                return Ease.OutSine;
            case Ease.OutBack:
                return Ease.InBack;
            case Ease.OutBounce:
                return Ease.InBounce;
            case Ease.OutCirc:
                return Ease.InCirc;
            case Ease.OutCubic:
                return Ease.InCubic;
            case Ease.OutElastic:
                return Ease.InElastic;
            case Ease.OutExpo:
                return Ease.InExpo;
            case Ease.OutFlash:
                return Ease.InFlash;
            case Ease.OutQuad:
                return Ease.InQuad;
            case Ease.OutQuart:
                return Ease.InQuart;
            case Ease.OutQuint:
                return Ease.InQuint;
            case Ease.OutSine:
                return Ease.InSine;
            default:
                return ease;
        }
    }
}
