using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    [Space(10)]
    [ArrayElementTitle("interactionType")]
    [SerializeField] List<InteractionTypeEntry> interactionTypes;
    public List<InteractionTypeEntry> InteractionTypes => interactionTypes;


    [System.Serializable]
    public class InteractionTypeEntry
    {
        [SerializeField] InteractionTypeSO interactionType;
        public InteractionTypeSO InteractionType => interactionType;

        bool AttackInteraction => interactionType != null && interactionType.InteractionTypeBehaviorType == typeof(AttackInteractionBehavior);
        bool DepleteStaminaInteraction => interactionType != null && interactionType.InteractionTypeBehaviorType == typeof(DepleteStaminaInteractionBehavior);

        [AllowNesting, ShowIf("AttackInteraction"), SerializeField] DamageableCharacterStats damageableCharacterStats;
        public IDamageable IDamageable => damageableCharacterStats;

        [AllowNesting, ShowIf("DepleteStaminaInteraction"), SerializeField] StaminaDamageableCharacterStats staminaDamageableCharacterStats;
        public IStamina IStamina => staminaDamageableCharacterStats;
    }
}
