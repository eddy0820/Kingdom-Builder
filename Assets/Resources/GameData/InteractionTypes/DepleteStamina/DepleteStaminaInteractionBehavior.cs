using UnityEngine;
using EddyLib.Stats;

public class DepleteStaminaInteractionBehavior : InteractionTypeBehavior
{
    [SerializeField] StatTypeSO damageStatType;

    PlayerStats PlayerStats => PlayerController.PlayerStats;

    public override void Interact(Interactable.InteractionTypeEntry interactionTypeEntry)
    {

        if(PlayerStats.GetStatFromType.TryGetValue(damageStatType, out Stat playerDamageStat))
            interactionTypeEntry.IStamina.DepleteStaminaInstant(playerDamageStat.Value);
    }
}
