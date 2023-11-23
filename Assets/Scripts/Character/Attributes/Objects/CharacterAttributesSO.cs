using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public abstract class CharacterAttributesSO : ScriptableObject
{
    [Expandable, SerializeField] MovementAttributesSO movementAttributes;
    public MovementAttributesSO MovementAttributes => movementAttributes;
}
