using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DepleteStaminaInteractionBehavior : InteractionTypeBehavior
{
    [SerializeField] StatTypeSO damageStatType;

    PlayerStats PlayerStats => PlayerController.PlayerStats;

    public override void Interact(Interactable interactable)
    {
        IStamina stamina = interactable.IStamina;

        if(stamina == null) return;

        if(PlayerStats.GetStatFromType.TryGetValue(damageStatType, out Stat playerDamageStat))
            stamina.DepleteStaminaInstant(playerDamageStat.Value);
    }
}
