using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class PlayerStats : StaminaDamageableCharacterStats
{
    [Space(10)]

    [SerializeField] float staminaDepletionInterval = 0.05f;

    float lastTimeStaminaReduced = 0;
    Coroutine crouchStaminaCoroutine;

    PlayerController PlayerController => PlayerController.Instance;
    PlayerCharacterStateMachine StateMachine => PlayerController.StateMachine;
    PlayerCharacterController.MovementAttributes MovementAttributes => PlayerController.Character.Attributes;

    protected override void OnStart()
    {
        base.OnStart();

        StateMachine.OnGroundedMovementSprinting += DoStaminaReduction;
        StateMachine.OnGroundedMovementCrouching += DoStaminaReductionCrouch;

        statModifier = new StatModifier(1, StatModifierTypes.Flat);
    }

    StatModifier statModifier;

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.N))
        {
            //DepleteStaminaInstant(10);
            TakeDamageInstant(10);
        }

        if(Input.GetKeyUp(KeyCode.M))
        {
            //GainStaminaOverTimeToPercent(80, 10);
            HealOverTimeToPercent(80, 10);
        }

        if(Input.GetKeyUp(KeyCode.B))
        {
            //GainStaminaOverTime(10, 5);
            HealOverTime(10, 5);
        }

        if(Input.GetKeyUp(KeyCode.V))
        {
            //GainStaminaInstant(20);
            HealInstant(20);
        }

        if(Input.GetKeyUp(KeyCode.C))
        {
            //ApplyStatModifier(statModifier, CommonStatTypeNames.MaxStamina);
            ApplyStatModifier(statModifier, CommonStatTypeNames.MaxHealth);
        }

        if(Input.GetKeyUp(KeyCode.X))
        {
            //RemoveStatModifier(statModifier, CommonStatTypeNames.MaxStamina);
            RemoveStatModifier(statModifier, CommonStatTypeNames.MaxHealth);
        }
    }

    private void DoStaminaReduction(bool shouldBeReducing)
    {
        if(shouldBeReducing)
        {
            if(Time.time - lastTimeStaminaReduced > staminaDepletionInterval)
            {
                DepleteStaminaInstant(MovementAttributes.SprintingStaminaCostPerSecond * staminaDepletionInterval);
                lastTimeStaminaReduced = Time.time;
            }
        }
    }

    private void DoStaminaReductionCrouch(bool shouldBeReducing)
    {
        if(shouldBeReducing && crouchStaminaCoroutine == null)
        {
            crouchStaminaCoroutine = StartCoroutine(HandleCrouchingStaminaCoroutine());
        }  
        else
        {
            if(crouchStaminaCoroutine != null)
            {
                StopCoroutine(crouchStaminaCoroutine);
                crouchStaminaCoroutine = null;
            }
        }
    }

    IEnumerator HandleCrouchingStaminaCoroutine()
    {
        while(true)
        {
            DepleteStaminaInstant(MovementAttributes.CrouchingStaminaCostPerSecond * staminaDepletionInterval);
            yield return new WaitForSeconds(staminaDepletionInterval);
        }
    }
}
