using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AttackInteractionBehavior : InteractionTypeBehavior
{
    [SerializeField] StatTypeSO damageStatType;

    PlayerStats PlayerStats => PlayerController.PlayerStats;

    public override void Interact(Interactable interactable)
    {
        IDamageable damageable = interactable.IDamageable;

        if(damageable == null) return;

        if(PlayerStats.GetStatFromType.TryGetValue(damageStatType, out Stat playerDamageStat))
            damageable.TakeDamageInstant(playerDamageStat.Value);
    }
}
