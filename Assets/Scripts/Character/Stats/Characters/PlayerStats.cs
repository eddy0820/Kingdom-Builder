using System;
using UnityEngine;
using EddyLib.Stats;
using EddyLib.GameSettingsSystem;

[Serializable]
public class PlayerStats : StaminaDamageableCharacterStats
{
    PlayerController PlayerController => PlayerController.Instance;
    PlayerCharacterController PlayerCharacterController => PlayerController.Character;
    PlayerCharacterStateMachine StateMachine => PlayerController.StateMachine;
    PlayerCharacterController.MovementAttributes MovementAttributes => PlayerCharacterController.Attributes;

    bool PlayerConsumesStamina => GameSettings.GetSettings<StatsSettings>().PlayerConsumesStamina;

    protected new void Start()
    {
        base.Start();

        GroundedCharacterState.OnGroundedMovementSprinting += DoStaminaReductionSprinting;
        GroundedCharacterState.OnGroundedMovementCrouching += DoStaminaReductionCrouch;

        statModifier = new StatModifier(1, StatModifierTypes.Flat);
    }

    StatModifier statModifier;

    private void Update()
    {
        /*if(Input.GetKeyUp(KeyCode.N))
        {
            //DepleteStaminaInstant(10);
            TakeDamageInstant(10);
        }

        if(Input.GetKeyUp(KeyCode.M))
        {
            //GainStaminaOverTimeToPercent(80, 10);
            HealOverTimeToPercent(100, 10);
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
            ApplyStatModifier(statModifier, CommonStatTypeNames.MaxStamina);
            //ApplyStatModifier(statModifier, CommonStatTypeNames.MaxHealth);
        }

        if(Input.GetKeyUp(KeyCode.X))
        {
            RemoveStatModifier(statModifier, CommonStatTypeNames.MaxStamina);
            //RemoveStatModifier(statModifier, CommonStatTypeNames.MaxHealth);
        }*/
    }

    private void DoStaminaReductionSprinting()
    {
        if(!PlayerConsumesStamina) return;

        float amountToReduce = MovementAttributes.SprintingStaminaCostPerSecond * Time.deltaTime;

        if(!HasEnoughStamina(amountToReduce))
        {
            PlayerCharacterController.SetIsSprinting(false);
            return;
        }

        DepleteStaminaInstant(amountToReduce);
    }

    private void DoStaminaReductionCrouch()
    {
        if(!PlayerConsumesStamina) return;
        
        float amountToReduce = MovementAttributes.CrouchingStaminaCostPerSecond * Time.deltaTime;

        if(!HasEnoughStamina(amountToReduce))
        {
            PlayerCharacterController.DoCrouchUp();
            return;
        }

        DepleteStaminaInstant(amountToReduce);
    }
}
