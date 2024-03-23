using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AttackInteractionBehavior : InteractionTypeBehavior
{
    [SerializeField] StatTypeSO damageStatType;

    PlayerStats PlayerStats => PlayerController.PlayerStats;

    public override void Interact(Interactable.InteractionTypeEntry interactionTypeEntry)
    {
        if(PlayerStats.GetStatFromType.TryGetValue(damageStatType, out Stat playerDamageStat))
            interactionTypeEntry.IDamageable.TakeDamageInstant(playerDamageStat.Value);
    }
}
