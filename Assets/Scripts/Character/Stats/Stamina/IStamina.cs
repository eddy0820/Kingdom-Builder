using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStamina
{
    public void SetStamina(float amount, bool setAsNoChange = false);

    public float GetCurrentStamina();
    public float GetProjectedStamina();

    public void DepleteStaminaInstant(float amount);

    public void GainStaminaInstant(float amount);
    public void GainStaminaOverTime(float percentAmount, float duration);
    public void GainStaminaOverTimeToPercent(float percent, float duration);
    public IEnumerator GainStaminaOverTimeCoroutine();

    public Action<float, float, float, EStaminaChangedOperation, float> OnStaminaChanged { get; set; }

    public string GetStaminaName();

    public bool HasEnoughStamina(float amount);
}
