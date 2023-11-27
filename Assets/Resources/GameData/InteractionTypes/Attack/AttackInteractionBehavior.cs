using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AttackInteractionBehavior : InteractionTypeBehavior
{
    [SerializeField] StatTypeSO damageStatType;

    PlayerStats PlayerStats => PlayerController.Stats as PlayerStats;

    public override void Interact(IInteractable interactable)
    {
        IDamageable damageable = interactable.IDamageable;

        if(damageable == null) return;

        if(PlayerStats.GetStatFromType.TryGetValue(damageStatType, out Stat playerDamageStat))
            damageable.TakeDamageInstant(playerDamageStat.Value);
    }
}
