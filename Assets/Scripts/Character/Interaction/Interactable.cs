using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    [SerializeField] bool canTakeDamage = false;
    [ShowIf("canTakeDamage"), SerializeField] DamageableCharacterStats damageableCharacterStats;

    [Space(10)]
    [SerializeField] bool hasStamina = false;
    [ShowIf("hasStamina"), SerializeField] StaminaDamageableCharacterStats staminaDamageableCharacterStats;

    [Space(10)]
    [SerializeField] List<InteractionTypeSO> interactionTypes;
    public List<InteractionTypeSO> InteractionTypes => interactionTypes;

    public IDamageable IDamageable => damageableCharacterStats;
    public IStamina IStamina => staminaDamageableCharacterStats;
}
